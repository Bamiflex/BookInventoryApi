using SqlBasedBookAPI.data;

public interface IAuthService
{
    Task<Result> RegisterAsync(UserRegistrationDto userDto);
    Task<Result> AuthenticateAsync(string username, string password);
}
