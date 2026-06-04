# Unyo — Event Management Platform

A web platform for managing events, venues, tickets, and participant registrations. ASP.NET Core 8 backend with JWT authentication, Angular 19 standalone frontend.

---

## Architecture

```
┌─────────────────────────────────────────────────────────────┐
│  Browser @ localhost:4200                                   │
│  Angular 19 SPA (standalone components, signals, lazy load) │
└───────────────────┬─────────────────────────────────────────┘
                    │ HTTP + Authorization: Bearer <JWT>
                    │ (CORS configured on backend)
┌───────────────────▼─────────────────────────────────────────┐
│  ASP.NET Core 8 @ localhost:5024                            │
│  REST API  ·  JWT Bearer  ·  ASP.NET Identity               │
│  Entity Framework Core  ·  SQL Server LocalDB               │
└─────────────────────────────────────────────────────────────┘
```

---

## System Requirements

| Tool | Minimum Version |
|------|----------------|
| .NET SDK | 8.0 |
| Node.js | 18+ |
| SQL Server / LocalDB | any recent version |
| Angular CLI | 19+ (`npm i -g @angular/cli`) |

---

## Running the Backend

### 1. Configure the connection string

Open `Unyo/appsettings.Development.json` and verify:

```json
{
  "ConnectionStrings": {
    "Unyo": "Server=(localdb)\\MSSQLLocalDB;Database=UnyoDb;Trusted_Connection=True;"
  },
  "Jwt": {
    "Key": "SuperSecretKeyThatIsAtLeast32BytesLong!!!!",
    "Issuer": "Unyo",
    "Audience": "UnyoUsers",
    "ExpiresInMinutes": 60
  }
}
```

> If you use SQL Server Express instead of LocalDB, replace `(localdb)\\MSSQLLocalDB` with `localhost\\SQLEXPRESS`.

### 2. Database migration + seed

Migrations run **automatically** on startup via `SeedData.InitializeAsync`. No need to run `dotnet ef database update` manually.

```bash
cd Unyo
dotnet run
```

The backend starts at **http://localhost:5024** (or **https://localhost:7285**).  
Swagger UI available at: http://localhost:5024/swagger

### Seed accounts created automatically

| Role | Email | Password |
|------|-------|----------|
| Admin | `admin@unyo.com` | `Admin@123!` |
| Vendor | `concerts.srl@unyo.com` | `Vendor@123!` |

> Any account registered through `/register` receives the **User** role.

---

## Running the Frontend

### 1. Install dependencies

```bash
cd unyo-frontend
npm install
```

### 2. Configure the API URL

Check `src/environments/environment.ts`:

```ts
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5024/api'   // ← HTTP port from launchSettings.json
};
```

> The default port in `launchSettings.json` is **5024**. If running with HTTPS, change to `https://localhost:7285/api`.

### 3. Start the dev server

```bash
ng serve
```

The frontend starts at **http://localhost:4200**.

---

## Running Both Together (Dev)

Open **two terminals**:

```bash
# Terminal 1 — backend
cd Unyo
dotnet run

# Terminal 2 — frontend
cd unyo-frontend
ng serve
```

CORS is already configured in `Program.cs` for `http://localhost:4200`:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularDev", policy =>
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod());
});
// ...
app.UseCors("AllowAngularDev");
```

---

## Project Structure

### Backend — `Unyo/`

```
Unyo/
├── Controllers/API/        # REST controllers
│   ├── AuthController.cs       # POST /api/authapi/login, /register
│   ├── EventsController.cs     # GET/POST/PUT/DELETE /api/events
│   ├── VenuesController.cs     # GET/POST/PUT/DELETE /api/venues
│   ├── CategoriesController.cs
│   ├── TicketsController.cs
│   ├── EventRegistrationsController.cs
│   └── UsersController.cs      # Admin only
├── Models/                 # EF Core entities (User, Event, Venue, ...)
├── DTOs/                   # Records for request/response shapes
├── Services/               # Business logic (IEventService, ...)
├── Repositories/           # Repository Pattern + Unit of Work
├── Mappings/               # MapToDto / MapToEntity extension methods
├── Middleware/             # ExceptionHandlingMiddleware, LoggingMiddleware
├── Data/
│   ├── AppDbContext.cs
│   └── SeedData.cs
├── appsettings.json
├── appsettings.Development.json
└── Program.cs
```

### Frontend — `unyo-frontend/`

```
src/app/
├── core/
│   ├── models/             # TypeScript interfaces (mirror C# DTOs)
│   ├── services/           # AuthService + one service per API resource
│   ├── interceptors/       # auth.interceptor — attaches Bearer token automatically
│   └── guards/             # authGuard, adminGuard
├── features/               # Feature-based organization (Clean Architecture)
│   ├── auth/
│   │   ├── login/          # LoginComponent
│   │   └── register/       # RegisterComponent
│   ├── dashboard/          # DashboardComponent — homepage with stats
│   ├── events/
│   │   ├── event-list/     # Searchable event list
│   │   ├── event-detail/   # Event details + registration
│   │   └── event-form/     # Create / edit + ticket management
│   ├── venues/
│   │   ├── venue-list/
│   │   └── venue-form/
│   ├── categories/
│   │   └── category-list/  # Inline CRUD
│   ├── registrations/
│   │   └── registration-list/
│   └── users/              # Admin only
│       ├── user-list/
│       └── user-form/
└── shared/
    └── components/
        └── header/         # HeaderComponent with reactive navigation
