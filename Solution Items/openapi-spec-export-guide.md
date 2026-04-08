# ?? **OPENAPI SPECIFICATION EXPORT - COMPLETE GUIDE**

**Date:** January 2025  
**Status:** ? **IMPLEMENTED!**

---

## ? **What Was Implemented**

### **OpenAPI Spec Export Endpoint**
- ? Accessible at: `/openapi/v{version}.json`
- ? Example: `https://localhost:5001/openapi/v1.json`
- ? Returns: JSON OpenAPI 3.0 specification
- ? Available in: Development environment (configurable for production)
- ? Authentication: Anonymous (can be secured if needed)

### **Export Scripts**
- ? PowerShell script: `Scripts/Export-OpenApiSpec.ps1`
- ? Bash script: `Scripts/export-openapi-spec.sh`
- ? Cross-platform support (Windows, Linux, macOS)

---

## ?? **How to Export OpenAPI Spec**

### **Method 1: Direct Browser Access (Easiest)**

1. Start your API in Development mode
2. Open browser: `https://localhost:5001/openapi/v1.json`
3. Save the JSON file

**That's it!** ?

---

### **Method 2: PowerShell Script (Windows)**

```powershell
# Navigate to solution root
cd "C:\path\to\your\solution"

# Run export script
.\Scripts\Export-OpenApiSpec.ps1

# Custom options
.\Scripts\Export-OpenApiSpec.ps1 `
    -ApiUrl "https://localhost:5001" `
    -Version "1" `
    -OutputFile "my-api-spec.json"
```

**Output:**
```
?? Exporting OpenAPI Specification...

?? Fetching from: https://localhost:5001/openapi/v1.json

? Success! OpenAPI spec exported to: openapi-spec.json

?? File Info:
   Size: 45.23 KB
   Path: C:\path\to\solution\openapi-spec.json

?? Next Steps for Frontend Teams:
   1. Generate TypeScript client...
   2. Generate C# client...
   3. Generate Python client...
```

---

### **Method 3: Bash Script (Linux/macOS)**

```bash
# Navigate to solution root
cd /path/to/your/solution

# Make script executable (first time only)
chmod +x Scripts/export-openapi-spec.sh

# Run export script
./Scripts/export-openapi-spec.sh

# Custom options
API_URL="https://localhost:5001" VERSION="1" OUTPUT_FILE="my-api-spec.json" ./Scripts/export-openapi-spec.sh
```

---

### **Method 4: cURL (Any Platform)**

```bash
# Download spec
curl https://localhost:5001/openapi/v1.json -o openapi-spec.json -k

# Pretty print with jq
curl https://localhost:5001/openapi/v1.json -k | jq . > openapi-spec.json
```

---

### **Method 5: wget**

```bash
wget --no-check-certificate https://localhost:5001/openapi/v1.json -O openapi-spec.json
```

---

## ?? **Generate Client SDKs**

### **TypeScript Client (Frontend)**

**Using OpenAPI Generator:**
```bash
# Install generator
npm install -g @openapitools/openapi-generator-cli

# Generate TypeScript Fetch client
openapi-generator-cli generate \
  -i openapi-spec.json \
  -g typescript-fetch \
  -o ./src/api

# Or TypeScript Axios client
openapi-generator-cli generate \
  -i openapi-spec.json \
  -g typescript-axios \
  -o ./src/api
```

**Using NSwagStudio:**
```bash
# Install NSwag
npm install -g nswag

# Generate TypeScript client
nswag run nswag.json
```

**Example nswag.json:**
```json
{
  "runtime": "Net80",
  "defaultVariables": null,
  "documentGenerator": {
    "fromDocument": {
      "json": "openapi-spec.json"
    }
  },
  "codeGenerators": {
    "openApiToTypeScriptClient": {
      "className": "ApiClient",
      "moduleName": "",
      "namespace": "",
      "typeScriptVersion": 5.0,
      "template": "Fetch",
      "promiseType": "Promise",
      "dateTimeType": "Date",
      "output": "./src/api/api-client.ts"
    }
  }
}
```

---

### **C# Client (Backend/Desktop)**

**Using NSwag:**
```bash
# Install NSwag CLI
dotnet tool install -g NSwag.ConsoleCore

# Generate C# client
nswag openapi2csclient \
  /input:openapi-spec.json \
  /output:ApiClient.cs \
  /namespace:MyApp.ApiClient \
  /className:TasheerApiClient
```

**Using Kiota (Microsoft):**
```bash
# Install Kiota
dotnet tool install -g Microsoft.OpenApi.Kiota

# Generate C# client
kiota generate \
  -l csharp \
  -d openapi-spec.json \
  -c TasheerApiClient \
  -n MyApp.ApiClient \
  -o ./Generated
```

---

### **Python Client**

**Using OpenAPI Generator:**
```bash
# Install generator
pip install openapi-generator-cli

# Generate Python client
openapi-generator-cli generate \
  -i openapi-spec.json \
  -g python \
  -o ./api_client \
  --package-name tasheer_api_client
```

---

### **Java Client**

**Using OpenAPI Generator:**
```bash
openapi-generator-cli generate \
  -i openapi-spec.json \
  -g java \
  -o ./java-client \
  --library okhttp-gson
```

---

### **JavaScript/Node.js Client**

**Using OpenAPI Generator:**
```bash
openapi-generator-cli generate \
  -i openapi-spec.json \
  -g javascript \
  -o ./js-client
