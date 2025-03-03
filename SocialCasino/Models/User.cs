namespace SocialCasino.Models;

public class User
{
    //info that will be stored in the database about the user
    public int UserId { get; set; } = 0;
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public uint Balance { get; set; } = 0;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}