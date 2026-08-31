# Garage Management System

A proof-of-concept garage management app with an ASP.NET Core Web API backend and Angular frontend. The UI uses a dashboard layout with a dark sidebar, top navigation bar, and card-based content area.

## Project structure

- `api/` — ASP.NET Core Web API (`GarageManagement.Api`)
- `ui/` — Angular frontend

## Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) (net10.0)
- [Node.js](https://nodejs.org/) (v20+ recommended)

## Run locally

Start the API and UI in separate terminals.

### 1. Start the API

```bash
dotnet run --project api/GarageManagement.Api
```

The API runs at `http://localhost:5034` (and `https://localhost:7249`).

### 2. Start the UI

```bash
cd ui
npm install
npm start
```

The Angular dev server runs at `http://localhost:4200` and proxies `/api` requests to the backend.

## Verify the integration

Open `http://localhost:4200`. The dashboard loads sample forecast data from `GET /api/weatherforecast` to confirm the UI and API are connected.
