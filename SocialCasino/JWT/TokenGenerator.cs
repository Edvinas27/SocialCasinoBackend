using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace SocialCasino.JWT;
public class TokenGenerator
{
    private readonly IConfiguration _configuration;
    
    public TokenGenerator(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public string GenerateToken(int userId)
    {
        //main class to use for tokenhandling
        var tokenHandler = new JwtSecurityTokenHandler();
        
        //we take secret key 
        
        var key = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT key not found");
        
        //we convert it to bytes
        var keyBytes = Encoding.UTF8.GetBytes(key);
        
        // info that will be inside the claim
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, userId.ToString())
        };

        //we create token descriptor - info that will be shown in the token
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(20),
            //we sign the token with the key
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
        };

        //we create the token
        var token = tokenHandler.CreateToken(tokenDescriptor);
        
        //token has to be sent as string
        return tokenHandler.WriteToken(token);
    }
}