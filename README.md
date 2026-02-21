# 📘 StudyPlanner

> A study planning and task management web application that helps students organize tasks, schedule study sessions, and track progress in one place.

![.NET Version](https://img.shields.io/badge/.NET-8.0-purple)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-8.0-blue)
![EF Core](https://img.shields.io/badge/EF_Core-8.0-orange)
![License](https://img.shields.io/badge/license-Apache_2.0-blue)

---

## 📋 Table of Contents

- [About the Project](#-about-the-project)
- [Technologies Used](#️-technologies-used)
- [Prerequisites](#-prerequisites)
- [Getting Started](#-getting-started)
- [Configuration](#️-configuration)
- [Project Structure](#-project-structure)
- [Features](#-features)
- [Usage](#-usage)
- [User Roles & Permissions](#-user-roles--permissions)
- [Contact](#-contact)

---

## 📖 About the Project

StudyPlanner is a web application built with ASP.NET Core MVC (.NET 8.0) that helps students and self-learners organize their study workflow. Users can create and manage study tasks, log study sessions, and organize work by subject and category — all in one structured place instead of scattered notes and reminders.

**Problem it solves:** Students often struggle to track what they need to study, how long they've studied, and what's coming up. StudyPlanner brings everything into one clean, focused interface.

**Target audience:** Students and independent learners who want a simple, structured way to plan and manage their study workflow.

---

## 🛠️ Technologies Used

| Technology | Version | Purpose |
|------------|---------|---------|
| ASP.NET Core MVC | 8.0 | Web framework |
| Entity Framework Core | 8.0 | ORM / Database access |
| SQL Server (LocalDB) | 17.0.1000.7 | Database |
| ASP.NET Core Identity | 8.0 | Authentication & Authorization |
| Bootstrap | 5.3 | Frontend styling |
| Bootstrap Icons | latest | UI icons |
| Razor Views | 8.0 | Server-side HTML rendering |
| jQuery Validation | 1.19.5 | Client-side validation |

---

## ✅ Prerequisites

Make sure you have the following installed before running the project:

- [.NET SDK 8.0+](https://dotnet.microsoft.com/download)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)
- SQL Server LocalDB (included with Visual Studio) or [SQL Server Express](https://www.microsoft.com/en-us/sql-server)
- [Git](https://git-scm.com/)

---

## 🚀 Getting Started

**No configuration required!** The app is pre-configured and works out of the box.

### 1. Clone the repository

```bash
git clone https://github.com/LiquidIvo/StudyPlanner.git
cd StudyPlanner
```

### 2. Restore dependencies

```bash
dotnet restore
```

### 3. Apply database migrations

```bash
dotnet ef database update --project StudyPlanner.Data --startup-project StudyPlanner
```

### 4. Run the application

```bash
cd StudyPlanner
dotnet run
```

Or open the solution in **Visual Studio** and press **F5**.

### 5. Access the application

Navigate to `https://localhost:5001` or `http://localhost:5000`

---

## ⚙️ Configuration

The application uses **SQL Server LocalDB** which is included with Visual Studio — no additional setup required.

The connection string is pre-configured in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=StudyPlanner.Web;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

**No modifications needed for local development!**

> For other SQL Server setups, update the connection string:
> - **SQL Server Express:** `Server=localhost\\SQLEXPRESS;Database=StudyPlanner.Web;Trusted_Connection=True;`
> - **Full SQL Server:** `Server=localhost;Database=StudyPlanner.Web;Trusted_Connection=True;`

### Identity / Password Requirements

The application requires passwords to meet these criteria:
- Minimum **8 characters**
- At least **1 uppercase** letter
- At least **1 lowercase** letter
- At least **1 digit**
- At least **1 special character**

---

## 📁 Project Structure

```
StudyPlanner/
│
├── StudyPlanner.Web/                    # Web (Presentation Layer)
│   ├── Controllers/                     # MVC Controllers
│   │   ├── HomeController.cs
│   │   ├── CategoryController.cs
│   │   ├── SubjectController.cs
│   │   ├── StudyTaskController.cs
│   │   └── StudySessionController.cs
│   ├── Views/                           # Razor Views (.cshtml)
│   │   ├── Category/                    # Create, Edit, Delete, Index
│   │   ├── Subject/                     # Create, Edit, Delete, Index
│   │   ├── StudyTask/                   # Create, Edit, Delete, Index, Details
│   │   ├── StudySession/                # Create, Edit, Delete, Details
│   │   ├── Home/
│   │   └── Shared/                      # Layout, LoginPartial
│   ├── Areas/Identity/Pages/Account/   # Login, Register, Logout
│   ├── appsettings.json                 # App configuration
│   └── Program.cs                       # Entry point & DI setup
│
├── StudyPlanner.Data/                   # Data Layer
│   ├── ApplicationDbContext.cs          # EF Core DbContext (IdentityDbContext)
│   ├── Data/Migrations/                 # EF Core Migrations
│   └── Repositories/                    # Repository pattern
│       ├── Interfaces/IRepository.cs
│       └── Repository.cs
│
├── StudyPlanner.Data.Models/            # Entity Models
│   ├── Category.cs
│   ├── Subject.cs
│   ├── StudyTask.cs
│   └── StudySession.cs
│
├── StudyPlanner.Services/               # Service Layer
│   ├── Contracts/                       # Service interfaces
│   │   ├── ICategoryService.cs
│   │   ├── ISubjectService.cs
│   │   ├── IStudyTaskService.cs
│   │   └── IStudySessionService.cs
│   └── Services/                        # Service implementations
│       ├── CategoryService.cs
│       ├── SubjectService.cs
│       ├── StudyTaskService.cs
│       └── StudySessionService.cs
│
├── StudyPlanner.ViewModels/             # ViewModels / DTOs
│   ├── Category/
│   ├── Subject/
│   ├── StudyTask/
│   └── StudySession/
│
└── StudyPlanner.GCommon/                # Shared / Common
    ├── EntityValidation.cs              # Validation constants
    ├── ApplicationConstants.cs
    └── Enums/
        ├── TaskPriority.cs              # Low, Medium, High
        └── TaskStatus.cs               # Pending, InProgress, Completed
```

---

## ✨ Features

- ✅ User registration and login (ASP.NET Core Identity)
- ✅ CRUD operations for Study Tasks (with priority and status tracking)
- ✅ CRUD operations for Study Sessions (with duration tracking)
- ✅ CRUD operations for Categories (with color coding)
- ✅ CRUD operations for Subjects (with color coding)
- ✅ User-specific data — each user sees only their own data
- ✅ Ownership validation — users cannot access or modify other users' data
- ✅ Server-side and client-side input validation
- ✅ Responsive UI with Bootstrap 5
- ✅ Clean 3-layer architecture (Data, Services, Presentation)
- ✅ Repository pattern for data access
- ✅ Navigation hidden from unauthenticated users
- ✅ Downloadable PDF for specific study task with its study sessions

---

## 💻 Usage

### First Time Setup
1. Navigate to the app and click **Register**
2. Create an account (email + password)
3. Log in and start planning!

### Managing Study Tasks
1. Click **Tasks** in the navigation bar
2. Click **+ Create Task** to add a new task
3. Fill in title, description, due date, priority (Low/Medium/High), status (Pending/InProgress/Completed), category, and subject
4. View, edit, or delete tasks from the task list

### Logging Study Sessions
1. Open a task's **Details** page
2. Add a study session with start time, end time, and notes
3. View total study time tracked per task

### Organizing with Categories & Subjects
1. Go to **Categories** or **Subjects** in the navbar
2. Create color-coded entries to organize your tasks
3. Assign them when creating or editing study tasks

---

## 👥 User Roles & Permissions

| Role | Permissions |
|------|-------------|
| Guest | Home page only — navigation links are hidden |
| Authenticated User | Full CRUD on own tasks, sessions, categories, and subjects |

All data is **user-specific** — each user can only see and manage their own data.

---

## 📬 Contact

**Your Name** — [@LiquidIvo](https://github.com/LiquidIvo)

Project Link: [https://github.com/LiquidIvo/SoftUni_StudyPlanner](https://github.com/LiquidIvo/SoftUni_StudyPlanner)

---

*Built as part of the **ASP.NET Fundamentals** course — SoftUni.*
