# Deployment Helper (C#)

This folder provides a C# CLI wrapper to build, push, and deploy the API image.

## Prerequisites
- Docker
- Pulumi CLI
- Azure authentication (`az login`)

## Usage
```bash
dotnet run --project deploy/src/Branch.Platform.Deploy -- build
dotnet run --project deploy/src/Branch.Platform.Deploy -- push
dotnet run --project deploy/src/Branch.Platform.Deploy -- deploy
```

Environment variables:
- `BRANCH_ACR_SERVER` (e.g. `myregistry.azurecr.io`)
- `BRANCH_IMAGE_NAME` (default `branchplatformapi`)
- `BRANCH_IMAGE_TAG` (default `latest`)
- `BRANCH_PULUMI_STACK` (default `dev`)
