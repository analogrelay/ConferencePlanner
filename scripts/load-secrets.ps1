function yn($prompt) {
    while ($true) {
        $response = (Read-Host -Prompt "$prompt [y/N] ? ").Trim().ToLowerInvariant()

        if (!$response -or $response -eq "n") {
            return $false
        }
        elseif ($response -eq "y") {
            return $true
        }
    }
}

function loadsecrets($projectPath) {
    $secrets = @{};
    Push-Location $projectPath | Out-Null
    try {
        dotnet user-secrets list | ForEach-Object {
            $splat = $_.Split("=");
            $key = $splat[0].Trim();
            $value = $splat[1].Trim();
            $secrets[$key] = $value
        }
    }
    finally {
        Pop-Location | Out-Null
    }

    $secrets
}

function setenv($envName, $value) {
    Write-Host "Setting $envName ..."
    Set-Content "env:\$envName" $value
}

Write-Host -ForegroundColor Yellow "This command will use the 'dotnet user-secrets' to load secrets into the current environment in order to allow for Docker development."
Write-Host
Write-Host -ForegroundColor Yellow "Please remember to use the 'unload-secrets.ps1' script to unload the secrets when you are done!"
Write-Host
Write-Host -ForegroundColor Yellow "Finally, this script should only be used for developer-time secrets. Production secrets should be tracked in a more secure manner."

if (!(yn "Do you understand")) {
    throw "User cancelled"
}

$BackEndSecrets = loadsecrets "$PSScriptRoot\..\src\BackEnd"
$FrontEndSecrets = loadsecrets "$PSScriptRoot\..\src\FrontEnd"

setenv "ConferencePlanner_BackEnd_AzureAdClientId" $BackEndSecrets["Authentication:ClientId"]
setenv "ConferencePlanner_AzureAdTenant" $BackEndSecrets["Authentication:Tenant"]
setenv "ConferencePlanner_FrontEnd_AzureAdClientId" $FrontEndSecrets["Authentication:ClientId"]
setenv "ConferencePlanner_FrontEnd_AzureAdClientSecret" $FrontEndSecrets["Authentication:ClientSecret"]