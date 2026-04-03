
# 📘 StudyPlanner

> A study planning and task management web application that helps students organize tasks, schedule study sessions, and track progress — all in one place.

![.NET Version](https://img.shields.io/badge/.NET-8.0-purple)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-8.0-blue)
![EF Core](https://img.shields.io/badge/EF_Core-8.0-orange)
![License](https://img.shields.io/badge/license-Apache_2.0-blue)

---

## 📋 Table of Contents

- [About the Project](#-about-the-project)
- [Technologies Used](#️-technologies-used)
- [Architecture](#️-architecture)
- [Entity Models](#️-entity-models)
- [Features](#-features)
- [User Roles & Permissions](#-user-roles--permissions)
- [Prerequisites](#-prerequisites)
- [Getting Started](#-getting-started)
- [Configuration](#️-configuration)
- [Project Structure](#-project-structure)
- [Seeding](#-seeding)
- [Validation](#-validation)
- [Security](#️-security)
- [Unit Tests](#-unit-tests)
- [API](#-api)
- [Contact](#-contact)

---

## 📖 About the Project

StudyPlanner is a multi-layer ASP.NET Core MVC web application (.NET 8.0) that helps students and self-learners organize their study workflow. Users can create and manage study tasks, log study sessions, organize work by subject and category, store useful resources, and track their progress — all in one structured interface.

**Problem it solves:** Students often struggle to track what they need to study, how long they've studied, and what's coming up. StudyPlanner brings everything into one clean, focused interface.

**Target audience:** Students and independent learners who want a simple, structured way to plan and manage their study workflow.

---

## 🛠️ Technologies Used

| Technology | Version | Purpose |
|------------|---------|---------|
| ASP.NET Core MVC | 8.0 | Web framework |
| Entity Framework Core | 8.0 | ORM / Database access |
| SQL Server (LocalDB) | Latest | Primary database |
| ASP.NET Core Identity | 8.0 | Authentication & Authorization |
| Bootstrap | 5.3 | Frontend styling |
| Bootstrap Icons | Latest | UI icons |
| Razor Views | 8.0 | Server-side HTML rendering |
| jQuery Validation | 1.19.5 | Client-side form validation |
| QuestPDF | Latest | PDF generation for study tasks |
| ZenQuotes API | Public | Motivational quotes in footer |

---

## 🏗️ Architecture

The application follows a **clean multi-project architecture** with strict separation of concerns across 6 projects:

```
StudyPlanner Solution
│
├── StudyPlanner.GCommon              # Shared constants, enums, validation rules
├── StudyPlanner.Data.Models          # Entity models (domain layer)
├── StudyPlanner.Data                 # DbContext, Repository, Migrations, Seeding
├── StudyPlanner.Services             # Service interfaces (Contracts) + implementations + DTOs
├── StudyPlanner.Services.Models      # DTOs organized per entity
├── StudyPlanner.ViewModels           # ViewModels and InputModels for views
├── StudyPlanner.Web                  # Presentation layer (Controllers, Views, Areas)
├── StudyPlanner.Web.Infrastructure   # App startup extensions (middleware, seeders)
└── StudyPlanner.Services.Tests       # NUnit unit tests for all services
```

### Layer Responsibilities

**`GCommon`** — validation length constants (`SubjectNameMaxLength`), application constants (`PageSize`, `AdminOrUser`), and enums (`TaskPriority`, `TaskStatus`).

**`Data.Models`** — pure entity classes with EF Core attributes and navigation properties. No business logic.

**`Data`** — `ApplicationDbContext`, generic `IRepository<T>` / `Repository<T>`, EF Core migrations, and `IdentitySeeder` for roles and admin user.

**`Services`** — service contracts (interfaces) and their implementations. All business logic lives here. Services speak **DTOs** — they have zero knowledge of ViewModels or views.

**`ViewModels`** — `InputModels` (with `[Required]`, `[MaxLength]` validation attributes for forms) and `ViewModels` (read-only display models for views).

**`Web`** — MVC Controllers, Razor Views, Areas (Admin, Identity), and `wwwroot`. Controllers are the bridge: they receive ViewModels from forms, map them to DTOs, call services, receive DTOs back, map to ViewModels, and pass to views.

**`Web.Infrastructure`** — `WebApplicationExtensions.cs` with `UseRolesSeeder()` and `UseAdminUserSeeder()` middleware extension methods called from `Program.cs`.

### Data Flow

```
View (ViewModel) → Controller → DTO → Service → Entity → Database
Database → Entity → Service → DTO → Controller → ViewModel → View
```

### Design Patterns Used

- **Repository Pattern** — generic `IRepository<T>` abstracts EF Core queries
- **Service Layer Pattern** — all business logic in services, controllers stay thin
- **DTO Pattern** — separate data transfer objects per operation (Read, Create, Edit)
- **MVC Areas** — Admin area separated from main application

---

## 🗃️ Entity Models

| Model | Description | Key Relationships |
|-------|-------------|-------------------|
| `ApplicationUser` | Extended IdentityUser with `FullName` and `DateOfBirth` | Owns all user data |
| `ApplicationRole` | Extended IdentityRole | Admin, User roles |
| `StudyTask` | Core task with title, description, due date, priority, status | Belongs to Category, Subject, User |
| `StudySession` | A timed study session linked to a task | Belongs to StudyTask, User |
| `Category` | Color-coded task category | Owns StudyTasks |
| `Subject` | Color-coded subject area | Owns StudyTasks |
| `Resource` | Personal resource library (links, books, articles) | Belongs to User only |

---

## ✨ Features

### Core Features
- ✅ User registration and login (ASP.NET Core Identity)
- ✅ CRUD for Study Tasks with priority (Low/Medium/High) and status (NotStarted/InProgress/Completed)
- ✅ CRUD for Study Sessions with start/end time, duration tracking, and notes
- ✅ CRUD for Categories with color coding
- ✅ CRUD for Subjects with color coding
- ✅ CRUD for Resources (personal link/book library with clickable URLs)
- ✅ User-specific data — each user sees only their own data
- ✅ Ownership validation — users cannot access other users' data

### Search & Pagination
- ✅ Search by name on Category index
- ✅ Search by name on Subject index
- ✅ Search by title + filter by priority on StudyTask index
- ✅ Server-side pagination on Category, Subject, StudyTask, Resource indexes
- ✅ Server-side pagination on study sessions inside StudyTask Details

### Admin Area
- ✅ Admin-only area (`/Admin/Users`) for user management
- ✅ List all users with full name and email
- ✅ Search users by name or email
- ✅ Server-side pagination on user list
- ✅ Delete users (with self-deletion protection)
- ✅ Role-based access — only `Admin` role can access

### Additional Features
- ✅ PDF download for study tasks with their sessions (QuestPDF)
- ✅ Motivational quote in footer via ZenQuotes public API (AJAX)
- ✅ Custom error pages (400, 401, 403, 404, 500)
- ✅ Responsive Bootstrap 5 UI
- ✅ Partial views and sections used throughout
- ✅ Total study time calculation per task
- ✅ Profile management page (Manage/Index)

---

## 👥 User Roles & Permissions

| Role | Access |
|------|--------|
| Guest | Home page only — all navigation hidden |
| User | Full CRUD on own tasks, sessions, categories, subjects, resources |
| Admin | Everything a User can do + access to Admin area (user management) |

Roles are seeded automatically on startup. Role-based authorization uses `[Authorize(Roles = ApplicationRoles.AdminRoleName)]` constants defined in `GCommon`.

---

## ✅ Prerequisites

- [.NET SDK 8.0+](https://dotnet.microsoft.com/download)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or later
- SQL Server LocalDB (included with Visual Studio) or SQL Server Express
- [Git](https://git-scm.com/)

---

## 🚀 Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/LiquidIvo/SoftUni_StudyPlanner.git
cd SoftUni_StudyPlanner
```

### 2. Restore dependencies

```bash
dotnet restore
```

### 3. Apply database migrations

```bash
dotnet ef database update --project StudyPlanner.Data --startup-project StudyPlanner.Web
```

### 4. Run the application

```bash
cd StudyPlanner.Web
dotnet run
```

Or open the solution in **Visual Studio** and press **F5**.

### 5. Log in as Admin

```
Email:    admin@gmail.com
Password: Admin12345!
```

---

## ⚙️ Configuration

The connection string is configured in `appsettings.json`. The app checks for `DevConnection` first, then falls back to `DefaultConnection`:

```json
{
  "ConnectionStrings": {
    "DevConnection": "Server=(localdb)\\mssqllocaldb;Database=StudyPlanner;Trusted_Connection=True;",
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=StudyPlanner;Trusted_Connection=True;"
  }
}
```

### Identity / Password Requirements

```json
{
  "IdentityOptions": {
    "Password": {
      "RequireDigit": true,
      "RequireLowercase": true,
      "RequireUppercase": true,
      "RequireNonAlphanumeric": false,
      "RequiredLength": 8,
      "RequiredUniqueChars": 1
    },
    "Lockout": {
      "MaxFailedAccessAttempts": 5,
      "DefaultLockoutTimeSpanMinutes": 15
    },
    "User": {
      "RequireUniqueEmail": true
    }
  }
}
```

---

## 📁 Project Structure

```
SoftUni_StudyPlanner/
│
├── StudyPlanner.GCommon/
│   ├── ApplicationConstants.cs
│   ├── EntityValidation.cs
│   └── Enums/TaskPriority.cs, TaskStatus.cs
│
├── StudyPlanner.Data.Models/
│   ├── ApplicationUser.cs, ApplicationRole.cs
│   ├── Category.cs, Subject.cs
│   ├── StudyTask.cs, StudySession.cs
│   └── Resource.cs
│
├── StudyPlanner.Data/
│   ├── ApplicationDbContext.cs
│   ├── Repositories/Repository.cs, Interfaces/IRepository.cs
│   └── Seeding/IdentitySeeder.cs, Contracts/IIdentitySeeder.cs
│
├── StudyPlanner.Services/
│   ├── Contracts/IAdminService, ICategoryService, ISubjectService,
│   │            IStudyTaskService, IStudySessionService,
│   │            IResourceService, IQuoteService, IPdfService
│   ├── Models/Admin, Category, Subject, StudyTask, StudySession, Resource, Quote
│   └── Services/AdminService, CategoryService, SubjectService,
│               StudyTaskService, StudySessionService,
│               ResourceService, QuoteService, PdfService
│
├── StudyPlanner.ViewModels/
│   └── Admin, Category, Subject, StudyTask, StudySession, Resource
│
├── StudyPlanner.Web/
│   ├── Areas/Admin/Controllers/UsersController.cs
│   │        Admin/Views/Users/Index.cshtml, Delete.cshtml
│   │        Identity/Pages/Account/Login, Register, Logout, Manage/Index
│   ├── Controllers/Home, Category, Subject, StudyTask, StudySession, Resource
│   │             Api/QuoteController.cs
│   ├── Views/Category, Subject, StudyTask, StudySession, Resource, Home, Shared
│   ├── wwwroot/css/site.css, js/quote.js, js/site.js
│   └── Program.cs
│
├── StudyPlanner.Web.Infrastructure/
│   └── Extensions/WebApplicationExtensions.cs
│
└── StudyPlanner.Services.Tests/
    ├── SubjectServiceTests.cs
    ├── CategoryServiceTests.cs
    ├── StudyTaskServiceTests.cs
    ├── StudySessionServiceTests.cs
    ├── ResourceServiceTests.cs
    ├── AdminServiceTests.cs
    ├── QuoteServiceTests.cs
    └── PdfServiceTests.cs
```

---

## 🌱 Seeding

The application seeds data automatically on every startup:

```csharp
app.UseRolesSeeder();      // creates Admin and User roles if they don't exist
app.UseAdminUserSeeder();  // creates admin@gmail.com with Admin role if not exists
```

Seeding is idempotent — if the roles and admin user already exist, nothing happens.

**Seeded admin credentials:**

```
Email:    admin@gmail.com
Password: Admin12345!
```

---

## 🔒 Validation

### Server-side
- All InputModels use Data Annotations (`[Required]`, `[MaxLength]`, `[MinLength]`, `[Url]`, `[RegularExpression]`)
- Validation constants centralized in `GCommon/EntityValidation.cs`
- Services validate ownership before every read/write operation
- `ModelState.IsValid` checked in every POST action

### Client-side
- jQuery Validation + Unobtrusive Validation via `_ValidationScriptsPartial`
- Included in all Create and Edit views via `@section Scripts`

### Database-level
- `[Required]` → `NOT NULL` columns
- `[MaxLength]` → `nvarchar(N)` columns
- Foreign key constraints via EF Core relationships

---

## 🛡️ Security

- **Authentication** — ASP.NET Core Identity with cookie-based auth
- **Authorization** — `[Authorize]` on all controllers, `[Authorize(Roles)]` for Admin area
- **Ownership checks** — every service method verifies `entity.UserId == userId`
- **CSRF protection** — `[ValidateAntiForgeryToken]` on all POST actions
- **XSS prevention** — Razor auto-escapes all output
- **SQL injection** — fully prevented by EF Core parameterized queries
- **External links** — `rel="noopener noreferrer"` on all `target="_blank"` links

---

## 🧪 Unit Tests

The solution includes a dedicated test project — `StudyPlanner.Services.Tests` — covering all 8 services with NUnit.

### Test Stack

| Package | Purpose |
|---------|---------|
| NUnit | Test framework |
| Moq | Mocking repositories and services |
| MockQueryable.Moq | Makes EF Core async methods work on mocked `IQueryable` |
| RichardSzalay.MockHttp | Mocks `HttpClient` for `QuoteService` |

### Coverage

Every service is fully tested with the following scenario types:

| Scenario | Example |
|----------|---------|
| Happy path | Valid input returns expected DTO |
| Not found | Missing entity throws `KeyNotFoundException` |
| Wrong user | Other user's entity throws `UnauthorizedAccessException` |
| Invalid input | Bad foreign key throws `ArgumentException` |
| Guard checks | Delete with related data throws `InvalidOperationException` |
| Pagination | Page 2 with page size 3 returns correct slice |
| Search / filter | Search term or priority filter returns matching results |
| Side effects | `Verify` confirms `AddAsync`, `Update`, `Delete`, `SaveChangesAsync` were called |

### Running the Tests

```bash
dotnet test
```

Or in Visual Studio: `Test → Run All Tests`

### Viewing Coverage

Install the **Fine Code Coverage** extension in Visual Studio (`Extensions → Manage Extensions`), then run your tests. The coverage panel opens via `View → Other Windows → Fine Code Coverage` and shows line and branch coverage per class.

> Business logic coverage exceeds **65%** across all services.

---

## 🌐 API

```
GET /api/quote
```

Returns a random motivational quote from ZenQuotes:

```json
{
  "text": "The secret of getting ahead is getting started.",
  "author": "Mark Twain"
}
```

Fetched client-side via AJAX on every page load. Falls back to a hardcoded quote if the API is unavailable.

---

## 📬 Contact

**LiquidIvo** — [@LiquidIvo](https://github.com/LiquidIvo)

Project Link: [https://github.com/LiquidIvo/SoftUni_StudyPlanner](https://github.com/LiquidIvo/SoftUni_StudyPlanner)

---

*Built as part of the **ASP.NET Advanced** course — SoftUni 2026.*