```

---

## Features & Role Permissions

| Page / Action | Guest | User | Vendor | Admin |
|---|:---:|:---:|:---:|:---:|
| Browse events | ✅ | ✅ | ✅ | ✅ |
| Event details | ✅ | ✅ | ✅ | ✅ |
| Browse venues | ✅ | ✅ | ✅ | ✅ |
| Register for an event | ❌ | ✅ | ✅ | ✅ |
| Manage own registrations | ❌ | ✅ | ✅ | ✅ |
| Browse categories | ❌ | ✅ | ✅ | ✅ |
| Create / edit events | ❌ | ❌ | ✅ | ✅ |
| Manage tickets per event | ❌ | ❌ | ✅ | ✅ |
| Create / edit venues | ❌ | ❌ | ❌ | ✅ |
| Manage users | ❌ | ❌ | ❌ | ✅ |

---

## Frontend Routes

| Route | Component | Guard |
|-------|-----------|-------|
| `/` | `DashboardComponent` | — |
| `/login` | `LoginComponent` | — |
| `/register` | `RegisterComponent` | — |
| `/events` | `EventListComponent` | — |
| `/events/new` | `EventFormComponent` | `authGuard` |
| `/events/:id` | `EventDetailComponent` | — |
| `/events/:id/edit` | `EventFormComponent` | `authGuard` |
| `/venues` | `VenueListComponent` | — |
| `/venues/new` | `VenueFormComponent` | `authGuard + adminGuard` |
| `/venues/:id/edit` | `VenueFormComponent` | `authGuard + adminGuard` |
| `/categories` | `CategoryListComponent` | `authGuard` |
| `/registrations` | `RegistrationListComponent` | `authGuard` |
| `/users` | `UserListComponent` | `authGuard + adminGuard` |
| `/users/new` | `UserFormComponent` | `authGuard + adminGuard` |
| `/users/:id/edit` | `UserFormComponent` | `authGuard + adminGuard` |

---

## How Authentication Works

1. `POST /api/authapi/login` → backend returns `{ token, expiresIn }`
2. `AuthService` decodes the JWT (no external library), extracts `id`, `email`, `name`, `roles[]`
3. Token is stored in `localStorage`; current state is held in Angular **Signals**
4. `auth.interceptor.ts` (functional interceptor) automatically attaches `Authorization: Bearer <token>` to every HTTP request
5. On page refresh, `AuthService` reloads the token from `localStorage` and checks expiration
6. `authGuard` and `adminGuard` read the `isLoggedIn()` / `isAdmin()` signals and redirect if needed

---

## API Endpoints

| Method | URL | Required Role |
|--------|-----|---------------|
| POST | `/api/authapi/login` | — |
| POST | `/api/authapi/register` | — |
| GET | `/api/events` | — |
| GET | `/api/events/:id` | — |
| POST | `/api/events` | Admin, Vendor |
| PUT | `/api/events/:id` | Admin, Vendor (owner) |
| DELETE | `/api/events/:id` | Admin, Vendor (owner) |
| GET | `/api/venues` | — |
| POST/PUT/DELETE | `/api/venues` | Admin |
| GET | `/api/categories` | — |
| POST/DELETE | `/api/categories` | Admin |
| GET/POST/DELETE | `/api/tickets` | Admin, Vendor |
| GET/POST/DELETE | `/api/eventregistrations` | Auth |
| GET/POST/PUT/DELETE | `/api/users` | Admin |
