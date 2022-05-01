param(
  [ValidateSet("dev", "prod")]
  [Parameter(Mandatory = $True)]
  [string]$EnvironmentType
)

$ErrorActionPreference = "Stop"
Import-Module $PSScriptRoot/common.psm1 -Force

Write-Host "Deploying background jobs to $EnvironmentType environment.";

dotnet lambda deploy-serverless `
  --project-location $PSScriptRoot/../SeiyuuMoe.MalBackgroundJobs `
  --framework 'net6.0' `
  --stack-name seiyuu-moe-mal-bg-jobs-$EnvironmentType `
  --s3-bucket seiyuu-moe-deploy-$EnvironmentType `
  --s3-prefix mal-bg-jobs-$EnvironmentType/ `
  --template $PSScriptRoot/../SeiyuuMoe.MalBackgroundJobs.Lambda/application.yaml `
  --template-parameters "EnvironmentType=$EnvironmentType" `
  --capabilities CAPABILITY_NAMED_IAM `
  --region eu-central-1 `
  --tags "environment=${EnvironmentType}" `
  --configuration Release; StopOnNativeError