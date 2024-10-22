# RecipeApplication

## Purpose

An ASP.NET Core MVC Recipe Application where users can view, add, edit, and delete recipes. Recipe owners have full control over their submitted recipes, while general users can browse through all available recipes.

## Technologies Used

- ASP.NET Core MVC
- Entity Framework Core (EF Core)
- SQL Server
- Bootstrap (for front-end design)

## Features

- **Browse Recipes**: All users can browse recipes posted by others.
- **Add Recipes**: Authenticated users can submit their own recipes.
- **Edit/Delete Recipes**: Recipe owners can edit or delete their recipes.
- **View Recipe Details**: Detailed view of each recipe with instructions and ingredients.

## Prerequisites

- Visual Studio 2022 or later
- SQL Server
- .NET Core 6.0 SDK or later

## Steps to Run the Project

### 1. Clone the Repository

- Open Command Prompt and navigate to the directory where you want to clone the project.
- Run the following command:

   `git clone https://github.com/your-username/RecipeApplication.git`

### 2. Open the Project in Visual Studio

- Navigate to the directory where the project was cloned.
- Locate the `RecipeApplication.sln` file and double-click it to open the project in Visual Studio.

### 3. Update the Connection String

- Open the `appsettings.json` file located in the root of the project.
- Update the connection string with your SQL Server details:

 "ConnectionStrings": { "DefaultConnection": "data source=your_server_name;initial catalog=RecipeDB;integrated security=true;encrypt=false" }


### 4. Delete Existing Migrations

- If there is a `Migrations` folder in the project, delete it to start fresh.

### 5. Create and Apply Migrations

- Open **Tools > NuGet Package Manager > Package Manager Console** in Visual Studio.
- Run the following commands:

`add-migration InitialCreate`

`update-database`

### 6. Run the Project

- Press  **Run** button in Visual Studio to start the project.

## Project Structure

- **Controllers**: Handles request routing and business logic.
- **Models**: Defines the structure of the Recipe entity and other domain models.
- **Views**: Contains the Razor views that render HTML pages.
- **wwwroot**: Stores static files like CSS, JavaScript, and images.
