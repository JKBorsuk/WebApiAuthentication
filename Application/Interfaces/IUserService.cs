
namespace Application.Interfaces
{
    public interface IUserService
    {
        //LoginDto LoginUser(int id);
        UserDto GetUserById(int id);

        User GetUserByLogin(string login);
        User AddNewUser(RegisterDto newUser);
        void UpdateUser(int id, UserDto uuser);
        void SetUserNonActive(int id);
        bool IsUserActive(string login);

        string GenerateJwt(User dto);
    }
}
