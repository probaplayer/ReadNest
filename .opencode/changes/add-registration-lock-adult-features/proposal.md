# Proposal: Add Registration, Lock Adult Features

## Summary

Add three new features to ReadNest:

1. **User Registration API** - New public endpoint `/api/auth/signup` for self-registration
2. **Novel Locking** - ability to lock novels and share with specific users
3. **Adult Content Blur** - blur adult novel covers on hover

## Why

1. **Registration**: Currently users cannot self-register. They need admin to create accounts. This blocks user growth.
2. **Locking**: Authors need to control access to their novels - share only with specific users while keeping private from others.
3. **Adult Blur**: Protect underaged users from accidental exposure to adult content.

## Scope

### In Scope
- Backend: Add signup endpoint, add IsLocked/SharedUserIds/IsAdult to Novel model
- Backend: API endpoints for sharing locked novels
- Backend: Permission check returning 403 for unauthorized access
- Frontend: Registration page
- Frontend: CardNovel component with blur for adult content

### Out of Scope
- Password reset email flow
- Email verification
- OAuth login

## Success Criteria

1. New user can register via `/api/auth/signup` and login immediately
2. Locked novels do not appear in listings - only accessible via direct link for authorized users
3. Adult novel covers are blurred until user hovers over them