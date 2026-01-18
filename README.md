# Tasks CRUD API
## Requirements
- .NET 10 SDK
- SQL Server Express

### 1. Clone the repo
git clone https://github.com/RomanDenis/TasksApi.git
cd TasksApi


### 2. Configure db connection
change `appsettings.json` if needed:

{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=TasksApiDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
}


### 3. Run migrations

dotnet ef database update


### 4. Launch the application

dotnet run