# Tasks: Add Registration, Lock Adult Features

## Phase 1: Backend - Model Updates

### Task 1.1: Update Novel Model
- File: `Back-end/ReadNest-Model/Models/Novel.cs`
- Add: `IsLocked`, `SharedUserIds`, `IsAdult` properties

### Task 1.2: Update NovelResponese DTO
- File: `Back-end/ReadNest-Model/Models/Responses/NovelResponese.cs`
- Add: `IsLocked`, `SharedUserIds`, `IsAdult` properties

### Task 1.3: Update DetailNovel DTO
- File: `Back-end/ReadNest-Model/Models/Dto/Novel/DetailNovel.cs`
- Add: `IsLocked`, `SharedUserIds`, `IsAdult` properties

---

## Phase 2: Backend - API

### Task 2.1: Add Signup Endpoint
- File: `Back-end/ReadNest-BE/Controllers/AuthController.cs`
- Add: `POST /api/auth/signup` (no auth required)
- Creates user with role-reader-002

### Task 2.2: Add Share Novel Endpoint
- File: `Back-end/ReadNest-BE/Controllers/NovelController.cs`
- Add: `POST /api/novel/{id}/share`
- Body: `{ "userId": "string" }`
- Requires: Author or Admin

### Task 2.3: Add Permission Check in GetById
- File: `Back-end/ReadNest-BE/Controllers/NovelController.cs`
- Update: `GET /api/novel/{id}` to check IsLocked + SharedUserIds
- Return 403 if not authorized

---

## Phase 3: Frontend - Services

### Task 3.1: Add Register to IAuthService
- File: `Front-end/ReadNest-FE/ReadNest-FE/Interfaces/IAuthService.cs`
- Add: `Task<Response<UserResponse>> Register(UserLogin userRegister)`

### Task 3.2: Implement Register in AuthService
- File: `Front-end/ReadNest-FE/ReadNest-FE/Services/Features/AuthService.cs`
- Implement: Calls `POST /api/auth/signup`

### Task 3.3: Add ShareNovel to INovelService
- File: `Front-end/ReadNest-FE/ReadNest-FE/Interfaces/INovelService.cs`
- Add: `Task<Response<bool>> ShareNovel(string novelId, string userId)`

### Task 3.4: Implement ShareNovel in NovelService
- File: `Front-end/ReadNest-FE/ReadNest-FE/Services/Features/NovelService.cs`
- Implement: Calls `POST /api/novel/{id}/share`

---

## Phase 4: Frontend - Pages & Components

### Task 4.1: Create Registration Page
- File: `Front-end/ReadNest-FE/ReadNest-FE/Pages/Register/Register.razor`
- Copy from: `Login.razor`
- Changes: Title "Sign up", calls Register(), redirect to Home on success

### Task 4.2: Update Login Link
- File: `Front-end/ReadNest-FE/ReadNest-FE/Pages/Login/Login.razor`
- Change: Line 112 - `<a href="#" class="font-bold">Sign up</a>` → `<a href="/register" class="font-bold">Sign up</a>`

### Task 4.3: Add Blur CSS
- File: `Front-end/ReadNest-FE/ReadNest-FE/vite-project/src/app.css`
- Add: `.blur-adult` classes

### Task 4.4: Update CardNovel Component
- File: `Front-end/ReadNest-FE/ReadNest-FE/Components/CardNovel/CardNovel.razor`
- Add: Blur class when IsAdult = true
- Hide locked novels for non-author/non-admin users

### Task 4.5: Update Novel Detail Page
- File: `Front-end/ReadNest-FE/ReadNest-FE/Pages/Novel/Novel.razor`
- Add: Handle 403 response
- Display: "You do not have permission to read this novel"

---

## Implementation Order

1. Backend Model (Tasks 1.1 → 1.3)
2. Backend API (Tasks 2.1 → 2.3)
3. Frontend Services (Tasks 3.1 → 3.4)
4. Frontend UI (Tasks 4.1 → 4.5)

---

## Notes

- Database: Add columns via EF Core migration or manually
- Test each feature after implementation
- Run lint/typecheck after each phase