using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace SocialCasino.JWT;
/*Vps sito nesupratau XD, o bicas aiskino lyg cia legnviausias dalykas pasaulyje*/
public class TokenGenerator
{
    private readonly IConfiguration _configuration;
    
    public TokenGenerator(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public string GenerateToken(int userId)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT key not found");
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, userId.ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(20),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}