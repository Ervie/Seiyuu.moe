function StopOnNativeError
{
  if ($lastexitcode -ne 0)
  {
    Write-Error "Native command failed with non-zero error code" -ErrorAction Stop
  }
}