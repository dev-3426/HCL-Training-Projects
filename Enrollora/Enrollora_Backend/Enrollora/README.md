# Enrollora — Student & Course Management API

**Project:** Enrollora  
**Framework:** .NET 10 / ASP.NET Core Web API  
**Database:** SQL Server + Entity Framework Core 10  
**Authentication:** JWT Bearer  
**Frontend-ready CORS origin:** `http://localhost:4200`

Enrollora provides student self-registration with administrator approval, course management, capacity-aware enrollments, cancellation workflows, and administrative reporting.

## 1. Requirements

- .NET 10 SDK
- SQL Server Express / Developer / Standard
- Visual Studio 2026 or Visual Studio Code
- Optional: Postman

## 2. Configure the database and JWT

Open `src/Enrollora.Api/appsettings.json`.

Update `ConnectionStrings:DefaultConnection` to match your SQL Server instance.

For example:

```json
"DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=EnrolloraDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
```

Change the JWT signing key and the seeded administrator credentials before using this outside local development. Prefer user-secrets or environment variables for secrets.

The development seed administrator is read from `SeedAdmin`:
- Email: `admin@enrollora.local`
- Password: `ChangeMe@123`

The administrator is seeded when the API starts for the first time after the database is created.

## 3. Create the database

From the solution folder:

```bash
dotnet restore
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate --project src/Enrollora.Api --startup-project src/Enrollora.Api
dotnet ef database update --project src/Enrollora.Api --startup-project src/Enrollora.Api
dotnet run --project src/Enrollora.Api
```

`DbSeeder` applies pending migrations at startup as well. The explicit migration commands above are recommended so that the initial schema is visible in source control.

Open the Swagger URL printed by the application (typically `/swagger` in Development).

## 4. Authentication workflow

### Student registration

`POST /api/auth/register`

```json
{
  "name": "Arun Kumar",
  "email": "arun@example.com",
  "password": "Student@123",
  "department": "Computer Science",
  "age": 21,
  "cgpa": 8.4
}
```

The student account is created with `Pending` status. Pending and rejected students cannot log in.

### Administrator login

`POST /api/auth/login`

```json
{
  "email": "admin@enrollora.local",
  "password": "ChangeMe@123"
}
```

Copy the returned token and use Swagger's **Authorize** action with `Bearer <token>`.

### Student login

The student can log in only after an administrator approves the registration.

## 5. API endpoint and authorization matrix

| Method | Endpoint | Access | Purpose |
|---|---|---|---|
| POST | `/api/auth/register` | Public | Student registration |
| POST | `/api/auth/login` | Public | Admin / Student login |
| GET | `/api/students` | Admin | Filtered student list |
| GET | `/api/students/{id}` | Admin | Student details |
| GET | `/api/students/me` | Student | Own profile |
| PUT | `/api/students/me` | Student | Update own profile |
| PUT | `/api/students/{id}` | Admin | Update student |
| PUT | `/api/students/{id}/approve` | Admin | Approve pending student |
| PUT | `/api/students/{id}/reject` | Admin | Reject pending student |
| DELETE | `/api/students/{id}` | Admin | Delete student without enrollment history |
| GET | `/api/courses` | Admin, Student | Filtered course list |
| GET | `/api/courses/{id}` | Admin, Student | Course details |
| POST | `/api/courses` | Admin | Create course |
| PUT | `/api/courses/{id}` | Admin | Update course |
| DELETE | `/api/courses/{id}` | Admin | Deactivate course (history preserved) |
| GET | `/api/enrollments` | Admin | All enrollments |
| GET | `/api/enrollments/my` | Student | Own enrollments |
| GET | `/api/enrollments/student/{studentId}` | Admin | Enrollments by student |
| GET | `/api/enrollments/course/{courseId}` | Admin | Enrollments by course |
| POST | `/api/enrollments` | Student | Enroll in a course |
| POST | `/api/enrollments/{enrollmentId}/cancellation-request` | Student | Request cancellation |
| GET | `/api/cancellation-requests` | Admin | All cancellation requests |
| GET | `/api/cancellation-requests/my` | Student | Own cancellation requests |
| PUT | `/api/cancellation-requests/{id}/approve` | Admin | Approve cancellation |
| PUT | `/api/cancellation-requests/{id}/reject` | Admin | Reject cancellation |
| GET | `/api/dashboard/summary` | Admin | Dashboard KPIs |

