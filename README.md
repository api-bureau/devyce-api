# Devyce API Client

A lightweight C# client library and CLI tool for rapid prototyping and onboarding with the Devyce API. Designed to speed up development and exploration of Devyce's communication platform capabilities.

## 📦 What's Included

- **ApiBureau.Devyce.Api**: .NET client library for the Devyce API
- **ApiBureau.Devyce.Api.Console**: Command-line interface for exploring and testing the API

## 🚀 Features

- Type-safe C# client with full async/await support
- Organized endpoint groups (Calls, Users, Contacts, Transcripts, CRM Sync)
- CLI tool with multiple output formats (text and JSON)
- Built-in authentication handling
- Comprehensive logging with Serilog
- .NET 10 support

---

## 📚 API Client Library

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/api-bureau/devyce-api.git
   ```

1. Add the reference to your project

1. Add this package `Microsoft.Extensions.Hosting` to your project

1. Example project

   ```csharp
   using ApiBureau.Devyce.Api.Extensions;
   using ApiBureau.Devyce.Api.Interfaces;
   using ApiBureau.Devyce.Api.Queries;
   using Microsoft.Extensions.Configuration;
   using Microsoft.Extensions.DependencyInjection;
   using Microsoft.Extensions.Hosting;
   using System.Text.Json;

   var builder = Host.CreateApplicationBuilder(args);
   builder.Configuration.AddInMemoryCollection(
   [
       new ("DevyceSettings:BaseUrl", ""),
       new ("DevyceSettings:ApiKey", ""),
       new ("DevyceSettings:OrganizationId", "")
   ]);
   builder.Services.AddDevyce(builder.Configuration);

   var serviceProvider = builder.Services.BuildServiceProvider();
   var client = serviceProvider.GetRequiredService<IDevyceClient>();

   // Fetch users
   var users = await client.Users.GetAsync();
   Console.WriteLine(JsonSerializer.Serialize(users));

   // Fetch recent calls
   var callQuery = new CallQuery(DateTime.Now.AddHours(-1), DateTime.Now);
   var calls = await client.Calls.GetAsync(callQuery);

   Console.WriteLine(JsonSerializer.Serialize(calls));
   ```

### Available Endpoints

- Users Endpoint
  - Get all users: `await client.Users.GetAsync();`
- Calls Endpoint
  - Access call records and call history
  ```csharp
  // Get calls within a date range
  var query = new CallQuery(startDate, endDate);
  var calls = await client.Calls.GetAsync(query);
  ```
- Contacts Endpoint
  - Manage contact information.
  ```csharp
  // Get all contact IDs
  var contactIds = await client.Contacts.GetContactIdsAsync(
      organizationId, 
      cancellationToken);
  
  // Get specific contact details
  var contact = await client.Contacts.GetContactAsync(
      organizationId, 
      contactId, 
      cancellationToken);
  ```
- Transcripts Endpoint
  - Access call transcriptions (requires additional API permissions).
  ```csharp
  // Get call transcript
  Cvar transcript = await client.Transcripts.GetAsync(
      callId, 
      cancellationToken);
  ```
- CRM Sync Details Endpoint
  - Retrieve CRM synchronization information for calls.
  ```csharp
  // Get CRM sync details for a call
  IList<CrmSyncDetailsDto> crmDetails = await client.CrmSyncDetails.GetAsync(
      callId, 
      cancellationToken);
  ```

## 🖥️ Command-Line Interface (CLI)

The CLI tool provides an interactive way to explore the Devyce API without writing code.

### Setup

1. **Clone the repository**:
   ```bash
   git clone https://github.com/api-bureau/devyce-api.git
   cd devyce-api/src/ApiBureau.Devyce.Api.Console
   ```

2. **Configure settings**:
   
   Edit `appsettings.json` or use User Secrets:
   
   ```json
   {
     "DevyceSettings": {
       "BaseUrl": "base-url",
       "ApiKey": "your-api-key-here",
       "OrganizationId": "your-organization-id"
     }
   }
   ```
   
   **Using User Secrets (recommended for development):**
   ```bash
   dotnet user-secrets set "DevyceSettings:BaseUrl" "base-url"
   dotnet user-secrets set "DevyceSettings:ApiKey" "your-api-key"
   dotnet user-secrets set "DevyceSettings:OrganizationId" "your-org-id"
   ```

3. **Build and run**:
   ```bash
   dotnet build
   dotnet run
   ```

### CLI Commands

All commands support two output formats:
- **Text** (default): Human-readable formatted output
- **JSON**: Machine-readable JSON output

#### Global Options

- `--format`, `-f` - Output format (`Text` or `Json`, default: `Text`)
- `--last-minutes`, `-m` - Time range in minutes for call-related commands (default: `120`)

### 📋 Command Reference

#### 1. List Users

Display all Devyce users with their details.

**Syntax:**
```bash
dotnet run -- users [options]
```

**Options:**
- `--format`, `-f` - Output format (`Text` or `Json`)

**Examples:**
```bash
# List users in text format
dotnet run -- users

