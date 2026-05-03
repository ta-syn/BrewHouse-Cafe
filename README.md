# ☕ BrewHouse Cafe - Premium Management System

[![Build Status](https://img.shields.io/badge/Build-Success-brightgreen?style=for-the-badge&logo=dotnet)](https://github.com/ta-syn/BrewHouse-Cafe-)
[![Platform](https://img.shields.io/badge/Platform-.NET%208%20MVC-512bd4?style=for-the-badge&logo=dotnet)](https://github.com/ta-syn/BrewHouse-Cafe-)
[![Database](https://img.shields.io/badge/Database-SQL%20Server-red?style=for-the-badge&logo=microsoft-sql-server)](https://github.com/ta-syn/BrewHouse-Cafe-)
[![Frontend](https://img.shields.io/badge/Design-Glassmorphism%20%7C%20Premium-gold?style=for-the-badge&logo=css3)](https://github.com/ta-syn/BrewHouse-Cafe-)

Welcome to **BrewHouse Cafe**, an enterprise-grade Cafe Management System designed for modern cafes. Built with **ASP.NET Core 8 MVC**, this platform offers a seamless experience for customers, staff, and administrators with a focus on real-time operational efficiency and stunning aesthetics.

---

## 🌟 Key Features

### 📊 Advanced Dashboards
- **Dual-Metric Analytics:** Track "Today's vs. Total" orders and revenue at a glance.
- **High-Density Statistics:** Split-card design for efficient data visualization.
- **BST Synchronization:** All time-stamps are perfectly synced with **Bangladesh Standard Time (UTC+6)**.

### 🛒 Modern Customer Experience
- **Real-time Cart:** AJAX-powered "Add to Cart" with live badge updates and zero page refreshes.
- **Glassmorphic UI:** A visually stunning, modern search interface and menu layout.
- **Instant Feedback:** Integrated **SweetAlert2** for premium notifications and alerts.

### 🔐 Security & Access
- **Flexible Login:** Authenticate using either **Email or Name** for maximum convenience.
- **RBAC (Role-Based Access Control):** Dedicated portals for Admins, Staff, and Customers.
- **Secure Auth:** Industry-standard password hashing using **BCrypt**.

---

## 🛠️ Tech Stack

- **Backend:** C# | .NET 8 MVC
- **Database:** Entity Framework Core | SQL Server
- **Frontend:** Vanilla CSS (Premium Themes) | Bootstrap 5.3 | jQuery
- **Notifications:** SweetAlert2 | Animate.css
- **Patterns:** Dependency Injection | Repository Pattern | Custom Exception Filters

---

## 🚀 Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)

### Installation
1. **Clone the repository:**
   ```bash
   git clone https://github.com/ta-syn/BrewHouse-Cafe-.git
   ```

2. **Update Database:**
   Ensure your connection string is correct in `appsettings.json`, then run:
   ```bash
   dotnet ef database update
   ```

3. **Run the Application:**
   ```bash
   dotnet run
   ```
   The app will be available at `http://localhost:5100`.

---

## 📸 Design Philosophy
The system utilizes a **Premium Dark/Coffee Theme** with glassmorphism elements, ensuring a high-end feel that matches the quality of a boutique cafe. Every micro-interaction is polished to provide a smooth user journey.

---

## 🤝 Contribution
Contributions are welcome! Feel free to open a Pull Request or report an issue.

---

## 📄 License
Designed and Developed with ❤️ by **BrewHouse Team**.

> [!TIP]
> This system is fully optimized for **Bangladesh Standard Time**. No more timezone bugs!

---
© 2026 BrewHouse Cafe Management. All rights reserved.
