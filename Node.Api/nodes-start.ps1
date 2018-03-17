$scriptDir = Split-Path -Path $MyInvocation.MyCommand.Definition -Parent

Write-Host $scriptDir

For ($i=5555; $i -le 5560; $i++) {

  $ScriptBlock = {
    param($i, $scriptDir) 
    $args = "run --server.urls=http://localhost:" + "$i"
    Start-Process -NoNewWindow -FilePath 'dotnet' -WorkingDirectory $scriptDir -ArgumentList $args
  }

  # Show the loop variable here is correct
  Write-Host "processing $i..."

  # pass the loop variable across the job-context barrier
  Start-Job $ScriptBlock -ArgumentList $i, $scriptDir
}

# Wait for all to complete
While (Get-Job -State "Running") { Start-Sleep -seconds 4 }

# Display output from all jobs
Get-Job | Receive-Job

# Cleanup
Remove-Job *