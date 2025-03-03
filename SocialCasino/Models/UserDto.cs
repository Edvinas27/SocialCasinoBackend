namespace SocialCasino.Models;

public class UserDto
{
    //required info for creating a new user
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}