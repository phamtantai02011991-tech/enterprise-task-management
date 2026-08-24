# Project Setup Guide

## Requirements

- .NET SDK 10.0
- SQL Server
- Visual Studio 2022 or later (recommended)

---

## Database Configuration

This project uses **SQL Server with Windows Authentication**.  
No username or password is required.

The connection string is configured in:

```
appsettings.json
```

Example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=MiniProjectDb;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

### Notes

- `Server=.` means the project connects to the default local SQL Server instance.
- If you are using SQL Server Express, update the connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=MiniProjectDb;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

- Each team member must have SQL Server installed and access through Windows Authentication.

---

## Database Migration

After cloning the repository:

### Using Visual Studio Package Manager Console

Run:

```powershell
Update-Database
```

### Or using .NET CLI

Run:

```bash
dotnet ef database update
```

The database will be created automatically.

---

## File Upload Configuration

Uploaded files are stored in:

```
wwwroot/uploads/
```

This folder is ignored by Git and is not included in the repository.

If the folder does not exist after cloning the project, create it manually:

```
wwwroot/uploads
```

---

## Run the Project

Using Visual Studio:

```
F5
```

or using .NET CLI:

```bash
dotnet run
```

---

## Git Ignore Rules

The following files/folders are excluded from Git:

- Build output:
  - `bin/`
  - `obj/`

- Visual Studio cache files:
  - `.vs/`

- IDE configuration files:
  - `.vscode/`
  - `.idea/`

- Log files:
  - `*.log`

- Local database files:
  - `*.mdf`
  - `*.ldf`

- Uploaded files:
  - `wwwroot/uploads/`

---

## Team Development Notes

- Database schema has been finalized before development.
- Do not modify database structure or entities without discussing with the team.
- Each role is developed separately using ASP.NET Core Areas:

```
Areas
├── Admin
├── Manager
└── Employee
```

- Reuse existing services when possible.
- Avoid deleting or changing other members' code without discussion.