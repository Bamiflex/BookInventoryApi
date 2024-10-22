using System;
using System.Threading.Tasks;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using SqlBasedBookAPI.data;
using SqlBasedBookAPI;
using Microsoft.EntityFrameworkCore;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;

    public AuthService(IUserRepository userRepository, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    public async Task<Result> AuthenticateAsync(string username, string password)
    {
        var user = await _userRepository.GetUserByUsernameAsync(username);
        if (user == null || !VerifyHmac512Password(password, user.PasswordHash, user.PasswordSalt))
        {
            return new Result { Success = false, Message = "Invalid username or password." };
        }

        // Generate JWT token
        var token = _tokenService.GenerateToken(user);
        return new Result { Success = true, Token = token, Message = "Login successful." };
    }

    public async Task<Result> RegisterAsync(UserRegistrationDto userDto)
    {
        var existingUser = await _userRepository.GetUserByEmailAsync(userDto.Email);
        if (existingUser != null)
        {
            return new Result { Success = false, Message = "A user with this email already exists." };
        }

        if (await _userRepository.UserExists(userDto.Username))
        {
            return new Result { Success = false, Message = "Username already exists." };
        }

        // Ensure the password is coming from userDto
        if (string.IsNullOrEmpty(userDto.Password))
        {
            return new Result { Success = false, Message = "Password cannot be empty." };
        }

        // Hash the password using HMAC512
        var (passwordHash, passwordSalt) = CreateHmac512PasswordHash(userDto.Password);

        var user = new User
        {
            FirstName = userDto.FirstName,
            LastName = userDto.LastName,
            Username = userDto.Username,
            Email = userDto.Email,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt
        };

        await _userRepository.CreateUserAsync(user);
        return new Result { Success = true, Message = "User registered successfully." };
    }

    private (byte[] passwordHash, byte[] passwordSalt) CreateHmac512PasswordHash(string password)
    {
        using (var hmac = new HMACSHA512())
        {
            byte[] passwordSalt = hmac.Key;
            byte[] passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return (passwordHash, passwordSalt);
        }
    }

    private bool VerifyHmac512Password(string password, byte[] storedHash, byte[] storedSalt)
    {
        using (var hmac = new HMACSHA512(storedSalt))
        {
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(storedHash);
        }
    }
}
