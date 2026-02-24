# Visit 'Em All

## Quick Start
First, clone this repository. Then:

- Install the .NET Entity Framework CLI
    * `dotnet tool install --global dotnet-ef`
- Create the database/s
    * `createdb visitemall_csharp_development`
    * `createdb visitemall_csharp_test`
- Run the migration to create the tables
    * `cd` into `/VisitEmAll`
  * `dotnet ef database update`
  * `DATABASE_NAME=visitemall_csharp_development dotnet ef database update`
- Start the application, with the development database
  * `DATABASE_NAME=visitemall_csharp_development dotnet watch run`
- Go to `http://localhost:5287/`

## Running Tests

- Start the application, with the default (test) database
    * `dotnet watch run`
- Open a second terminal session and run the tests (in the root)
    * `dotnet test`

## Updating the Database

Changes are applied to the database programatically, using files called _migrations_, which live in the `/Migrations` directory.        
The process is as follows...

- To create a new table
  * First, create the model
  * Then go to VisitEmAllDbContext
  * And add this `public DbSet<MODEL_NAME>? MODEL_NAME { get; set; }` 
- Generate the migration file
  * `cd` into `/VisitEmAll`
  * Decide what you want to call the migration file
  * > Note: It's best to use good descriptive names
  * Then do `dotnet ef migrations add ` followed by the name you chose
- Run the migration
  * `dotnet ef database update`

## Local Configuration

### Setting Up `appsettings.Development.json`
> Note: This is your secrets file and must always be in `.gitignore`

In removing the secrets, there is now a small config step that's need to connect to your database.

- Add `appsettings.Development.json` to `/VisitEmAll` from the project root
    * `touch VisitEmAll/appsettings.Development.json` 
- Add the `"ConnectionStrings"` to `appsettings.Development.json`.      
    * It should look like this:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Username=<YOUR USERNAME>;Password=1234;Database=visitemall_csharp_test"
  }
}
```
> Note: If you're unsure what your username should be, type `whoami` into the terminal and copy & paste that in place of `<YOUR USERNAME>`
> Note: There is an `example.appsettings.Development.json` in `VisitEmAll/` that should **not be deleted** and contains this infromation too

## BaseTest Classes & Test Configuration

For the new configuration to work, you will need to create a `VisitEmAll.Test/appsettings.Test.json` using the `example.appsettings.Development.json` as a guide.  
```bash
touch VisitEmAll.Test/appsettings.Test.json
```

For new tests the TestBase classes can be inherited.

Playwright tests will inherit from `PlaywrightTestBase`, which is in turn inheriting from `PageTest`.

NUnit tests will inherit from `NUnitTestBase`.

Both TestBase classes are handling the setup and teardown for each and every test, ensuring the test database stays clean.