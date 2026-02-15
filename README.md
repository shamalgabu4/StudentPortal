# StudentPortal

ASP.NET Core MVC sample application with Identity, EF Core (Code First), SQL Server and Bootstrap 5.

Quick start

1. Ensure .NET 8 SDK is installed.
2. From the `StudentPortal` folder run:

```powershell
dotnet restore
dotnet ef migrations add InitialCreate
dotnet ef database update
dotnet run
```

Default admin seeded: `admin@studentportal.local` / `Admin@123`

Notes
- Update the connection string in `appsettings.json` to point to your SQL Server if not using LocalDB.
