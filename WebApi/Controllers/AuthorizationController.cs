using Application.Dto;
using Application.Interfaces;
using Domain.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authorization;

namespace WebAPI.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IPasswordHasher<string> _passwordHasher;
        private readonly IMapper _mapper;
        public AuthorizationController(IUserService userService, IPasswordHasher<string> passwordHasher, IMapper mapper)
        {
            _userService = userService;
            _passwordHasher = passwordHasher;
            _mapper = mapper;
        }

        [SwaggerOperation(Summary = "Retrieves a specific user by unique id")]
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var uuser = _userService.GetUserById(id);
            if (uuser == null)
            {
                return NotFound();
            }
            return Ok(uuser);
        }

        [SwaggerOperation(Summary = "Checks if the user is currently logged in")]
        [HttpGet("Logged")]
        public IActionResult IsLogged()
        {
            var users = HttpContext.Session.GetString("user");

            if (users == null || _userService.GetUserByLogin(users).IsActive == false)
            {
                return NotFound();
            }
            return Ok();
        }

        [SwaggerOperation(Summary = "Create a new User")]
        [HttpPost]
        public IActionResult Register(RegisterDto newUser)
        {
            var uuser = _userService.AddNewUser(newUser);
            return Created($"api/users/{uuser.Login}", uuser);
        }

        [SwaggerOperation(Summary = "Update an existing intention")]
        [HttpPut("{id}")]
        public IActionResult Update(int id, UserDto updateUser)
        {
            _userService.UpdateUser(id, updateUser);
            return NoContent();
        }

        [SwaggerOperation(Summary = "Deactivate some guy's account")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _userService.SetUserNonActive(id);
            return NoContent();
        }

        [SwaggerOperation(Summary = "Login to user panel")]
        [HttpPost("Login")]
        public IActionResult Login(LoginDto user)
        {
            var uuser = _userService.GetUserByLogin(user.Login);
            var loginuser = _mapper.Map<User>(user);
            if (uuser != null)
            {
                if (_userService.IsUserActive(uuser.Login))
                {

                    var result = _passwordHasher.VerifyHashedPassword(uuser.Login, uuser.Hashed_Password, user.Password);

                    if (result == PasswordVerificationResult.Failed)
                    {
                        return NotFound();
                    }

                    var token = _userService.GenerateJwt(uuser);

                    if (token == null)
                        return BadRequest(new { message = "Username or password is incorrect" });

                    HttpContext.Session.SetString("user", user.Login);
                    return Ok(token);
                }
            }
            return NotFound();
        }
    }
}
