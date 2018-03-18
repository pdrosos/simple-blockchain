$scriptDir = Split-Path -Path $MyInvocation.MyCommand.Definition -Parent

Write-Host $scriptDir

$nodeApiDir =  $scriptDir + '\Node.Api'

Write-Host $nodeApiDir

$minerConsoleDir = $scriptDir + '\Miner.Console'

Write-Host $minerConsoleDir

$consoleArg = 'run --server.urls=http://localhost:'

Write-Host "Starting nodes..."

For ($port=5555; $port -le 5560; $port++) {

  $ScriptBlockNodes = {
    param($port, $nodeApiDir) 
    $nodeArgs = 'run --server.urls=http://localhost:' + $port
    Write-Host $nodeArgs
    Start-Process -FilePath "dotnet" -NoNewWindow -WorkingDirectory $nodeApiDir -ArgumentList $nodeArgs
  }

  # Show the loop variable here is correct
  Write-Host "processing $port..."

  # pass the loop variable across the job-context barrier
  Start-Job $ScriptBlockNodes -ArgumentList $port, $nodeApiDir
}

# Wait for all to complete
While (Get-Job -State "Running") { Start-Sleep -seconds 14 }

# Write-Host "Starting miners..."

# For ($port=5570; $port -le 5570; $port++) {

  # $ScriptBlockMiners = {
    # param($port, $minerConsoleDir)
    # $randomHexString = Get-Random -Maximum 100
    # Write-Host $randomHexString
    # $minerArgs = 'bin\Debug\netcoreapp2.0\Miner.Console.dll http://localhost:' + ($port - 15) + ' ' + $randomHexString
    # Write-Host $minerArgs
    # Start-Process -FilePath "dotnet" -NoNewWindow -WorkingDirectory $minerConsoleDir -ArgumentList $minerArgs
  # }

  # # Show the loop variable here is correct
  # Write-Host "processing $port..."

  # # pass the loop variable across the job-context barrier
  # Start-Job $ScriptBlockMiners -ArgumentList $port, $minerConsoleDir
# }

# # Wait for all to complete
# While (Get-Job -State "Running") { Start-Sleep -seconds 4 }

# Display output from all jobs
# Get-Job | Receive-Job

# Cleanup
Remove-Job *
