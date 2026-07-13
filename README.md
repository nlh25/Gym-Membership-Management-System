# Gym Membership Management System

A complete gym membership management system built with .NET 8 using Clean Architecture pattern.

## Architecture Overview

```
GymMembershipManagementSystem/
├── GMMS.Api          - ASP.NET Core 8 Web API (REST endpoints)
├── GMMS.App          - Blazor Server App with MudBlazor UI
├── GMMS.Domain       - Business logic, Services, Models, Result<T> pattern
├── GMMS.Database     - EF Core 8, SQL Server, Code-First Migrations
```

## Features

### Domain Entities
- **Member** - Gym members with unique codes
- **MembershipPlan** - Subscription plans (price, duration, description)
- **Membership** - Links Members to Plans with start/end dates and status
- **PaymentMethod** - Payment methods (Cash, Card, etc.)
- **Payment** - Payment records linked to Memberships

### API Endpoints

| Controller | Endpoints |
|------------|-----------|
| `api/Member` | GET, POST, PUT, DELETE |
| `api/MemberShip` | GET (by MemberId), GET all, POST, PUT, DELETE |
| `api/MemberShipPlan` | GET, POST, PUT, DELETE |
| `api/PaymentMethod` | GET, POST, PUT, DELETE |
| `api/Payment` | GET, POST |

All responses use `Result<T>` pattern:
```json
{
  "isSuccess": true,
  "message": "Operation successful",
  "data": { ... }
}
```

### Blazor UI (MudBlazor)
- **Members** - Full CRUD with search, pagination, dialogs
- **All Memberships** - View all memberships with filtering by status
- **Membership Plans** - Full CRUD via MudBlazor dialogs
- **Payment Methods** - Full CRUD via MudBlazor dialogs
- **Payments** - List and create payments

## Getting Started

### Prerequisites
- .NET 8 SDK
- SQL Server (LocalDB or Express)
- Visual Studio 2022 or VS Code

### Database Setup
1. Update connection string in `GMMS.Api/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DbConnection": "Server=.;Database=GMMSDb;User Id=sa;Password=12345;TrustServerCertificate=True;"
  }
}
```

2. Run migrations:
```bash
cd GMMS.Database
dotnet ef database update
```

### Running the Application

**Terminal 1 - API:**
```bash
cd GMMS.Api
dotnet run
```
API runs on `https://localhost:7xxx` (Swagger at `/swagger`)

**Terminal 2 - Blazor App:**
```bash
cd GMMS.App
dotnet run
```
App runs on `https://localhost:5xxx`

## Project Structure

### GMMS.Domain
```
Features/
  Member/
    MemberService.cs
    Models/
      MemberListModel.cs
  MemberShip/
    MemberShipService.cs
    Models/
      MemberShipModel.cs
  MemberShipPlan/
    MemberShipPlanService.cs
    Models/
      MemberShipPlanModel.cs
  Payment/
    PaymentService.cs
    Models/
      PaymentModel.cs
  PaymentMethod/
    PaymentMethodService.cs
    Models/
      PaymentMethodModel.cs
Enums/
  Enums.cs (MembershipPlanStatus, PaymentStatus)
Result.cs (Result<T> wrapper)
```

### GMMS.Database
```
AppDbContextModels/
  AppDbContext.cs
  TblMember.cs
  TblMembership.cs
  TblMembershipPlan.cs
  TblPayment.cs
  TblPaymentMethod.cs
Migrations/
```

### GMMS.Api
```
Controllers/
  BaseController.cs
  MemberController.cs
  MemberShipController.cs
  MemberShipPlanController.cs
  PaymentController.cs
  PaymentMethodController.cs
Program.cs
```

### GMMS.App
```
Components/
  Layout/
    MainLayout.razor
    NavMenu.razor
  Pages/
    Home.razor
Feature/
  Member/
    MemberList.razor (+ .cs)
    MemberCreate.razor (+ .cs)
    MemberEdit.razor (+ .cs)
    MemberDelete.razor (+ .cs)
  Membership/
    MembershipListAll.razor (+ .cs)
    MembershipCreate.razor (+ .cs)
    MembershipEdit.razor (+ .cs)
    MembershipDelete.razor (+ .cs)
  MembershipPlan/
    MembershipPlanList.razor (+ .cs)
    MembershipPlanCreate.razor (+ .cs)
    MembershipPlanEdit.razor (+ .cs)
    MembershipPlanDelete.razor (+ .cs)
  PaymentMethod/
    PaymentMethodList.razor (+ .cs)
    PaymentMethodCreate.razor (+ .cs)
    PaymentMethodEdit.razor (+ .cs)
    PaymentMethodDelete.razor (+ .cs)
  Payment/
    PaymentList.razor (+ .cs)
    PaymentCreate.razor (+ .cs)
    PaymentDetail.razor (+ .cs)
Services/
  ApiService.cs
  HttpClientService.cs
  ApiEndpoints.cs
Program.cs
App.razor
```

## Key Patterns

### Result<T> Pattern
All services return `Result<T>` for consistent error handling:
```csharp
public class Result<T>
{
    public bool IsSuccess { get; set; }
    public bool IsError => !IsSuccess;
    public string Message { get; set; }
    public T? Data { get; set; }
}
```

### Soft Delete
All entities use `IsDeleted` flag instead of hard deletes.

### Enum-Based Status
```csharp
public enum MembershipPlanStatus { none, Pending, Active, Expired }
public enum PaymentStatus { none, Pending, Completed, Failed }
```

## API Examples

### Create Member
```http
POST /api/Member
Content-Type: application/json

{
  "memberCode": "MEM001",
  "name": "John Doe"
}
```

### Create Membership
```http
POST /api/MemberShip
Content-Type: application/json

{
  "memberId": 1,
  "membershipPlanId": 1,
  "paymentMethodId": 1,
  "amount": 99.99,
  "sspath": "receipt.pdf"
}
```

### Create Membership Plan
```http
POST /api/MemberShipPlan
Content-Type: application/json

{
  "planCode": "GOLD",
  "planName": "Gold Membership",
  "price": 99.99,
  "durationDays": 30,
  "description": "Full access to all facilities"
}
```

## Technologies
- .NET 8
- ASP.NET Core Web API
- Blazor Server
- MudBlazor (Material Design)
- Entity Framework Core 8
- SQL Server
- Clean Architecture

## License
MIT