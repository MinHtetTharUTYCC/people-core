---
sessionId: session-261122-192058-1kqu
---

# Updated Plan

I've updated the `plan.md` with the following changes:

### What was removed:
1. **From "What it covers"** — Removed the "Testing" bullet point.
2. **From Tech Stack table** — Removed the "Testing" row (xUnit + Moq + TestServer).
3. **From Project Structure** — Removed the `PeopleCore.Tests/` project and its subdirectories.
4. **From Stage 1** — Removed xUnit from NuGet packages list.
5. **From Stages 2–7** — Removed all "Add unit tests for ... and integration tests for ..." bullet points.
6. **Stage 8** — Completely reworked: removed all test-related items (unit tests, integration tests, validator tests, test helpers, full test suite). Kept only non-test items: seed data, Swagger with JWT auth support, API documentation comments, migration verification, end-to-end flow verification, and final build.
7. **Testing Strategy section** — Removed entirely.

### What remains:
- All functional requirements, entities, DTOs, services, controllers, validators, migrations, authorization, and business logic are untouched.
- Stage 8 now focuses on finalization: seed data, Swagger polish, documentation, migration checks, end-to-end manual verification, and final build.

# Delivery Steps

### ✓ Step 1: Remove test references from Overview, Tech Stack, and Project Structure
Remove testing-related entries from the top-level sections of plan.md.

### ✓ Step 2: Remove test bullet points from Stages 1-7 and rewrite Stage 8
Strip all test-related bullets from delivery stages and rework Stage 8 to focus on non-test finalization only.

### ✓ Step 3: Implement Stage 1 - Setup project structure and infrastructure
Create the layered solution, configure references, add NuGet packages, create entities, DbContext, Program.cs, middleware, and first migration.

### ✓ Step 4: Implement Stage 2 - Authentication and authorization
Implement JWT-based auth with ASP.NET Identity, AuthService, AuthController, and validators.

### ✓ Step 5: Implement Stage 3 - Departments & Positions module
Build organizational structure management with CRUD operations.

### ✓ Step 6: Implement Stage 4 - Employee Management module
Build employee CRUD with filtering and pagination.

### ✓ Step 7: Implement Stage 5 - Leave Management module
Build leave request workflow with approval process.

### ✓ Step 8: Implement Stage 6 - Attendance & Time Tracking module
Build attendance tracking with clock-in/out.

### ✓ Step 9: Implement Stage 7 - Payroll Basics module
Build payroll processing with salary components and pay runs.

### ✓ Step 10: Implement Stage 8 - Finalize with documentation and verification
Add seed data, configure Swagger, verify end-to-end flow.