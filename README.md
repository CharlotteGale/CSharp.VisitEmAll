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
  * And add this `public DbSet<MODEL NAME>? MODEL_NAME { get; set; }` 
- Generate the migration file
  * `cd` into `/VisitEmAll`
  * Decide what you want to call the migration file
  * > Note: It's best to use good descriptive names
  * Then do `dotnet ef migrations add ` followed by the name you chose
- Run the migration
  * `dotnet ef database update`