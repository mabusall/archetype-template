# Elastic Stack Post-Installation Setup Guide

Since you've just spun up a fresh installation of the Elastic Stack (Elasticsearch, Kibana, APM) via Docker, your backend is now successfully transmitting data, but Kibana doesn't yet know how to display it. 

Follow these steps in Kibana (http://localhost:5601) to wire everything up:

## 1. Create a Data View for Serilog Logs
Your backend uses Serilog to stream logs directly to Elasticsearch using modern Data Streams. The logs are being sent to `logs-crs.v2.api-development`. We need to tell Kibana to look for them.

1. In Kibana, open the **Hamburger Menu** (top-left) and scroll down to **Stack Management** (under Management).
2. Click on **Data Views** (formerly Index Patterns) in the left sidebar.
3. Click the **Create data view** button.
4. **Name:** `Application Logs`
5. **Index pattern:** `logs-*` (This will match your API logs and any future microservices).
6. **Timestamp field:** Select `@timestamp`.
7. Click **Save data view to Kibana**.

*You can now view your beautifully structured logs by navigating to **Discover** or **Observability -> Logs**.*

## 2. Verify Elastic APM
Your .NET application is already successfully pushing traces to the APM server container on port 8201. 

1. Open the **Hamburger Menu** and navigate to **Observability -> APM -> Services**.
2. You should see `Crs_v2_Api` listed under the services. 
3. Click on it to see your HTTP request traces, database query execution times, and dependency calls (like HTTP calls to Nusuk/CRM).

> [!TIP] If APM Prompts for Setup
> If the APM page asks you to "Add APM integration", you can simply click **"I have an existing APM Server"** or bypass the setup, because your standalone `elastic-apm` Docker container is already receiving the data and writing it directly to Elasticsearch.

## 3. Link Logs to APM Traces (Optional but Recommended)
To allow you to click on an APM trace and immediately see the logs generated *during* that exact request:

1. Go back to **Stack Management -> Advanced Settings**.
2. Search for the setting `observability:logsIndex` (or `apm_oss:logsIndices`).
3. Ensure it includes `logs-*` (which matches the Data View we created in Step 1). 
4. Save the settings.

Now your Logs and APM Traces are fully correlated!
