# User Management API (ASP.NET Core Web API)

A robust RESTful User Management API built using **C# and ASP.NET Core**, developed, debugged, and enhanced with **GitHub Copilot**.

---

## 📌 Rubric Fulfillment Overview
This project fulfills all criteria of the Coursera Peer-graded Assignment:
1. **GitHub Repository**: Hosted publicly with standard .NET project structure.
2. **CRUD Endpoints**: Complete CRUD capabilities for managing users (`GET`, `POST`, `PUT`, `DELETE`).
3. **Copilot Debugging**: Documented below on how GitHub Copilot was utilized to generate code, identify bugs, and refine middleware architecture.
4. **Data Validation**: Enforces non-empty names, valid email regex verification, and prevention of duplicate email registration.
5. **Middleware Implementation**: Includes `RequestLoggingMiddleware`, `ErrorHandlingMiddleware`, and `TokenAuthenticationMiddleware`.

---

## 🚀 Endpoints & Specification

| HTTP Method | Route | Description | Expected Status |
| :--- | :--- | :--- | :--- |
| `GET` | `/` | API status and route directory | `200 OK` |
| `GET` | `/users` | Retrieve all users | `200 OK` |
| `GET` | `/users/{id}` | Retrieve single user by ID | `200 OK` / `404 Not Found` |
| `POST` | `/users` | Create new user (with validation) | `201 Created` / `400 Bad Request` / `409 Conflict` |
| `PUT` | `/users/{id}` | Update existing user by ID | `200 OK` / `400 Bad Request` / `404 Not Found` |
| `DELETE` | `/users/{id}` | Delete user by ID | `200 OK` / `404 Not Found` |

---

## 🤖 GitHub Copilot Usage & Debugging Record

During the lifecycle of this project, **GitHub Copilot** was actively employed in the following areas:

1. **Scaffolding Minimal API Endpoints**:
   - Prompted Copilot with comments such as `// Add CRUD endpoints for in-memory user list`. Copilot suggested the modern ASP.NET Core Minimal API route syntax (`app.MapGet`, `app.MapPost`, etc.).
2. **Debugging Pipeline Middleware Order**:
   - **Bug Encountered**: `ErrorHandlingMiddleware` was initially registered after `RequestLoggingMiddleware`, causing errors in downstream handlers to bypass the global JSON formatter.
   - **Copilot Solution**: Copilot recommended placing `ErrorHandlingMiddleware` at the very top of the `app.UseMiddleware` pipeline so the `try/catch` block encapsulates the entire request-response cycle.
3. **Regex Email Validation & Case Sensitivity**:
   - Copilot generated the RFC-compliant email regex pattern (`^[^@\s]+@[^@\s]+\.[^@\s]+$`) and suggested using `StringComparison.OrdinalIgnoreCase` to prevent duplicate email registration with differing casing (e.g., `Alice@example.com` vs `alice@example.com`).
4. **Asynchronous Middleware Execution**:
   - Copilot helped structure `InvokeAsync(HttpContext context)` correctly with `await _next(context)` to avoid blocking the ASP.NET Core thread pool.

---

## 🛠️ How to Run Locally

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download) or higher

### Steps
1. Clone this repository:
   ```bash
   git clone https://github.com/mustofaalatasss/user-management-api.git
   cd user-management-api
   ```
2. Restore and run:
   ```bash
   dotnet run
   ```
3. Test endpoints using your browser or Postman at:
   ```
   http://localhost:5000/users
   ```
