# 💼 SpendSmart - Expense & Income Tracker

## 📌 Project Overview

SpendSmart is a full-stack web application designed to help users manage their personal finances by tracking both expenses and income. It provides a simple, user-friendly interface with insightful visualizations to help users understand their financial habits.

**Tech Stack:**

* Frontend: Angular with Angular Material
* Backend: ASP.NET Core Web API
* Database: SQL Server (or SQLite for development)
* Authentication: JWT-based login/signup
* Charts: Chart.js or ngx-charts

---

## 👤 User Roles & Access

* **User**: Can register, log in, and manage their own data.
* Each user can:

  * Add/edit/delete their own expenses and income
  * Define their own categories (case-insensitive, avoids redundancy)
  * View personalized analytics and reports
* No admin or shared access functionality for now

---

## ✅ Core Features

* User registration and login
* Secure JWT authentication
* Add/edit/delete **expenses**
* Add/edit/delete **income**
* Filter/search expenses/income by keyword, category, and date
* Pagination and sorting on transaction lists
* Category management (user-defined, editable list)
* Analytics:

  * Pie chart: Category-wise distribution
  * Line/Area chart: Trends over time
* Date range filter for analytics

---

## 🧩 Data Models

### 🧍 User

* UserId (GUID)
* Username (string, unique)
* FirstName (string)
* MiddleName (string?)
* LastName (string)
* Email (string, unique)
* PasswordHash (string)
* CreatedAt (DateTime)

### 💸 Expense

* ExpenseId (GUID)
* UserId (GUID, FK)
* Title (string)
* Description (string?)
* Amount (decimal)
* Category (string)
* PaymentMode (string)
* PaidBy (string)
* Date (DateTime)
* ReceiptImage (string or byte\[])
* CreatedAt (DateTime)
* UpdatedAt (DateTime?)

### 💰 Income

* IncomeId (GUID)
* UserId (GUID, FK)
* Source (string)
* Description (string?)
* Amount (decimal)
* Date (DateTime)
* CreatedAt (DateTime)
* UpdatedAt (DateTime?)

### 🏷️ Category

* CategoryId (GUID)
* UserId (GUID, FK)
* Name (string, case-insensitive)
* Type (string) — 'Expense' or 'Income'

---

## 🌐 API Endpoints (Planned)

### 🔐 Authentication

* `POST /api/auth/register` – Register a new user
* `POST /api/auth/login` – Login and receive JWT token

### 💸 Expense

* `GET /api/expenses` – Get paginated expenses
* `GET /api/expenses/{id}` – Get a specific expense
* `POST /api/expenses` – Add new expense
* `PUT /api/expenses/{id}` – Update expense
* `DELETE /api/expenses/{id}` – Delete expense

### 💰 Income

* `GET /api/income` – Get paginated incomes
* `POST /api/income` – Add income
* `PUT /api/income/{id}` – Update income
* `DELETE /api/income/{id}` – Delete income

### 🏷️ Categories

* `GET /api/categories` – Get all user-defined categories
* `POST /api/categories` – Add category
* `DELETE /api/categories/{id}` – Delete category

### 📊 Analytics

* `GET /api/analytics/category-distribution` – Pie chart data
* `GET /api/analytics/date-trend` – Line/area chart data (date range supported)

---

## 🖼️ Frontend Pages (Angular)

* Login Page
* Register Page
* Dashboard (analytics + summary)
* Expense List + Filters
* Add/Edit Expense
* Income List + Filters
* Add/Edit Income
* Category Management
* User Profile (optional)

---

## 📊 Charts & Analytics

* **Pie Chart** – Expense category distribution
* **Line/Area Chart** – Spending trend by date
* Date range selector (last 7 days, this month, custom)

---

## 🧱 UI Design with Angular Material

* Use Angular Material components for:

  * Navbar, Toolbar
  * Tables (mat-table)
  * Dialogs (Add/Edit forms)
  * Datepickers, Inputs, Dropdowns
  * Cards, Tabs, Icons
* Responsive layout using `mat-grid` and `flex-layout`

---

## 🚀 Future Enhancements

* File uploads (receipt images)
* Budget vs. actual category tracking
* Shared accounts/family mode
* Export to Excel/PDF
* Monthly email summary/reminders

---

## ✅ Development Phases (Roadmap)

1. Define DB schema and relationships
2. Set up .NET backend with EF Core
3. Implement authentication (JWT)
4. Build CRUD APIs for expenses/income/categories
5. Set up Angular app with Material UI
6. Connect Angular frontend to backend via services
7. Add charts and analytics
8. Test and polish user experience

---

> 📂 You can version control this project using Git and deploy using Azure, Vercel, or Firebase Hosting.
