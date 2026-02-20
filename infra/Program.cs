using Pulumi;
using Pulumi.AzureNative.App;
using Pulumi.AzureNative.App.Inputs;
using Pulumi.AzureNative.Authorization;
using Pulumi.AzureNative.Cache;
using Pulumi.AzureNative.Cache.Inputs;
using Pulumi.AzureNative.ContainerRegistry;
using Pulumi.AzureNative.DBforPostgreSQL.FlexibleServers;
using Pulumi.AzureNative.DBforPostgreSQL.FlexibleServers.Inputs;
using Pulumi.AzureNative.Insights;
using Pulumi.AzureNative.KeyVault;
using Pulumi.AzureNative.KeyVault.Inputs;
using Pulumi.AzureNative.OperationalInsights;
using Pulumi.AzureNative.OperationalInsights.Inputs;
using Pulumi.AzureNative.Resources;
using Pulumi.Random;
using KeyVaultSecret = Pulumi.AzureNative.KeyVault.Secret;

return await Deployment.RunAsync(() =>
{
    var config = new Config();
    var location = config.Get("location") ?? "eastus";
    var environmentName = config.Get("environmentName") ?? "branch-platform-dev";
    var containerImage = config.Require("containerImage");
    var postgresAdminUser = config.Get("postgresAdminUser") ?? "branchadmin";
    var jwtAudience = config.Get("jwtAudience") ?? "branch-platform-api";
    var jwtAuthority = config.Get("jwtAuthority") ?? "https://your-oidc-provider";

    var suffix = new RandomString("suffix", new RandomStringArgs
    {
        Length = 6,
        Upper = false,
        Lower = true,
        Number = true,
        Special = false
    });

    var resourceGroup = new ResourceGroup("branch-platform-rg", new ResourceGroupArgs
    {
        Location = location,
        ResourceGroupName = Output.Format($"{environmentName}-rg")
    });

    var workspace = new Workspace("branch-platform-law", new WorkspaceArgs
    {
        ResourceGroupName = resourceGroup.Name,
        Location = resourceGroup.Location,
        WorkspaceName = Output.Format($"{environmentName}-law"),
        Sku = new WorkspaceSkuArgs { Name = "PerGB2018" },
        RetentionInDays = 30
    });

    var appInsights = new Component("branch-platform-ai", new ComponentArgs
    {
        ResourceGroupName = resourceGroup.Name,
        Location = resourceGroup.Location,
        ApplicationType = "web",
        Kind = "web",
        WorkspaceResourceId = workspace.Id
    });

    var registry = new Registry("branch-platform-acr", new RegistryArgs
    {
        ResourceGroupName = resourceGroup.Name,
        Location = resourceGroup.Location,
        RegistryName = Output.Format($"{environmentName}acr{suffix.Result}"),
        Sku = new Pulumi.AzureNative.ContainerRegistry.Inputs.SkuArgs { Name = "Basic" },
        AdminUserEnabled = true
    });

    var postgresPassword = new RandomPassword("postgres-password", new RandomPasswordArgs
    {
        Length = 24,
        Special = true
    });

    var postgresServer = new Server("branch-platform-postgres", new ServerArgs
    {
        ResourceGroupName = resourceGroup.Name,
        Location = resourceGroup.Location,
        ServerName = Output.Format($"{environmentName}-pg-{suffix.Result}"),
        Version = ServerVersion.Ver16,
        AdministratorLogin = postgresAdminUser,
        AdministratorLoginPassword = postgresPassword.Result,
        Sku = new SkuArgs { Name = "Standard_B1ms", Tier = SkuTier.Burstable },
        Storage = new StorageArgs { StorageSizeGB = 64 },
        Backup = new BackupArgs { BackupRetentionDays = 7, GeoRedundantBackup = GeoRedundantBackupEnum.Disabled },
        HighAvailability = new HighAvailabilityArgs { Mode = HighAvailabilityMode.Disabled },
        CreateMode = CreateMode.Create,
        Network = new NetworkArgs { PublicNetworkAccess = ServerPublicNetworkAccessState.Enabled }
    });

    _ = new Database("branch-platform-postgres-db", new DatabaseArgs
    {
        ResourceGroupName = resourceGroup.Name,
        ServerName = postgresServer.Name,
        DatabaseName = "branch_platform",
        Charset = "UTF8",
        Collation = "en_US.utf8"
    });

    _ = new FirewallRule("branch-platform-postgres-fw", new FirewallRuleArgs
    {
        ResourceGroupName = resourceGroup.Name,
        ServerName = postgresServer.Name,
        FirewallRuleName = "allow-azure-services",
        StartIpAddress = "0.0.0.0",
        EndIpAddress = "0.0.0.0"
    });

    var redis = new Redis("branch-platform-redis", new RedisArgs
    {
        ResourceGroupName = resourceGroup.Name,
        Location = resourceGroup.Location,
        Name = Output.Format($"{environmentName}-redis-{suffix.Result}"),
        Sku = new RedisSkuArgs { Family = "C", Name = "Basic", Capacity = 0 },
        EnableNonSslPort = false,
        MinimumTlsVersion = "1.2"
    });

    var clientConfig = GetClientConfig.Invoke();

    var keyVault = new Vault("branch-platform-kv", new VaultArgs
    {
        ResourceGroupName = resourceGroup.Name,
        Location = resourceGroup.Location,
        VaultName = Output.Format($"{environmentName}-kv-{suffix.Result}"),
        Properties = new VaultPropertiesArgs
        {
            TenantId = clientConfig.Apply(c => c.TenantId),
            Sku = new Pulumi.AzureNative.KeyVault.Inputs.SkuArgs { Family = "A", Name = "standard" },
            AccessPolicies =
            {
                new AccessPolicyEntryArgs
                {
                    TenantId = clientConfig.Apply(c => c.TenantId),
                    ObjectId = clientConfig.Apply(c => c.ObjectId),
                    Permissions = new PermissionsArgs { Secrets = { "Get", "Set", "List", "Delete" } }
                }
            },
            EnabledForDeployment = true,
            EnableSoftDelete = true,
            SoftDeleteRetentionInDays = 7,
            EnablePurgeProtection = false,
            PublicNetworkAccess = "Enabled"
        }
    });

    var postgresConnection = Output.Format(
        $"Host={postgresServer.Name}.postgres.database.azure.com;Port=5432;Database=branch_platform;Username={postgresAdminUser};Password={postgresPassword.Result};Ssl Mode=Require;Trust Server Certificate=false");

    _ = new KeyVaultSecret("postgres-connection-secret", new Pulumi.AzureNative.KeyVault.SecretArgs
    {
        ResourceGroupName = resourceGroup.Name,
        VaultName = keyVault.Name,
        SecretName = "PostgresConnection",
        Properties = new SecretPropertiesArgs { Value = postgresConnection }
    });

    var redisKeys = ListRedisKeys.Invoke(new ListRedisKeysInvokeArgs
    {
        ResourceGroupName = resourceGroup.Name,
        Name = redis.Name
    });

    var redisConnection = Output.Format($"{redis.HostName}:6380,password={redisKeys.Apply(k => k.PrimaryKey)},ssl=True,abortConnect=False");

    _ = new KeyVaultSecret("redis-connection-secret", new Pulumi.AzureNative.KeyVault.SecretArgs
    {
        ResourceGroupName = resourceGroup.Name,
        VaultName = keyVault.Name,
        SecretName = "RedisConnection",
        Properties = new SecretPropertiesArgs { Value = redisConnection }
    });

    var sharedKeys = GetSharedKeys.Invoke(new GetSharedKeysInvokeArgs
    {
        ResourceGroupName = resourceGroup.Name,
        WorkspaceName = workspace.Name
    });

    var cae = new ManagedEnvironment("branch-platform-cae", new ManagedEnvironmentArgs
    {
        ResourceGroupName = resourceGroup.Name,
        Location = resourceGroup.Location,
        AppLogsConfiguration = new AppLogsConfigurationArgs
        {
            Destination = "log-analytics",
            LogAnalyticsConfiguration = new LogAnalyticsConfigurationArgs
            {
                CustomerId = workspace.CustomerId,
                SharedKey = sharedKeys.Apply(k => k.PrimarySharedKey)
            }
        }
    });

    var acrCredentials = ListRegistryCredentials.Invoke(new ListRegistryCredentialsInvokeArgs
    {
        ResourceGroupName = resourceGroup.Name,
        RegistryName = registry.Name
    });

    var app = new ContainerApp("branch-platform-api", new ContainerAppArgs
    {
        ResourceGroupName = resourceGroup.Name,
        ManagedEnvironmentId = cae.Id,
        Configuration = new ConfigurationArgs
        {
            Registries =
            {
                new RegistryCredentialsArgs
                {
                    Server = registry.LoginServer,
                    Username = acrCredentials.Apply(c => c.Username),
                    PasswordSecretRef = "acr-password"
                }
            },
            Secrets =
            {
                new SecretArgs { Name = "acr-password", Value = acrCredentials.Apply(c => c.Passwords[0].Value) },
                new SecretArgs { Name = "postgres-connection", Value = postgresConnection },
                new SecretArgs { Name = "redis-connection", Value = redisConnection }
            },
            Ingress = new IngressArgs { External = true, TargetPort = 8080, Transport = "Auto" }
        },
        Template = new TemplateArgs
        {
            Containers =
            {
                new ContainerArgs
                {
                    Name = "branch-platform-api",
                    Image = Output.Format($"{registry.LoginServer}/{containerImage}"),
                    Resources = new ContainerResourcesArgs { Cpu = 0.5, Memory = "1Gi" },
                    Env =
                    {
                        new EnvironmentVarArgs { Name = "ASPNETCORE_URLS", Value = "http://+:8080" },
                        new EnvironmentVarArgs { Name = "ConnectionStrings__Postgres", SecretRef = "postgres-connection" },
                        new EnvironmentVarArgs { Name = "ConnectionStrings__Redis", SecretRef = "redis-connection" },
                        new EnvironmentVarArgs { Name = "Authentication__Authority", Value = jwtAuthority },
                        new EnvironmentVarArgs { Name = "Authentication__Audience", Value = jwtAudience },
                        new EnvironmentVarArgs { Name = "APPLICATIONINSIGHTS_CONNECTION_STRING", Value = appInsights.ConnectionString }
                    }
                }
            },
            Scale = new ScaleArgs { MinReplicas = 1, MaxReplicas = 3 }
        }
    });

    return new Dictionary<string, object?>
    {
        ["resourceGroupName"] = resourceGroup.Name,
        ["containerRegistryServer"] = registry.LoginServer,
        ["containerAppUrl"] = app.LatestRevisionFqdn.Apply(host => $"https://{host}"),
        ["postgresServerName"] = postgresServer.Name,
        ["redisHost"] = redis.HostName,
        ["keyVaultName"] = keyVault.Name,
        ["applicationInsightsConnectionString"] = appInsights.ConnectionString
    };
});
