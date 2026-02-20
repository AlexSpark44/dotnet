# Pulumi Azure Infrastructure (C#)

This project provisions Azure resources for the Branch Platform API on Azure Container Apps.

## Provisioned resources
- Resource Group
- Log Analytics Workspace
- Application Insights
- Azure Container Registry
- Azure Container Apps Environment
- Azure Container App (API)
- Azure Database for PostgreSQL Flexible Server + database + firewall rule
- Azure Cache for Redis
- Azure Key Vault + secrets for connection strings

## Config
Set via `pulumi config set` or `Pulumi.<stack>.yaml`:
- `branch-platform-infra:location`
- `branch-platform-infra:environmentName`
- `branch-platform-infra:containerImage`
- `branch-platform-infra:postgresAdminUser`
- `branch-platform-infra:jwtAudience`
- `branch-platform-infra:jwtAuthority`

## Run
```bash
cd infra
pulumi stack init dev # once
pulumi up -s dev
```
