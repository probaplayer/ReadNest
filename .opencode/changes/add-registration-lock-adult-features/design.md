# Design: Add Registration, Lock Adult Features

## 1. User Registration

### Backend Changes

**New Endpoint**: `POST /api/auth/signup`

```
Request:
{
  "userName": "string (required, min 3 chars)",
  "password": "string (required)"
}

Response (201):
{
  "data": {
    "id": "string",
    "userName": "string",
    "token": "string",
    "refreshToken": "string",
    "hasContributePermission": false
  },
  "message": "Signup successful",
  "success": true
}

Response (400): "Username already exists" | "Username or password is empty"
```

- No authorization required
- Creates user with default role `role-reader-002`
- Returns JWT token immediately (same as login flow)

### Frontend Changes

**New Page**: `/register` (copy from Login.razor)

- Same UI as Login page
- Calls `AuthService.Register()` instead of `Login()`
- On success: redirect to Home + save token

---

## 2. Novel Locking

### Database Changes

**Table: Novel**

New columns:
```sql
IsLocked bit DEFAULT 0
SharedUserIds nvarchar(MAX) -- comma-separated: "user-id-1,user-id-2"
```

### Model Changes

**Novel.cs**
```csharp
public bool? IsLocked { get; set; }
public string? SharedUserIds { get; set; }
```

**NovelResponese.cs**
```csharp
public bool? IsLocked { get; set; }
public string? SharedUserIds { get; set; }
```

**DetailNovel.cs**
```csharp
public bool? IsLocked { get; set; }
public string? SharedUserIds { get; set; }
```

### Backend API

**POST /api/novel/{id}/share**

- Add specific user to shared list
- Request: `{ "userId": "string" }`
- Requires: Author of novel OR Admin
- Adds userId to SharedUserIds (comma-separated)

**GET /api/novel/{id}**

- Check permission before returning data
- If IsLocked == true:
  - Allow if: User is Author (CreateBy == currentUserId) OR User is in SharedUserIds OR User is Admin
  - Else: Return 403 "You do not have permission to read this novel"

### Frontend Changes

**INovelService.cs** + **NovelService.cs**
```csharp
Task<Response<bool>> ShareNovel(string novelId, string userId)
```

**CardNovel.razor**
```razor
@* Don't show locked novels in list *@
@* Only show if user is author or admin *@
```

**Novel.razor**
- Handle 403 response
- Display: "You do not have permission to read this novel"

---

## 3. Adult Content Blur

### Database Changes

**Table: Novel**

New column:
```sql
IsAdult bit DEFAULT 0
```

### Model Changes

**Novel.cs**
```csharp
public bool? IsAdult { get; set; }
```

**NovelResponese.cs**
```csharp
public bool? IsAdult { get; set; }
```

**DetailNovel.cs**
```csharp
public bool? IsAdult { get; set; }
```

### Frontend Changes

**CSS (app.css)**
```css
.novel-card .blur-adult {
  filter: blur(8px);
  transition: filter 0.3s ease;
}
.novel-card:hover .blur-adult {
  filter: blur(0);
}
```

**CardNovel.razor**
```razor
<img src="@novel.ImageUrl" alt="@novel.Id" class="@(novel.IsAdult == true ? "blur-adult" : "")" />
```

---

## File Changes Summary

### Backend
| File | Change |
|------|--------|
| Models/Novel.cs | Add IsLocked, SharedUserIds, IsAdult |
| Models/NovelResponese.cs | Add IsLocked, SharedUserIds, IsAdult |
| Models/Dto/Novel/DetailNovel.cs | Add IsLocked, SharedUserIds, IsAdult |
| Controllers/AuthController.cs | Add POST /signup endpoint |
| Controllers/NovelController.cs | Add POST /{id}/share, update GET /{id} permission check |

### Frontend
| File | Change |
|------|--------|
| Pages/Register/Register.razor | New registration page |
| Services/Features/IAuthService.cs | Add Register method |
| Services/Features/AuthService.cs | Implement Register |
| Services/Features/INovelService.cs | Add ShareNovel method |
| Services/Features/NovelService.cs | Implement ShareNovel |
| Components/CardNovel/CardNovel.razor | Add blur for adult content |
| Pages/Novel/Novel.razor | Handle 403 response |
| Pages/Login/Login.razor | Update "Sign up" link |
| vite-project/src/app.css | Add blur styles |