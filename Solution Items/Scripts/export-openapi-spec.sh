#!/bin/bash
# Export OpenAPI Specification
# This script downloads the OpenAPI spec from your running API
# Frontend teams can use this to generate client SDKs

# Default values
API_URL="${API_URL:-https://localhost:5001}"
VERSION="${VERSION:-1}"
OUTPUT_FILE="${OUTPUT_FILE:-openapi-spec.json}"

# Colors
CYAN='\033[0;36m'
YELLOW='\033[1;33m'
GREEN='\033[0;32m'
RED='\033[0;31m'
WHITE='\033[1;37m'
GRAY='\033[0;37m'
NC='\033[0m' # No Color

echo -e "${CYAN}?? Exporting OpenAPI Specification...${NC}"
echo ""

# Construct the OpenAPI endpoint URL
OPENAPI_URL="$API_URL/openapi/v$VERSION.json"

echo -e "${YELLOW}?? Fetching from: $OPENAPI_URL${NC}"

# Download the OpenAPI spec
if curl -k -s -o "$OUTPUT_FILE" -w "%{http_code}" "$OPENAPI_URL" | grep -q "200"; then
    echo ""
    echo -e "${GREEN}? Success! OpenAPI spec exported to: $OUTPUT_FILE${NC}"
    echo ""
    echo -e "${CYAN}?? File Info:${NC}"
    FILE_SIZE=$(du -h "$OUTPUT_FILE" | cut -f1)
    FILE_PATH=$(realpath "$OUTPUT_FILE")
    echo -e "${WHITE}   Size: $FILE_SIZE${NC}"
    echo -e "${WHITE}   Path: $FILE_PATH${NC}"
    echo ""
    echo -e "${CYAN}?? Next Steps for Frontend Teams:${NC}"
    echo -e "${WHITE}   1. Generate TypeScript client:${NC}"
    echo -e "${GRAY}      npx @openapitools/openapi-generator-cli generate -i $OUTPUT_FILE -g typescript-fetch -o ./src/api${NC}"
    echo ""
    echo -e "${WHITE}   2. Generate C# client:${NC}"
    echo -e "${GRAY}      dotnet tool install -g NSwag.ConsoleCore${NC}"
    echo -e "${GRAY}      nswag openapi2csclient /input:$OUTPUT_FILE /output:ApiClient.cs${NC}"
    echo ""
    echo -e "${WHITE}   3. Generate Python client:${NC}"
    echo -e "${GRAY}      pip install openapi-generator-cli${NC}"
    echo -e "${GRAY}      openapi-generator-cli generate -i $OUTPUT_FILE -g python -o ./api_client${NC}"
    echo ""
else
    echo ""
    echo -e "${RED}? Error: Failed to fetch OpenAPI spec${NC}"
    echo ""
    echo -e "${CYAN}?? Troubleshooting:${NC}"
    echo -e "${WHITE}   1. Ensure the API is running: $API_URL${NC}"
    echo -e "${WHITE}   2. Check if you're in Development mode (OpenAPI only exposed in dev)${NC}"
    echo -e "${WHITE}   3. Verify the version number: v$VERSION${NC}"
    echo -e "${WHITE}   4. Try accessing manually: $OPENAPI_URL${NC}"
    echo ""
    exit 1
fi
