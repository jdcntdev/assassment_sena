# 🎓 Premium Course Platform - .NET 8 & React

A professional, feature-rich course management platform built with focus on **Clean Architecture**, **Business Logic Validation**, and **Premium User Experience**.

## ✨ Key Features

- **🛡️ Secure Auth**: JWT & Identity integration with secure session management.
- **🏗️ Clean Architecture**: Decoupled layers (Domain, Application, Infrastructure, Api) for maximum testability.
- **⚡ Advanced Course Management**: Search, pagination, and status filters (Draft/Published).
- **📝 Lesson Control**: Unique ordering per course, reordering logic, and management.
- **🗑️ Soft Delete**: All primary data respects logical deletion as per business rules.
- **🎨 Premium UI**: Dark-mode interface built with React, Vite, Framer Motion, and HSL-tailored colors.
- **✅ Tested Logic**: Unit tests cover core business requirements and data integrity.

## 🛠️ Technology Stack

### Backend
- **Framework**: .NET 8 (WebAPI)
- **Database**: PostgreSQL (Entity Framework Core)
- **Security**: ASP.NET Core Identity + JWT Bearer
- **Testing**: xUnit + Moq + EF InMemory

### Frontend
- **Framework**: React 18 + TypeScript (Vite)
- **Styling**: Vanilla CSS (Custom Variable System)
- **Animations**: Framer Motion
- **Icons**: Lucide React
- **API**: Axios

## 🚀 Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js](https://nodejs.org/) (v18+)
- PostgreSQL (or Supabase)

### Installation

1. **Clone your repository** (or copy this project):
   ```bash
   git clone <your-repo-url>
   cd CoursePlatform
   ```

2. **Configure Backend**:
   - Navigate to `CoursePlatform.Api/appsettings.json`.
   - Update `DefaultConnection` with your PostgreSQL string.
   - (Optional) Customize `Jwt:Secret`.

3. **Run Backend**:
   ```bash
   dotnet ef database update --project CoursePlatform.Infrastructure --startup-project CoursePlatform.Api
   # OR let Program.cs auto-create it (Safe mode).
   dotnet run --project CoursePlatform.Api
   ```

4. **Run Frontend**:
   ```bash
   cd frontend
   npm install
   npm run dev
   ```

## 🧪 Running Tests
```bash
dotnet test
```

## 📜 License
MIT License. Feel free to use this for your personal portfolio.
