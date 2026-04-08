# Export OpenAPI Specification
# This script downloads the OpenAPI spec from your running API
# Frontend teams can use this to generate client SDKs

param(
    [string]$ApiUrl = "https://localhost:5001",
    [string]$Version = "1",
    [string]$OutputFile = "openapi-spec.json",
    [switch]$SkipCertificateCheck = $true
)

Write-Host "?? Exporting OpenAPI Specification..." -ForegroundColor Cyan
Write-Host ""

# Construct the OpenAPI endpoint URL
$openApiUrl = "$ApiUrl/openapi/v$Version.json"

Write-Host "?? Fetching from: $openApiUrl" -ForegroundColor Yellow

try {
    # Download the OpenAPI spec
    if ($SkipCertificateCheck) {
        $response = Invoke-WebRequest -Uri $openApiUrl -SkipCertificateCheck
    } else {
        $response = Invoke-WebRequest -Uri $openApiUrl
    }

    # Save to file
    $response.Content | Out-File -FilePath $OutputFile -Encoding UTF8

    Write-Host ""
    Write-Host "? Success! OpenAPI spec exported to: $OutputFile" -ForegroundColor Green
    Write-Host ""
    Write-Host "?? File Info:" -ForegroundColor Cyan
    $fileInfo = Get-Item $OutputFile
    Write-Host "   Size: $([math]::Round($fileInfo.Length / 1KB, 2)) KB" -ForegroundColor White
    Write-Host "   Path: $($fileInfo.FullName)" -ForegroundColor White
    Write-Host ""
    Write-Host "?? Next Steps for Frontend Teams:" -ForegroundColor Cyan
    Write-Host "   1. Generate TypeScript client:" -ForegroundColor White
    Write-Host "      npx @openapitools/openapi-generator-cli generate -i $OutputFile -g typescript-fetch -o ./src/api" -ForegroundColor Gray
    Write-Host ""
    Write-Host "   2. Generate C# client:" -ForegroundColor White
    Write-Host "      dotnet tool install -g NSwag.ConsoleCore" -ForegroundColor Gray
    Write-Host "      nswag openapi2csclient /input:$OutputFile /output:ApiClient.cs" -ForegroundColor Gray
    Write-Host ""
    Write-Host "   3. Generate Python client:" -ForegroundColor White
    Write-Host "      pip install openapi-generator-cli" -ForegroundColor Gray
    Write-Host "      openapi-generator-cli generate -i $OutputFile -g python -o ./api_client" -ForegroundColor Gray
    Write-Host ""

} catch {
    Write-Host ""
    Write-Host "? Error: Failed to fetch OpenAPI spec" -ForegroundColor Red
    Write-Host "   Message: $($_.Exception.Message)" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "?? Troubleshooting:" -ForegroundColor Cyan
    Write-Host "   1. Ensure the API is running: $ApiUrl" -ForegroundColor White
    Write-Host "   2. Check if you're in Development mode (OpenAPI only exposed in dev)" -ForegroundColor White
    Write-Host "   3. Verify the version number: v$Version" -ForegroundColor White
    Write-Host "   4. Try accessing manually: $openApiUrl" -ForegroundColor White
    Write-Host ""
    exit 1
}
