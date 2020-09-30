param(
  [ValidateSet("dev", "prod")]
  [Parameter(Mandatory = $True)]
  [string]$EnvironmentType
)

$ErrorActionPreference = "Stop"

. $PSScriptRoot/deploy-mal-bg-jobs.ps1 $EnvironmentType