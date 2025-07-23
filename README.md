# 💼 PennyPilot - Expense & Income Tracker

## 📌 Project Overview

PennyPilot is a full-stack web application designed to help users manage their personal finances by tracking both expenses and income. It provides a simple, user-friendly interface with insightful visualizations to help users understand their financial habits.

**Tech Stack:**

* Frontend: Angular 19
* Backend: ASP.NET Core Web API
* Database: PostgreSQL
* Authentication: JWT-based login/signup
* Charts: Chart.js, ngx-charts

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

  * Donut chart: Category-wise distribution
  * Line/Bar chart: Trends over time
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
* DOB (DateTime)
* PasswordHash (string)
* CreatedAt (DateTime)
* IsEnabled (bool)
* IsDeleted (bool)
* PasswordResetToken (string?)
* PasswordResetTokenExpiry (DateTime?)

### 💸 Expense

* ExpenseId (GUID)
* UserId (GUID, FK)
* CategoryId (GUID, FK)
* Title (string)
* Description (string?)
* Amount (decimal)
* PaymentMode (string)
* PaidBy (string)
* Date (DateTime)
* ReceiptImage (string or byte\[])
* CreatedAt (DateTime)
* UpdatedAt (DateTime?)
* IsEnabled (bool)
* IsDeleted (bool)

### 💰 Income

* IncomeId (GUID)
* UserId (GUID, FK)
* CategoryId (GUID, FK)
* Title (string)
* Source (string)
* Description (string?)
* Amount (decimal)
* Date (DateTime)
* CreatedAt (DateTime)
* UpdatedAt (DateTime?)
* IsEnabled (bool)
* IsDeleted (bool)

### 🏷️ Category

* CategoryId (GUID)
* Name (string, case-insensitive)
* Type (string) — 'Expense' or 'Income'
* IsEnabled (bool)
* IsDeleted (bool)

### 🏷️ UserCategory

* UserCategoryId (GUID)
* UserId (GUID, FK)
* CategoryID (GUID, FK)

---

## 🌐 API Endpoints

### 🔐 Authentication

* `POST /api/auth/register` – Register a new user
* `POST /api/auth/login` – Login and receive JWT token
* `POST /api/auth/forgot-password` – Generate password reset token
* `POST /api/auth/reset-password` – Reset password using reset token

### 📊 Charts
* `POST /api/Charts/DonutCharts` – Gets donut charts data
* `POST /api/Charts/IncomeExpenseBarChart` – Gets Bar chart data
* `POST /api/Charts/IncomeExpenseLineChart` – Gets Line chart data

### 💸 Expense

* `POST /api/expense` – Add List of expenses
* `POST /api/expense/ExpenseTable` – Gets list of paginated expenses

### 💰 Income

* `POST /api/income` – Add List of income
* `POST /api/income/ExpenseTable` – Gets list of paginated incomes

---

## 🖼️ Frontend Pages (Angular)

* Login/Signup Page
* Dashboard (analytics + summary)

---

## 📊 Charts & Analytics

* **Donut Chart - Expense Categories** – Expense category distribution
* **Donut Chart - User Expenses** – Expense user distribution
* **Donut Chart - Income Categories** – Income category distribution
* **Donut Chart - Income Sources** – Income source distribution
* **Line/Bar Chart** – Spending trend by date
* Date range selector (last 7 days, last 30 days,this month, last month, this year, custom, all)

---

## 🧱 UI Design with Angular Material

* Use Angular Material components for:

  * Navbar, Toolbar
  * Tables (mat-table)
  * Dialogs (Add/Edit forms)
  * Datepickers, Inputs, Dropdowns
  * Cards, Tabs, Icons
* Responsive layout 

---

## 🚀 Future Enhancements

* File uploads (receipt images)
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
