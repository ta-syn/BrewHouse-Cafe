# ☕ BrewHouse: Cafe Management System

A production-grade, enterprise-ready **Cafe Management System** built with **ASP.NET Core 8 MVC**. This project is designed for efficiency, security, and a premium user experience, demonstrating advanced Object-Oriented Programming (OOP) principles and modern software architecture.

---

## 🚀 Technologies Used
- **Backend:** ASP.NET Core 8 MVC (C#)
- **Database:** PostgreSQL (with Entity Framework Core)
- **Security:** BCrypt.Net-Next (Industry-standard password hashing)
- **UI/UX:** Bootstrap 5, Font Awesome 6, Google Fonts (Playfair Display & Lato)
- **Design Pattern:** Repository-Service Pattern & MVC
- **Deployment:** Optimized for Render.com

---

## ✨ Key Features

### 👑 Admin Dashboard
- **Menu Management:** Full CRUD operations for Beverages, Food, and Desserts using TPH inheritance.
- **Staff Control:** Manage staff accounts and permissions.
- **Analytics:** Real-time metrics for total revenue, today's sales, and popular items.
- **Table Management:** Track and manage cafe table occupancy.

### 🍳 Staff Portal
- **Kitchen Kanban Board:** Real-time order status management (Pending, Preparing, Ready).
- **Walk-in POS:** POS system for walk-in customers with AJAX-powered calculations.
- **Table Overview:** Quick view of available and occupied tables.

### 🛒 Customer Experience
- **Interactive Menu:** Searchable menu with category filtering and dynamic search.
- **Shopping Cart:** Session-based cart with JSON serialization.
- **Discount System:** Coupon code validation with expiry and percentage logic.
- **Order Tracking:** Live progress bar for tracking order status from kitchen to table.

---

## 🧩 OOP Concepts Applied
This project serves as a comprehensive demonstration of core **Object-Oriented Programming** principles:
- **Inheritance:** Concrete classes (`Beverage`, `Food`, `Dessert`) inheriting from the abstract `MenuItem`.
- **Polymorphism:** Method overriding and interface implementation for specialized behaviors.
- **Encapsulation:** Protecting data integrity via private fields and validated public properties.
- **Abstraction:** Using abstract classes and interfaces (`IOrderable`, `IDiscountable`) to define strict system contracts.
- **Exception Handling:** Custom exception hierarchy for robust error management.

---

## 🛠️ Local Setup Instructions

1. **Clone the Project**
2. **Database Configuration:**
   Update your connection string in `appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Host=localhost;Database=CafeDb;Username=postgres;Password=yourpassword"
   }
   ```
3. **Run Migrations:**
   ```bash
   dotnet ef database update
   ```
4. **Launch the App:**
   ```bash
   dotnet run
   ```

---

## 🔐 Credentials (Test Accounts)

| Role | Email | Password |
| :--- | :--- | :--- |
| **Admin** | `admin@cafe.com` | `admin123` |
| **Staff** | `staff@cafe.com` | `staff123` |

---

## ☁️ Deployment (Render.com)
The project is configured for seamless deployment on Render:
- **Build Command:** `dotnet publish -c Release -o out`
- **Start Command:** `dotnet out/CafeManagement.dll`
- **Database:** Set your `DATABASE_URL` environment variable to your PostgreSQL connection string.

---
*Developed with a focus on code quality, security, and performance.* ☕🔥
