# Database Migration Commands

## OutBoxDbContext (MassTransit + RabbitMQ)

### Add New Migration
```powershell
Add-Migration OutBox_Init -OutputDir MessageBus/Migrations -Context OutBoxDbContext -StartupProject Sample.Api -Project Archetype.Infrastructure -Verbose
```

### Update Database
```powershell
Update-Database -Context OutBoxDbContext -StartupProject Sample.Api -Project Archetype.Infrastructure -Verbose
```

### Remove Last Migration
```powershell
Remove-Migration -Context OutBoxDbContext -StartupProject Sample.Api -Project Archetype.Infrastructure
```

### Generate SQL Script
```powershell
Script-Migration -Context OutBoxDbContext -StartupProject Sample.Api -Project Archetype.Infrastructure -Output "OutBox_Migration.sql"
```

### Rollback to Specific Migration
```powershell
Update-Database -Context OutBoxDbContext -Migration <MigrationName> -StartupProject Sample.Api -Project Archetype.Infrastructure -Verbose
```

### List All Migrations
```powershell
Get-Migration -Context OutBoxDbContext -StartupProject Sample.Api -Project Archetype.Infrastructure
```

---

## SampleDbContext (Application Database)

### Add New Migration
```powershell
Add-Migration SampleDb_Init -OutputDir Repository/Migrations -Context SampleDbContext -StartupProject Sample.Api -Project Sample.Core.Api -Verbose
```

### Update Database
```powershell
Update-Database -Context SampleDbContext -StartupProject Sample.Api -Project Sample.Core.Api -Verbose
```

### Remove Last Migration
```powershell
Remove-Migration -Context SampleDbContext -StartupProject Sample.Api -Project Sample.Core.Api
```

### Generate SQL Script
```powershell
Script-Migration -Context SampleDbContext -StartupProject Sample.Api -Project Sample.Core.Api -Output "SampleDb_Migration.sql"
```

### Rollback to Specific Migration
```powershell
Update-Database -Context SampleDbContext -Migration <MigrationName> -StartupProject Sample.Api -Project Sample.Core.Api -Verbose
```

### List All Migrations
```powershell
Get-Migration -Context SampleDbContext -StartupProject Sample.Api -Project Sample.Core.Api -Verbose
```

### Drop Database
```powershell
Drop-Database -Context SampleDbContext -StartupProject Sample.Api -Project Sample.Core.Api -Verbose
```
