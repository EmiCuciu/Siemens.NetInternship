# Library Management System

### Prerequisites
- .NET 8.0 SDK
- Visual Studio 2022 or VS Code (recommended)

### Setup Instructions
1. Clone the repository:
   ```bash
   git clone https://github.com/EmiCuciu/Siemens.NetInternship.git
   cd LibraryManagement

2. Resore dependencies
   ```bash
   dotnet restore
   
3. Run the application:
   ```bash
   dotnet run --project ConsoleApp

### ⚙️ Configuration
- The SQLite database (Library.db) is auto-created in bin/Debug/net8.0.
- To change the location, modify the Data Source path in LibraryDbContext.cs.

### ✨ Added Functionality (Point 7)

### Intelligent Recommendation System:
#### Tracks user borrowing history.
#### Recommends books based on:
   - Frequently borrowed categories (40% weight)
   - Popular library titles (30% weight)
   - New arrivals (20% weight)
   - Author preferences (10% weight)
   - Access via Menu Option 8.

### 🕹️ Basic Usage
#### Add books (Option 1)
#### Borrow/return books (Options 4-5)
#### Test recommendations (Option 8) after borrowing 2+ books

### Note: The database resets on each run unless you persist the Library.db file.