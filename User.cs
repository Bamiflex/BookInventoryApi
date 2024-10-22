using System.ComponentModel.DataAnnotations;

public class User
{
    public int Id { get; set; }

    [Required]
    public string Username { get; set; }

    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }

    [Required]
    public byte[] PasswordHash { get; set; }
    public byte[] PasswordSalt { get; set; }
}