```

---

## ?? **Frontend Team Workflow**

### **Step 1: Export Spec**

```bash
# Backend team exports spec
./Scripts/export-openapi-spec.sh
```

### **Step 2: Share Spec**

**Option A: Commit to repo**
```bash
git add openapi-spec.json
git commit -m "Update OpenAPI spec"
git push
```

**Option B: Upload to shared location**
- Confluence/Wiki
- SharePoint
- S3 bucket
- Azure Blob Storage

**Option C: Publish to API gateway**
- Azure API Management
- AWS API Gateway
- Kong

### **Step 3: Generate Client**

```bash
# Frontend team generates client
npm run generate-api-client
```

**Example package.json script:**
```json
{
  "scripts": {
    "generate-api-client": "openapi-generator-cli generate -i openapi-spec.json -g typescript-fetch -o ./src/api"
  }
}
```

---

## ?? **Configuration Options**

### **Enable in Production (Optional)**

By default, OpenAPI is only exposed in Development. To enable in production:

**Program.cs:**
```csharp
// BEFORE (Development only)
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// AFTER (Always available, but secured)
app.MapOpenApi().RequireAuthorization(); // Require authentication

// OR (Configuration-driven)
if (app.Configuration.GetValue<bool>("OpenApi:Enabled"))
{
    app.MapOpenApi();
}
```

**appsettings.Production.json:**
```json
{
  "OpenApi": {
    "Enabled": true
  }
}
```

---

### **Add API Key Protection**

```csharp
app.MapOpenApi().RequireApiKey(); // Custom extension method
```

---

### **Custom Export Path**

```csharp
app.MapOpenApi("/swagger/v{documentName}/openapi.json");
// Access at: /swagger/v1/openapi.json
```

---

## ?? **What the Spec Includes**

### **Endpoints:**
- All API routes (`/v1/order`, etc.)
- HTTP methods (GET, POST, PUT, DELETE)
- Request/response schemas
- Status codes
- Examples

### **Models:**
- Request DTOs
- Response DTOs
- Validation rules
- Data types

### **Security:**
- Authentication schemes (Bearer JWT, API Key)
- Authorization requirements
- OAuth2 flows (if configured)

### **Metadata:**
- API title, version, description
- Contact information
- License information
- Tags and groupings

---

## ?? **Best Practices**

### **1. Version Your Spec**

```bash
# Export with version in filename
curl https://localhost:5001/openapi/v1.json -o openapi-v1.0.0.json -k

# Keep history
openapi-v1.0.0.json
openapi-v1.1.0.json
openapi-v2.0.0.json
```

### **2. Automate Export in CI/CD**

**Azure Pipelines:**
```yaml
- task: PowerShell@2
  displayName: 'Export OpenAPI Spec'
  inputs:
    filePath: 'Scripts/Export-OpenApiSpec.ps1'
    arguments: '-ApiUrl $(ApiUrl) -OutputFile $(Build.ArtifactStagingDirectory)/openapi-spec.json'
```

### **3. Validate Spec**

```bash
# Install validator
npm install -g @stoplight/spectral-cli

# Validate spec
spectral lint openapi-spec.json
```

### **4. Generate Docs**

**Using Redoc:**
```bash
npx redoc-cli bundle openapi-spec.json -o api-docs.html
```

**Using Swagger UI:**
```bash
npx swagger-ui-watcher openapi-spec.json
```

---

## ?? **Troubleshooting**

### **Problem: 404 Not Found**

**Cause:** API not running or not in Development mode

**Fix:**
```bash
# Ensure API is running
dotnet run --project Application/Services/Tasheer.Api

# Check environment
echo $ASPNETCORE_ENVIRONMENT  # Should be "Development"
```

---

### **Problem: SSL Certificate Error**

**Cause:** Self-signed certificate in development

**Fix:**
```bash
# PowerShell
.\Scripts\Export-OpenApiSpec.ps1 -SkipCertificateCheck

# curl
curl -k https://localhost:5001/openapi/v1.json -o spec.json

# wget
wget --no-check-certificate https://localhost:5001/openapi/v1.json
```

---

### **Problem: Empty or Invalid JSON**

**Cause:** Wrong version number or API not fully started

**Fix:**
```bash
# Check available versions
curl https://localhost:5001/openapi/ -k

# Try different version
curl https://localhost:5001/openapi/v1.json -k
```

---

## ?? **Monitoring & Analytics**

### **Track Downloads**

Add telemetry to the OpenAPI endpoint:

```csharp
app.MapOpenApi().WithOpenApi(operation =>
{
    operation.Summary = "OpenAPI Specification (v1)";
    operation.Description = "Download the OpenAPI spec for client generation";
    return operation;
});

// Log downloads
app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/openapi"))
    {
        logger.LogInformation("OpenAPI spec downloaded from {IP}", 
            context.Connection.RemoteIpAddress);
    }
    await next();
});
```

---

## ?? **Summary**

### **? What You Now Have:**

1. ? **OpenAPI Endpoint:** `/openapi/v1.json`
2. ? **PowerShell Script:** `Scripts/Export-OpenApiSpec.ps1`
3. ? **Bash Script:** `Scripts/export-openapi-spec.sh`
4. ? **Documentation:** This guide
5. ? **Multiple export methods:** Browser, scripts, curl, wget

### **? What Frontend Teams Can Do:**

1. ? Export spec in seconds
2. ? Generate TypeScript/C#/Python/Java clients
3. ? Keep clients in sync with API
4. ? Automate client generation
5. ? Reduce integration errors

### **? Benefits:**

- ? **Faster integration** - Auto-generated clients
- ?? **Always in sync** - Export latest spec anytime
- ?? **Self-documenting** - Spec matches code
- ??? **Type-safe** - Strongly-typed clients
- ?? **Developer experience** - Easy to use

---

**OpenAPI Spec Export is now production-ready!** ??

---

_Implemented: January 2025_
_Location: `/openapi/v1.json`_
_Scripts: `Scripts/Export-OpenApiSpec.ps1`, `Scripts/export-openapi-spec.sh`_
