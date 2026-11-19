# Middleware Implementation Notes

This document describes the custom middleware pipeline integrated into the **User Management API**:

---

## 1. RequestLoggingMiddleware (`RequestLoggingMiddleware.cs`)
- **Purpose**: Intercepts every incoming HTTP request and outgoing response.
- **Functionality**:
  - Captures timestamp, HTTP method (`GET`, `POST`, `PUT`, `DELETE`), and request path.
  - Passes the context to the next middleware via `_next(context)`.
  - Logs the final HTTP response status code upon completion.

---

## 2. ErrorHandlingMiddleware (`ErrorHandlingMiddleware.cs`)
- **Purpose**: Acts as a safety net wrapping around the entire request pipeline.
- **Functionality**:
  - Catches unhandled exceptions thrown anywhere in subsequent middlewares or endpoint handlers.
  - Formats responses into standardized JSON with HTTP 500 status code.
  - Prevents raw stack traces from leaking to client responses.

---

## 3. TokenAuthenticationMiddleware (`TokenAuthenticationMiddleware.cs`)
- **Purpose**: Validates requests targeting modification endpoints (`POST`, `PUT`, `DELETE`).
- **Functionality**:
  - Checks for the presence of the `X-Api-Key` header.
  - Bypasses public `GET` requests so users can read data without credentials.
