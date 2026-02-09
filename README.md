# b-shrpr

**Be sharper.** Look your best for recruiters.

## What’s here

- **User model** (for the db): `api/Models/User.cs` — `Id`, `Name`, `Username`, `Email`, `CreatedAt`.
- **API**: GET/POST `/api/users` (in-memory store; users returned in alphabetical order by name).
- **Admin frontend**: Home page lists users (alphabetical) and an “Add user” button; add form at `/add` (name, username, email).

## Run everything

From the repo root:

```bash
npm install
npm run dev
```

- API: **http://localhost:5001**
- Web: **http://localhost:5173**

## Run separately

- **API only:** `npm run dev:api` (or `cd api && dotnet run`)
- **Web only:** `npm run dev:web` (or `cd web && npm run dev`)

## Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) (e.g. `brew install --cask dotnet-sdk` on macOS)
- Node.js 18+
