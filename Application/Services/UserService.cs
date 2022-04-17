using Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly AuthenticationSettings _authenticationSettings;
        private readonly IPasswordHasher<string> _passwordHasher;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, AuthenticationSettings authenticationSettings, IPasswordHasher<string> passwordHasher,  IMapper mapper)
        {
            _passwordHasher = passwordHasher;
            _authenticationSettings = authenticationSettings;
            _userRepository = userRepository;
            _mapper = mapper;
        }
        public User AddNewUser(RegisterDto newUser)
        {
            var uuser = _mapper.Map<User>(newUser);
            uuser.Hashed_Password = _passwordHasher.HashPassword(uuser.Login, newUser.Hashed_Password);

            uuser.IsActive = true;

            _userRepository.Add(uuser);
            return uuser;
        }

        public UserDto GetUserById(int id)
        {
            var uuser = _userRepository.GetById(id);
            return _mapper.Map<UserDto>(uuser);
        }

        public User GetUserByLogin(string login)
        {
            var uuser = _userRepository.GetByLogin(login);
            return uuser;
        }

        public void SetUserNonActive(int id)
        {
            var existingUser = _userRepository.GetById(id);
            existingUser.IsActive = false;
            _userRepository.Update(existingUser);
        }

        public bool IsUserActive(string login)
        {
            return _userRepository.GetByLogin(login).IsActive;
        }

        public void UpdateUser(int id, UserDto uuser)
        {
            var existingUser = _userRepository.GetById(id);
            var updatedUser = _mapper.Map(uuser, existingUser);

            existingUser.FirstName = updatedUser.FirstName;
            existingUser.LastName = updatedUser.LastName;

            _userRepository.Update(updatedUser);
        }
        public string GenerateJwt(User dto)
        {
            var user = _userRepository.GetByLogin(dto.Login);

            if (user is null)
            {
                return null;
            }

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationSettings.JwtKey));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(_authenticationSettings.JwtExpireDays);

            var token = new JwtSecurityToken(_authenticationSettings.JwtIssuer,
                _authenticationSettings.JwtIssuer,
                claims,
                expires: expires,
                signingCredentials: cred);

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }
    }
}
