# ReadNest Backend - Docker Deployment

## Requirements

- Docker Desktop (Windows 10/11)
- Docker Compose

## Quick Start

```bash
cd Back-end/ReadNest-BE
docker-compose up -d --build
```

## Services

| Service | Port | URL |
|---------|-----|-----|
| API (HTTP) | 8080 | http://localhost:8080 |
| API (HTTPS) | 8443 | https://localhost:8443 |
| Redis | 6379 | localhost:6379 |
| Swagger | 8080/swagger | http://localhost:8080/swagger |

## Volume Mounts

| Host Path | Container Path | Description |
|-----------|----------------|-------------|
| `./data/sqlite` | `/app/data` | SQLite database |
| `./data/uploads/images` | `/app/Uploads/images` | Uploaded images |
| `./data/redis` | `/data` | Redis persistence |
| `./certs` | `/app/certs` | HTTPS certificates |

## First Setup

### 1. Database

The database is automatically created on first run via `EnsureCreated()`.

To use existing database:
```bash
# Place your existing readnest.db in ./data/sqlite/
# File must be named readnest.db
```

### 2. Images

Place images in `./data/uploads/images/`:

```
data/
└── uploads/
    └── images/
        ├── image1.jpg
        ├── image2.png
        └── ...
```

### 3. HTTPS Certificate

Generate self-signed certificate:

```powershell
# Run as Administrator
$cert = New-SelfSignedCertificate -DnsName "localhost" -CertStoreLocation "Cert:\CurrentUser\My" -NotAfter (Get-Date).AddYears(1)
$pwd = ConvertTo-SecureString -String "YourPasswordHere" -Force -AsPlainText
Export-PfxCertificate -Cert $cert -FilePath "certs/https.pfx" -Password $pwd
```

Or use the existing `certs/https.pfx` (password: `YourPasswordHere` for dev only).

## Database Migration

When model changes require database schema updates:

### 1. Add Migration

```bash
# From ReadNest-BE directory
docker compose run --rm -it api dotnet ef migrations add MigrationName
```

### 2. Apply Migration to Database

```bash
# Apply all pending migrations
docker compose run --rm -it api dotnet ef database update

# Or apply to specific migration
docker compose run --rm -it api dotnet ef database update MigrationName
```

### 3. List Migrations

```bash
docker compose run --rm -it api dotnet ef migrations list
```

### 4. Rollback (if needed)

```bash
# Rollback to previous migration
docker compose run --rm -it api dotnet ef database update PreviousMigrationName
```

---

**For new clone:**

```bash
# 1. Clone repo
git clone https://github.com/ngocanh0202/ReadNest.git

# 2. Navigate to backend
cd Back-end/ReadNest-BE

# 3. Apply migrations
docker compose up -d --build
docker compose run --rm -it api dotnet ef database update

# 4. Start services
docker compose up -d
```

## Configuration

Environment variables in `docker-compose.yml`:

| Variable | Default | Description |
|----------|---------|-------------|
| `ASPNETCORE_ENVIRONMENT` | Production | Runtime environment |
| `ASPNETCORE_URLS` | http://0.0.0.0:8080;https://0.0.0.0:8443 | Listen URLs |
| `ConnectionStrings__DefaultConnection` | Data Source=/app/data/readnest.db | Database path |
| `Cors__AllowAll` | true | Enable CORS for all origins |

## Common Commands

```bash
# Build and start
docker-compose up -d --build

# View logs
docker-compose logs -f api

# Stop
docker-compose down

# Restart
docker-compose restart

# Access container shell
docker exec -it readnest-api /bin/bash
```

## API Endpoints

- `GET /api/novels` - List novels
- `GET /api/novels/{id}` - Get novel details
- `POST /api/novels` - Create novel (auth required)
- `GET /api/volumes` - List volumes
- `GET /api/users` - List users (admin)
- `GET /Uploads/images/{filename}` - Get image

## Troubleshooting

### Port already in use

```bash
# Check what's using port 8080
netstat -ano | findstr :8080
```

### Database locked

```bash
# Stop all containers and retry
docker-compose down
docker-compose up -d
```

### Images not loading

Ensure volume mount is correct:
```bash
docker exec readnest-api ls -la /app/Uploads/images/
```

### Redis connection failed

```bash
# Check Redis is running
docker-compose ps
docker exec readnest-redis redis-cli ping
```

## Security Notes

- Change `ASPNETCORE_Kestrel__Certificates__Default__Password` in production
- Disable `Cors__AllowAll=true` in production
- Use proper SSL certificate in production
- Update JWT secret in `appsettings.json`