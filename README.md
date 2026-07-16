# Gym Membership Management System

A complete gym membership management system built with .NET 8 using Clean Architecture pattern.

---

## 📋 Project Overview

### System Overview
A comprehensive gym membership management system designed to handle all aspects of gym operations:

- **Manage Members** - Register, update, and track gym members with unique codes
- **Manage Membership Plans** - Daily, Monthly, 3-Months, and custom duration plans with pricing
- **Create & Renew Memberships** - Link members to plans with automatic start/end date calculation
- **Record Payments** - Support for Cash, Mobile Banking, and other payment methods
- **Payment History** - Complete transaction tracking with receipt paths
- **Generate Income Reports** - Daily, Monthly, Yearly income reports
- **Membership Reports** - Active, Expired, Expiring Soon, and Cancelled membership reports
- **Payment History Reports** - Filterable by member, plan, payment method, date range

---

### 🗄️ Database Schema

#### **Tables Overview**

| Table | Description | Key Columns |
|-------|-------------|-------------|
| **Member** | Gym members | `MemberId`, `MemberCode` (unique), `Name`, `IsDeleted`, `CreatedAt`, `UpdatedAt` |
| **MembershipPlan** | Subscription plans | `MembershipPlanId`, `PlanName`, `Price`, `DurationDays`, `Description`, `IsActive`, `IsDeleted`, `CreatedAt`, `UpdatedAt` |
| **Membership** | Member-plan subscriptions | `MembershipId`, `MemberId`, `MembershipPlanId`, `StartDate`, `EndDate`, `Status` (Pending/Active/Expired/Cancelled), `CreatedAt`, `UpdatedAt` |
| **PaymentMethod** | Payment modes | `PaymentMethodId`, `Name` (Cash/Mobile/Card), `IsActive`, `CreatedAt`, `UpdatedAt` |
| **Payment** | Payment transactions | `PaymentId`, `MembershipId`, `PaymentMethodId`, `Amount`, `SSPath` (receipt), `Status` (Pending/Paid/Failed), `CreatedAt` |

#### **Relationships**

```
Member (1) ──────────────── (Many) Membership
MembershipPlan (1) ──────── (Many) Membership
Membership (1) ──────────── (Many) Payment
PaymentMethod (1) ───────── (Many) Payment
```

- **Member → Membership**: One member can have multiple memberships over time
- **MembershipPlan → Membership**: One plan can be subscribed by many members
- **Membership → Payment**: One membership can have multiple payments (installments)
- **PaymentMethod → Payment**: One payment method used in many payments

#### **Enums**

| Enum | Values |
|------|--------|
| `MembershipPlanStatus` | `Pending`, `Active`, `Expired`, `Cancelled` |
| `PaymentStatus` | `Pending`, `Paid`, `Failed` |

---

### 📊 Reports

| Report | Description | Filters |
|--------|-------------|---------|
| **Daily Income Report** | Total revenue per day | Date |
| **Monthly Income Report** | Revenue aggregated by month | Month/Year |
| **Yearly Income Report** | Annual revenue summary | Year |
| **Active Membership Report** | Currently active memberships | Plan, Date Range |
| **Expired Membership Report** | Expired/cancelled memberships | Date Range, Plan |
| **Expiring Soon Report** | Memberships expiring in N days | Days Threshold |
| **Payment History Report** | All payments with details | Member, Plan, Method, Date Range, Status |

---

### 🔌 API Endpoints

| Resource | Endpoints | Optional Filters |
|----------|-----------|------------------|
| **Member** | `GET /api/Member`, `GET /api/Member/{id}`, `POST /api/Member`, `PATCH /api/Member/{id}`, `DELETE /api/Member/{id}` | Search by code/name, Pagination |
| **MembershipPlan** | `GET /api/MemberShipPlan`, `GET /api/MemberShipPlan/{id}`, `POST /api/MemberShipPlan`, `PATCH /api/MemberShipPlan/{id}`, `DELETE /api/MemberShipPlan/{id}` | Pagination |
| **Membership** | `GET /api/MemberShip`, `GET /api/MemberShip/{id}`, `POST /api/MemberShip`, `PATCH /api/MemberShip/{id}`, `DELETE /api/MemberShip/{id}` | `?memberId=`, `?planId=`, `?status=`, Pagination |
| **PaymentMethod** | `GET /api/PaymentMethod`, `GET /api/PaymentMethod/{id}`, `POST /api/PaymentMethod`, `PATCH /api/PaymentMethod/{id}`, `DELETE /api/PaymentMethod/{id}` | Pagination |
| **Payment** | `GET /api/Payment`, `GET /api/Payment/{id}`, `POST /api/Payment` | `?membershipId=`, `?methodId=`, `?status=`, Pagination |

> **Response Format**: All endpoints return `Result<T>`:
> ```json
> { "isSuccess": true, "message": "Success", "data": { ... } }
> ```

---

### 🖥️ Blazor UI Pages (MudBlazor)

| Page | Route | Type | Features |
|------|-------|------|----------|
| **Member List** | `/member-list` | Page + Dialogs | Search, Pagination, Create/Edit/Delete Dialogs |
| **All Memberships** | `/membership-list-all` | Page + Dialogs | Status Filter, Search, Pagination |
| **Member Memberships** | `/membership-list?memberId=X` | Page + Dialogs | Filtered by Member |
| **Membership Plans** | `/membershipplan-list` | Page + Dialogs | Create/Edit/Delete Dialogs |
| **Payment Methods** | `/paymentmethod-list` | Page + Dialogs | Create/Edit/Delete Dialogs |
| **Payments** | `/payment-list` | Page + Dialog | List + Create Dialog + Detail Dialog |

