Get-ChildItem env:\ConferencePlanner_* | ForEach-Object {
    Write-Host "Unsetting $($_.Name)"
    Remove-Item "env:\$($_.Name)" 
}