# List users in JSON format
dotnet run -- users --format json
dotnet run -- users -f json
```

**Sample Output (Text):**
```
=== Devyce Users ===
Active | John Smith | john.smith@company.com
Active | Jane Doe | jane.doe@company.com
Inactive | Bob Johnson | bob.johnson@company.com
Total users: 3
```

#### 2. List Calls

Display call records within a specified time range.

**Syntax:**
```bash
dotnet run -- calls [options]
```

**Options:**
- `--format`, `-f` - Output format (`Text` or `Json`)
- `--last-minutes`, `-m` - Number of minutes to look back (default: `120`)

**Examples:**
```bash
# List calls from the last 2 hours (default)
dotnet run -- calls

# List calls from the last 60 minutes in text format
dotnet run -- calls --last-minutes 60
dotnet run -- calls -m 60

# List calls from the last hour in JSON format
dotnet run -- calls --last-minutes 60 --format json
dotnet run -- calls -m 60 -f json

# List calls from the last 24 hours
dotnet run -- calls --last-minutes 1440
```

**Sample Output (Text):**
```
=== Devyce Calls (Last 120 minutes) ===
2024-01-15 14:23:45 | Duration: 180s | From: +441234567890 | To: +447987654321 | Id: call-abc123
2024-01-15 14:45:12 | Duration: 95s | From: +441234567891 | To: +447987654322 | Id: call-def456
Total calls: 2
```

#### 3. List CRM Details

Display CRM synchronization details for calls within a specified time range.

**Syntax:**
```bash
dotnet run -- crm-details [options]
```

**Options:**
- `--format`, `-f` - Output format (`Text` or `Json`)
- `--last-minutes`, `-m` - Number of minutes to look back (default: `120`)

**Examples:**
```bash
# List CRM details for calls from the last 2 hours
dotnet run -- crm-details

# List CRM details from the last 30 minutes in JSON format
dotnet run -- crm-details --last-minutes 30 --format json
dotnet run -- crm-details -m 30 -f json

# List CRM details from the last 4 hours in text format
dotnet run -- crm-details --last-minutes 240
```

**Sample Output (Text):**
```
=== Devyce CRM Sync Details (Last 120 minutes) ===
2024-01-15 14:23:45 | Duration: 180s | From: +441234567890 | To: +447987654321 | CRM: Salesforce:Contact:003xx000
2024-01-15 14:45:12 | Duration: 95s | From: +441234567891 | To: +447987654322 | CRM: No CRM data
Total calls: 2 | CRM details found: 1
```

#### 4. Get Call Transcript

Fetch and display the transcript for a specific call by its ID.

> **Note:** Requires additional API permissions. Contact Devyce if transcripts are not available.

**Syntax:**
```bash
dotnet run -- transcripts --call-id <call-id>
```

**Options:**
- `--call-id`, `-i` - **Required**. The call ID to retrieve transcript for

**Examples:**
```bash
# Get transcript for a specific call
dotnet run -- transcripts --call-id call-abc123
dotnet run -- transcripts -i call-abc123

# Use with the calls command to get IDs
dotnet run -- calls -m 60
# Copy a call ID from the output, then:
dotnet run -- transcripts -i <copied-call-id>
```

**Sample Output:**
```
=== Transcript for Call call-abc123 ===
{
  "callId": "call-abc123",
  "transcript": "Hello, this is John. How can I help you today?...",
  "confidence": 0.95,
  "language": "en-GB"
}
```

### 🎯 Common Usage Scenarios

#### Scenario 1: Monitor Recent Call Activity
```bash
# Check calls from the last 30 minutes
dotnet run -- calls --last-minutes 30

# Get detailed CRM sync information for those calls
dotnet run -- crm-details --last-minutes 30
```

<!--
#### Scenario 2: Export Data for Analysis
```bash
# Export users to JSON
dotnet run -- users --format json > users.json

# Export call data for the last 24 hours
dotnet run -- calls --last-minutes 1440 --format json > calls.json

# Export CRM sync details
dotnet run -- crm-details --last-minutes 1440 --format json > crm-details.json
```
-->

#### Scenario 2: Review Call Transcripts
```bash
# First, get recent calls to find call IDs
dotnet run -- calls --last-minutes 60

# Then retrieve transcript for a specific call
dotnet run -- transcripts --call-id call-abc123
```

#### Scenario 3: Daily Reporting
```bash
# Get all activity from the last 24 hours with CRM context
dotnet run -- crm-details --last-minutes 1440 --format json
```

## 🔧 Configuration

### DevyceSettings

| Property | Description | Required |
|----------|-------------|----------|
| `BaseUrl` | The Devyce API base URL | Yes |
| `ApiKey` | Your Devyce API key | Yes |
| `OrganizationId` | Your organization identifier | Yes |

### Logging Configuration

The CLI uses Serilog for logging. Configure in `appsettings.json`:

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "outputTemplate": "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
        }
      },
      {
        "Name": "File",
        "Args": {
          "path": "logs/devyce-cli-.log",
          "rollingInterval": "Day"
        }
      }
    ]
  }
}
```

## 💡 Tips

- Check the `logs/` directory for detailed execution logs
- Use shorter time ranges (`--last-minutes`) for faster responses with large datasets

## 📖 Additional Resources

- [Report Issues](https://github.com/api-bureau/devyce-api/issues)

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.