Student identity for profile, enrollment, and cancellation operations is read from the validated JWT claim. The API does not accept a client-supplied StudentId for those operations.

## 6. Filtering, sorting and pagination

All list endpoints return a `PagedResult<T>` with `items`, `pageNumber`, `pageSize`, `totalRecords`, and `totalPages`. Page size is capped at 100.

### Students

```http
GET /api/students?search=arun&status=Pending&department=Computer%20Science&minAge=18&maxAge=25&minCgpa=7&maxCgpa=10&sortBy=cgpa&sortOrder=desc&pageNumber=1&pageSize=10
```

Supported filters: `search`, `status`, `department`, `minAge`, `maxAge`, `minCgpa`, `maxCgpa`. Sorting: `name`, `age`, `cgpa`, `registeredAt`.

### Courses

```http
GET /api/courses?search=dotnet&isActive=true&availability=available&minFees=1000&maxFees=50000&minCapacity=10&maxCapacity=100&sortBy=fees&sortOrder=asc&pageNumber=1&pageSize=10
```

Supported filters: `search`, `isActive`, `availability` (`available` or `full`), `minFees`, `maxFees`, `minCapacity`, `maxCapacity`. Sorting: `name`, `fees`, `duration`, `capacity`.

### Enrollments

```http
GET /api/enrollments?search=arun&courseId=1&status=Active&fromDate=2026-01-01&toDate=2026-12-31&sortBy=enrolledAt&sortOrder=desc&pageNumber=1&pageSize=10
```

Supported filters: `search` (student name/email), `courseId`, `status`, `fromDate`, `toDate`. Sorting: `student`, `course`, `enrolledAt`.

### Cancellation requests

```http
GET /api/cancellation-requests?search=arun&courseId=1&status=Pending&fromDate=2026-01-01&toDate=2026-12-31&pageNumber=1&pageSize=10
```

Supported filters: `search` (student name/email), `courseId`, `status`, `fromDate`, `toDate`. Results are ordered by request date; `sortOrder=desc` reverses the order.

Enum query values can be supplied by name, for example `Pending`, `Approved`, `Rejected`, `Active`, and `Cancelled`.

## 7. Enrollment and cancellation rules

- Only approved students can enroll.
- Inactive courses cannot receive new enrollments.
- Duplicate active enrollment in the same course is blocked.
- Enrollment checks current capacity inside a SQL Server `Serializable` transaction.
- Students cannot directly delete or cancel enrollments.
- Only one pending cancellation request is allowed per enrollment at a time.
- Admin approval changes the enrollment to `Cancelled`, freeing one seat.
- Admin rejection leaves the enrollment `Active`.
- A cancelled enrollment remains in history; a student may enroll again later.
- Course deletion is implemented as deactivation to preserve enrollment history.
- A student with enrollment history cannot be hard-deleted.

## 8. Important implementation notes

- DTOs are used for request/response contracts; password hashes are never returned.
- Passwords are stored as ASP.NET Core password hashes.
- JWT contains NameIdentifier, Name, Email and Role claims.
- Authorization is enforced at controller actions.
- EF Core performs filtering, sorting, counting and pagination in SQL.
- Global exception middleware returns Problem Details-style JSON responses.
- The API enables Angular development requests from `http://localhost:4200`.

## 9. Suggested Angular integration

1. Register and login screens.
2. Store the JWT using an appropriate client-side strategy.
3. Add an HTTP interceptor to attach `Authorization: Bearer <token>`.
4. Add route guards for `Administrator` and `Student`.
5. Build Admin pages for approvals, students, courses, enrollments, cancellation requests and dashboard.
6. Build Student pages for profile, course catalogue, enrollments and cancellation requests.

## 10. Production hardening before deployment

- Replace the development JWT key and seeded admin password.
- Move secrets to environment variables / a secret store.
- Restrict CORS to the deployed Angular origin.
- Configure HTTPS and production logging.
- Add automated integration tests, especially for concurrent enrollment and cancellation review.
- Consider email verification and account recovery if required by the deployment.
