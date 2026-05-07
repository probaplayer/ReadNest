using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using ReadNest_BE.Handlers;
using ReadNest_BE.Interfaces.Repositories;
using ReadNest_BE.Services;
using ReadNest_Enums;
using ReadNest_Models;

namespace ReadNest_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly JwtService _jwtService;
        public AuthController(IUserRepository userRepository, IUserRoleRepository userRoleRepository , JwtService jwtService)
        {
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            _jwtService = jwtService;
        }
        [HttpPost("login")]
        [EnableRateLimiting("auth")]
        public async Task<IActionResult> Login([FromBody] UserLogin userLogin)
        {
            UserResponse? userResponse;
            try
            {
                userResponse = await _jwtService.Authenticate(userLogin);
                return Ok(new Response<UserResponse>(userResponse, "Login successfully", true));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("signup")]
        [EnableRateLimiting("auth")]
        public async Task<IActionResult> Signup([FromBody] UserLogin userRegister)
        {
            if (string.IsNullOrWhiteSpace(userRegister.UserName) || string.IsNullOrWhiteSpace(userRegister.Password))
                return BadRequest("Username or password is empty");

            var isUserExist = await _userRepository.IsUserExist(userRegister.UserName);
            if (isUserExist)
                return BadRequest("Username already exists");

            try
            {
                var user = new User
                {
                    UserName = userRegister.UserName,
                    PasswordHash = PasswordHandler.HashPassword(userRegister.Password),
                };

                var newUser = await _userRepository.Create(user);
                await _userRoleRepository.Create(new UserRole
                {
                    UserId = newUser.Id,
                    RoleId = "role-reader-002"
                });

                var userResponse = await _jwtService.Authenticate(userRegister);
                return Ok(new Response<UserResponse>(userResponse, "Signup successful", true));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Register")]
        [Authorize]
        public async Task<IActionResult> Register([FromBody] UserLogin userRegister)
        {
            string? header = Request.Headers["Authorization"];
            if (string.IsNullOrWhiteSpace(header) || !header.StartsWith("Bearer "))
                return BadRequest("Unauthorized");
            _jwtService.Token = header;
            string? roles = _jwtService.GetRoleFromToken();
            if (string.IsNullOrWhiteSpace(roles))
                return BadRequest("Unauthorized");
            var roleList = roles.Split(';');
            if (!roleList.Contains(RoleType.ADMIN.ToString()))
                return BadRequest("Forbidden");

            if (string.IsNullOrWhiteSpace(userRegister.UserName) || string.IsNullOrWhiteSpace(userRegister.Password))
                return BadRequest("Username or password is empty");

            var isUserExist = await _userRepository.IsUserExist(userRegister.UserName);
            if (isUserExist)
                return BadRequest("Username already exists");
            
            var user = new User
            {
                UserName = userRegister.UserName,
                PasswordHash = PasswordHandler.HashPassword(userRegister.Password),
            };


            UserResponse? userResponse = user.Adapt<UserResponse>();

            try
            {
                var newUser = await _userRepository.Create(user);
                await _userRoleRepository.Create(new UserRole
                {
                    UserId = newUser.Id,
                    RoleId = "role-reader-002"
                });

                userResponse = await _jwtService.Authenticate(userRegister);

                return Ok(new Response<UserResponse>(userResponse, "Register successfully", true));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] UserChangePassword request)
        {
            if (request == null ||
                string.IsNullOrWhiteSpace(request.UserName) ||
                string.IsNullOrWhiteSpace(request.OldPassword) ||
                string.IsNullOrWhiteSpace(request.NewPassword))
            {
                return BadRequest("All fields are required.");
            }

            string? header = Request.Headers["Authorization"];
            if (string.IsNullOrWhiteSpace(header) || !header.StartsWith("Bearer "))
                return Unauthorized("Missing or invalid authorization header.");

            _jwtService.Token = header;
            string? userId = _jwtService.GetUserIdFromToken();

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized("Invalid token.");

            var user = await _userRepository.GetById(userId);
            if (user == null)
                return NotFound("User not found.");

            if (!user.UserName!.Equals(request.UserName, StringComparison.OrdinalIgnoreCase))
                return BadRequest("Invalid username.");

            if (!PasswordHandler.VerifyPassword(request.OldPassword, user.PasswordHash!))
                return BadRequest("Incorrect old password.");

            try
            {
                user.PasswordHash = PasswordHandler.HashPassword(request.NewPassword);
                await _userRepository.Update(user); 
                return Ok(new Response<bool>(true, "Password changed successfully.", true));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"An error occurred while changing password: {ex.Message}");
            }
        }

        //[HttpDelete("Delete")]
        //[Authorize]
        //public async Task<IActionResult> Delete([FromQuery] string userId)
        //{
        //    string? header = Request.Headers["Authorization"];
        //    if (string.IsNullOrWhiteSpace(header) || !header.StartsWith("Bearer "))
        //        return BadRequest(new Response<UserResponse>(null, "Unauthorized", false));
        //    _jwtService.Token = header;
        //    string? roles = _jwtService.GetRoleFromToken();
        //    if (string.IsNullOrWhiteSpace(roles))
        //        return BadRequest(new Response<UserResponse>(null, "Unauthorized", false));
        //    var roleList = roles.Split(';');
        //    if (!roleList.Contains(RoleType.ADMIN.ToString()))
        //        return BadRequest(new Response<UserResponse>(null, "Forbidden", false));
        //    if (string.IsNullOrWhiteSpace(userId))
        //        return BadRequest(new Response<UserResponse>(null, "User id is empty", false));
        //    var isDeleted = false;
        //    try
        //    {
        //        isDeleted = await _userRepository.Delete(userId);
        //        if (!isDeleted)
        //            return BadRequest("Delete failed");
        //        return Ok(new Response<UserResponse>(null, "Delete Success", true));
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            try
            {
                var result = await _jwtService.RefreshToken(request.AccessToken, request.RefreshToken);
                return Ok(new Response<UserResponse>(result, "refresh successfully", true));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
