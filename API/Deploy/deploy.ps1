param(
  [ValidateSet("dev", "prod")]
  [Parameter(Mandatory = $True)]
  [string]$EnvironmentType
)

$ErrorActionPreference = "Stop"

. $PSScriptRoot/deploy-security.ps1 $EnvironmentType
. $PSScriptRoot/deploy-mal-bg-jobs.ps1 $EnvironmentType