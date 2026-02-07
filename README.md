# StudyPlanner 📘

**What it is**
A study planning and task management web application built with **ASP.NET Core MVC (.NET 8.0)**.

**Problem it solves**
Helps students and self-learners organize study tasks, schedule study sessions, and track progress in one structured place instead of scattered notes and reminders.

**Target audience**
Students and independent learners who want a simple, focused way to plan and manage their study workflow.

---

## 🧠 Project Goal

Deliver a clean, real‑world learning planner that demonstrates practical use of ASP.NET Core MVC, Entity Framework Core, authentication, and relational data modeling.

---

## 🛠️ Tech Stack

**Backend**

* ASP.NET Core MVC (.NET 8.0)

**Data**

* Entity Framework Core
* SQL Server 

**Frontend**

* Razor Views
* Bootstrap
* Bootstrap Icons

**Tools & Concepts**

* ASP.NET Identity (Authentication & Authorization)
* MVC Architecture
* CRUD Operations

---

## ⚙️ Setup & Run

### Prerequisites

* .NET SDK **8.0**
* SQL Server

### Installation

1. Clone the repository
2. Update the connection string in `appsettings.json`
3. Restore packages:

   ```bash
   dotnet restore
   ```
4. Apply migrations:

   ```bash
   dotnet ef database update
   ```
5. Run the application:

   ```bash
   dotnet run
   ```

   or press **F5** in Visual Studio

---

## 👥 User Roles & Permissions

* **Guest** – View public pages
* **Authenticated User** – Create and manage own study tasks and sessions

All data is user‑specific and protected by authorization rules.

---

## 🔑 Core Functionality

* CRUD operations for **Study Tasks**
* CRUD operations for **Study Sessions**
* CRUD operations for **Categories**
* CRUD operations for **Subjects**
* User authentication and authorization
* Relational data handling with EF Core

---

## 🧭 Application Flow

Main sections:

* `/Home`
* `/StudyTask`
* `/StudySession`
* `/Category`

Each study task acts as a parent entity with related study sessions.

---
