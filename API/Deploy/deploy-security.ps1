param(
    [ValidateSet("dev", "prod")]
    [Parameter(Mandatory = $True)]
    [string]$EnvironmentType
)

$ErrorActionPreference = "Stop"
Import-Module $PSScriptRoot/common.psm1 -Force

Write-Host "Deploying security.yaml to $EnvironmentType environment."

aws cloudformation deploy `
    --template-file $PSScriptRoot/../Environment/security.yaml `
    --stack-name seiyuu-moe-sec-$EnvironmentType `
    --parameter-overrides "EnvironmentType=${EnvironmentType}" `
    --capabilities CAPABILITY_NAMED_IAM `
    --tags "environment=${EnvironmentType}" `
    --no-fail-on-empty-changeset ; StopOnNativeError