**Navigation**: Sidebar with Members → All Memberships → Plans → Payment Methods → Payments

---

## 🚀 Quick Start

### Prerequisites
- .NET 8 SDK
- SQL Server (LocalDB, Express, or Full)
- Visual Studio 2022 / VS Code

### 1. Database Setup
```bash
cd GMMS.Database
dotnet ef database update
```
> Connection string in `GMMS.Api/appsettings.json`:
> ```json
> "ConnectionStrings": {
>   "DbConnection": "Server=(localdb)\\mssqllocaldb;Database=GMMSDb;Trusted_Connection=True;TrustServerCertificate=True;"
> }
> ```

### 2. Run API
```bash
cd GMMS.Api
dotnet run
```
- Swagger UI: `https://localhost:7xxx/swagger`
- API Base URL: `https://localhost:7xxx/api`

### 3. Run Blazor App
```bash
cd GMMS.App
dotnet run
```
- App URL: `https://localhost:5xxx`
- Configure `BackendApiUrl` in `GMMS.App/appsettings.json`:
  ```json
  "BackendApiUrl": "https://localhost:7xxx/"
  ```

### 4. Default Login Credentials

| Role | Username | Password |
|------|----------|----------|
| **Owner** | `owner` | `Owner@123` |
| **Admin** | `admin` | `Admin@123` |

> Both accounts are forced to change password on first login.

---

## ⚙️ Configuration

### GMMS.Api/appsettings.json
```json
{
  "ConnectionStrings": {
    "DbConnection": "Server=(localdb)\\mssqllocaldb;Database=GMMSDb;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Logging": { "LogLevel": { "Default": "Information" } }
}
```

### GMMS.App/appsettings.json
```json
{
  "BackendApiUrl": "https://localhost:7xxx/",
  "Logging": { "LogLevel": { "Default": "Information" } }
}
```

---

## 🏗️ Architecture

```
GymMembershipManagementSystem/
├── GMMS.Api          → ASP.NET Core Web API (Controllers, Swagger)
├── GMMS.App          → Blazor Server App (MudBlazor UI)
├── GMMS.Domain       → Business Logic (Services, Models, Result<T>, Enums)
├── GMMS.Database     → EF Core (Entities, DbContext, Migrations)
```

### Key Patterns
- **Result<T>** - Consistent API responses (`IsSuccess`, `Message`, `Data`)
- **Soft Delete** - `IsDeleted` flag on all entities
- **Clean Architecture** - Domain independent of infrastructure
- **Native `<select>` Dropdowns** - Used in Membership Create/Edit for reliable binding (see [Dropdown Fix](#-dropdown-fix))

---

## 🔧 Dropdown Fix (Membership Create/Edit)

**Issue**: MudBlazor `<MudSelect>` with `@foreach` didn't re-render items after async data load.

**Solution**: Use native `<select class="form-select">` with string binding properties:

```razor
<!-- In MembershipCreate.razor -->
<div class="mb-4">
    <MudText Typo="Typo.body2" Class="mb-1"><strong>Plan</strong> <span class="text-danger">*</span></MudText>
    <select class="form-select" @bind-value="_planStr" @bind-value:event="onchange">
        <option value="">-- Select Plan --</option>
        @foreach (var p in plans)
        {
            <option value="@p.MemberShipPlanId">@p.PlanName (@p.DurationDays days - @p.Price.ToString("C2"))</option>
        }
    </select>
</div>
```

```csharp
// In MembershipCreate.razor.cs
private string _planStr
{
    get => request.MembershipPlanId > 0 ? request.MembershipPlanId.ToString() : "";
    set => request.MembershipPlanId = int.TryParse(value, out var id) ? id : 0;
}
```

**Why**: Native `<select>` re-renders automatically when `plans` list updates; `MudSelect` with `@foreach` requires `Key` attribute or manual `StateHasChanged()`.

---

## 🐛 Troubleshooting

| Issue | Cause | Fix |
|-------|-------|-----|
| **Dropdown empty** | API not running / wrong URL | Check `BackendApiUrl` in `GMMS.App/appsettings.json`; ensure API runs |
| **API 404 / Swagger not loading** | API not started | Run `dotnet run` in `GMMS.Api` |
| **CORS Error** | Blazor app calling API cross-origin | Add `builder.Services.AddCors()` in `GMMS.Api/Program.cs` |
| **DB Migration fails** | SQL Server not running / wrong connection | Use LocalDB: `Server=(localdb)\\mssqllocaldb;Database=GMMSDb;Trusted_Connection=True;` |
| **Dropdown shows but no items** | `plans`/`members` list empty | Check Network tab → API calls returning data; verify `StateHasChanged()` called |
| **PaymentMethod not saving** | `IsActive` not bound | Ensure `<MudSwitch @bind-Checked="request.IsActive" />` or `<select>` for bool |

---

## 🛠️ Technologies

| Layer | Technology |
|-------|------------|
| **API** | ASP.NET Core 8, Swagger/OpenAPI |
| **UI** | Blazor Server, MudBlazor (Material Design) |
| **Database** | EF Core 8, SQL Server (LocalDB/Express) |
| **Architecture** | Clean Architecture, Result<T> Pattern |
| **Patterns** | Soft Delete, Repository-like Services, DTOs |

---

## 📁 Database Schema (Full SQL)

See [`docs/database-schema.sql`](docs/database-schema.sql) for complete CREATE TABLE scripts with constraints, indexes, and foreign keys.

---

## 📝 License

MIT License - Feel free to use and modify.