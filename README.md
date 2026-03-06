# PinPals
## The Social Travel Hub
PinPals (formerly VisitEmAll) is a fullstack web application for globetrotters who want to share their journeys and find inspiration through their social circle. Unlike a standard travel blog, PinPals focuses on the interactive "Pin" mechanic: "traveling" to a friend’s profile, liking their trips, and watching them automatically populate your own inspiration board.

### Key Features
- Bento Box Stats: A sleek, modern dashboard providing a high-level snapshot of your travel stats.

- Social Connectivity: Search for pals and "travel" to their profile pages.

- Holiday Inspo: Dynamic "Like" system handled via JS DOM manipulation that pins friend's trips to your banner.

- Full-Stack Power: Built with ASP.NET Core, Entity Framework, and Bootstrap 5.

### Project Structure
**/Controllers:** Handles the logic for Holidays, Dashboards, and Social connections.

**/Models:** Defines the data schema (Users, Holidays, Likes).

**/Views:** Razor pages styled with Bootstrap for dynamic content renderin


### Setup & Installation
**Prerequisites**   
- .NET 10.0 SDK
- PostgreSQL 

**Local Development**   
1. Clone the repository.

2. Install the Entity Framework CLI:

```Bash
dotnet tool install --global dotnet-ef
```
3. Initialize Databases:

```Bash
createdb visitemall_csharp_development
createdb visitemall_csharp_test
```
4. Run Migrations & Launch:

```Bash
cd VisitEmAll
DATABASE_NAME=visitemall_csharp_development dotnet ef database update
DATABASE_NAME=visitemall_csharp_development dotnet watch run
```
> *Access the app at: `http://localhost:5287/`*


### Testing & Database Seeding
**Automated Migrations**
> Note: The application includes a DbSeeder. The server automatically handles migrations and data seeding for the Test Database, ensuring a consistent state for every test run.

**Running Tests**
1. Start the app with the default (test) database: `dotnet watch run`

2. Open a second terminal and run: dotnet test

**Test Frameworks:**

- Playwright: (Inherits PlaywrightTestBase) for End-to-End browser testing.

- NUnit: (Inherits NUnitTestBase) for unit and integration logic.

- *Both bases handle Setup/Teardown to keep the DB clean.*


## Local Configuration

### Setting Up `appsettings.Development.json`
To connect to your local database, you must create an `appsettings.Development.json` in the `/VisitEmAll` directory (this file is git-ignored for security).

```json
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Username=<YOUR USERNAME>;Password=1234;Database=visitemall_csharp_test"
  }
```
> *Tip: Type `whoami` in your terminal to find your username.*

### Database Workflow (Migrations)
When adding new features or tables:

1. Create/Update the Model in `/Models`.

2. Register the DbSet in `VisitEmAllDbContext.cs`.

3. Generate Migration: `dotnet ef migrations add NameOfYourMigration`

4. Apply Changes: `dotnet ef database update`
