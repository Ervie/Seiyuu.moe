param(
  [ValidateSet("dev", "prod")]
  [Parameter(Mandatory = $True)]
  [string]$EnvironmentType
)

$ErrorActionPreference = "Stop"

Write-Host "Deploying background jobs to $EnvironmentType environment.";

dotnet lambda deploy serveless `
  --project-location $PScriptRoot/../SeiyuuMoe.JikanToDBParser `
  --framework 'netcoreapp3.1'
  --stack-name seiyuu-moe-mal-bg-jobs-$EnvironmentType
  --s3-bucket seiyuu-moe-deploy-$EnvironmentType
  --template $PScriptRoot/../SeiyuuMoe.JikanToDBParser/application.yaml `
  --template-parameters "EnvironmentType=$EnvironmentType"
  --capabilities CAPABILITY_NAMED_IAM `
  --region eu-central-1 `
  --tags "environment=${EnvironmentType}"
  --configuration Release; StopOnNativeError