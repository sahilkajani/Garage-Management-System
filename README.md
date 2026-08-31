# Garage Management System

A proof-of-concept garage management application for tracking workshop jobs from intake through completion. It consists of an ASP.NET Core Web API backend with a SQLite database and an Angular frontend with a dashboard-style layout (dark sidebar, top navigation, and card-based content).

## Features

### Create jobs
Capture new work from the **Create Job** page with:

- **Job details** — description, when the issue occurs (condition), mileage, and priority (High / Medium / Low)
- **Vehicle information** — registration, make, and model
- **Customer** — customer name
- **Assignment** — optional service adviser at creation time

New jobs are created with status **Unscheduled**.

### View and manage jobs
- **Job Status** — browse all jobs in a sortable list showing vehicle, customer, assignee, and dates
- **Job detail** — open any job to view and edit full information, including status, scheduled date, and completed date

### Schedule and assign work
- Set job status to **Unscheduled**, **Scheduled**, or **Completed**
- Pick a **scheduled date** when planning work
- **Assign** jobs to a service adviser
- Record a **completed date** when work is finished

### Dashboard
Overview metrics at a glance:

- Unscheduled jobs
- Scheduled jobs
- Completed jobs

Counts default to **0** when there is no data in that category.

## Project structure

- `api/` — ASP.NET Core Web API (`GarageManagement.Api`) with raw SQL via SQLite (`garage.db`)
- `ui/` — Angular frontend (standalone components, zoneless change detection)

## API

Jobs are exposed under `/api/jobs`:

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/jobs` | List all jobs (newest first) |
| `GET` | `/api/jobs/{id}` | Get a single job |
| `POST` | `/api/jobs` | Create a job |
| `PUT` | `/api/jobs/{id}` | Update a job |

Swagger is available in Development at `/swagger` when the API is running.

## Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) (net10.0)
- [Node.js](https://nodejs.org/) (v20+ recommended)

## Run locally

Start the API and UI in separate terminals.

### 1. Start the API

```bash
dotnet run --project api/GarageManagement.Api --launch-profile http
```

The API runs at `http://localhost:5287` (HTTPS profile also available on `https://localhost:7055`).

### 2. Start the UI

```bash
cd ui
npm install
npm start
```

The Angular dev server runs at `http://localhost:4200` and proxies `/api` requests to the backend.

## Verify the integration

1. Open `http://localhost:4200`
2. Use **Create Job** in the sidebar to add a job with vehicle and customer details
3. Open **Job Status** to see the job in the list, then click through to the detail page
4. Update status, scheduled date, and assignee, then check the **Dashboard** for updated counts

## Tests

Run API unit tests from the repository root:

```bash
dotnet test api/GarageManagement.Api.UnitTests
```
