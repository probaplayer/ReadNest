# ReadNest Agent Instructions

## Project Structure

```
ReadNest/
├── Front-end/ReadNest-FE/ReadNest-FE/    # Blazor WASM frontend (.NET 9.0)
│   ├── vite-project/                     # Vite + TailwindCSS + TypeScript build
│   │   └── package.json                   # Frontend npm scripts
│   └── wwwroot/js/                       # Compiled frontend assets (generated)
├── Back-end/ReadNest-BE/                 # .NET 8 backend API
│   ├── ReadNest-BE/                      # Main API project
│   │   └── Program.cs                    # Entry point, DI, middleware
│   └── ReadNest-Model/                   # Shared models
└── .github/workflows/deploy.yml          # GitHub Pages CI/CD
```

## Developer Commands

### Frontend
```bash
# Navigate to frontend
cd Front-end/ReadNest-FE/ReadNest-FE

# Run dev server (requires npm + .NET)
cd vite-project && npm run dev

# Build frontend assets (TypeScript + Vite)
cd vite-project && npm run build

# Run Blazor app
dotnet run --project ReadNest_FE.csproj

# Build and publish for GitHub Pages
dotnet publish ReadNest-FE.sln -c Release -p:UseAppHost=false
```

### Backend
```bash
# Navigate to backend
cd Back-end/ReadNest-BE/ReadNest-BE

# Run API
dotnet run

# Database auto-creates on startup via EnsureCreated()
```

### Docker
```bash
cd Back-end/ReadNest-BE
docker-compose up --build
# API: http://localhost:5000
# Redis: localhost:6379
```

## Key Technical Details

- **Frontend**: Blazor WASM with Vite bundling TailwindCSS/TypeScript to `wwwroot/js/`
- **Backend**: .NET 8, EF Core with SQLite, optional Redis for caching
- **Database**: SQLite at `data/sqlite/` (auto-created via `EnsureCreated()`)
- **API Docs**: Swagger at `/swagger` (dev only)
- **Rate Limiting**: Auth endpoints limited to 10 req/5min per IP; write ops 5 req/min per user
- **CORS**: Dev = all origins; Prod = `https://ngocanh0202.github.io` only
- **Build Output Path**: Frontend builds to `wwwroot/js/assets/main.js` and `wwwroot/js/assets/style.css`

## Common Pitfalls

- Frontend `.csproj` targets .NET 9.0, backend targets .NET 8.0 — ensure both SDKs installed
- Vite outputs to `../wwwroot/js` relative to `vite-project/`
- Run `npm run build` before publishing to ensure JS/CSS are in wwwroot
- Backend Program.cs auto-creates SQLite db; delete `data/` folder to reset

## Build Order

1. `cd vite-project && npm run build` — compiles TypeScript + Tailwind
2. `dotnet build` in frontend — embeds wwwroot content
3. Publish as needed

## External Services

- **AI**: Google Gemini API (configured via appsettings.json)
- **Deployment**: GitHub Pages (via `.github/workflows/deploy.yml`)