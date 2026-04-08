# Database Migration Commands

## OutBoxDbContext (MassTransit + RabbitMQ)

### Add New Migration
```powershell
Add-Migration OutBox_Init -OutputDir MessageBus/Migrations -Context OutBoxDbContext -StartupProject Tasheer.Api -Project Tasheer.Infrastructure -Verbose
```

### Update Database
```powershell
Update-Database -Context OutBoxDbContext -StartupProject Tasheer.Api -Project Tasheer.Infrastructure -Verbose
```

### Remove Last Migration
```powershell
Remove-Migration -Context OutBoxDbContext -StartupProject Tasheer.Api -Project Tasheer.Infrastructure
```

### Generate SQL Script
```powershell
Script-Migration -Context OutBoxDbContext -StartupProject Tasheer.Api -Project Tasheer.Infrastructure -Output "OutBox_Migration.sql"
```

### Rollback to Specific Migration
```powershell
Update-Database -Context OutBoxDbContext -Migration <MigrationName> -StartupProject Tasheer.Api -Project Tasheer.Infrastructure -Verbose
```

### List All Migrations
```powershell
Get-Migration -Context OutBoxDbContext -StartupProject Tasheer.Api -Project Tasheer.Infrastructure
```

---

## SampleDbContext (Application Database)

### Add New Migration
```powershell
Add-Migration SampleDb_Init -OutputDir Repository/Migrations -Context SampleDbContext -StartupProject Tasheer.Api -Project Tasheer.Core.Api -Verbose
```

### Update Database
```powershell
Update-Database -Context SampleDbContext -StartupProject Tasheer.Api -Project Tasheer.Core.Api -Verbose
```

### Remove Last Migration
```powershell
Remove-Migration -Context SampleDbContext -StartupProject Tasheer.Api -Project Tasheer.Core.Api
```

### Generate SQL Script
```powershell
Script-Migration -Context SampleDbContext -StartupProject Tasheer.Api -Project Tasheer.Core.Api -Output "SampleDb_Migration.sql"
```

### Rollback to Specific Migration
```powershell
Update-Database -Context SampleDbContext -Migration <MigrationName> -StartupProject Tasheer.Api -Project Tasheer.Core.Api -Verbose
```

### List All Migrations
```powershell
Get-Migration -Context SampleDbContext -StartupProject Tasheer.Api -Project Tasheer.Core.Api -Verbose
```

### Drop Database
```powershell
Drop-Database -Context SampleDbContext -StartupProject Tasheer.Api -Project Tasheer.Core.Api -Verbose
```
