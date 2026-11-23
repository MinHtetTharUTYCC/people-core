using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PeopleCore.Infrastructure.Security;

public class JwtTokenService : PeopleCore.Application.Identity.Services.IJwtTokenService
{
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _expirationMinutes;

    public JwtTokenService(IConfiguration configuration)
    {
        var jwt = configuration.GetSection("Jwt");
        _secretKey = jwt.GetValue<string>("SecretKey") ?? throw new InvalidOperationException("JWT SecretKey not configured");
        _issuer = jwt.GetValue<string>("Issuer") ?? throw new InvalidOperationException("JWT Issuer not configured");
        _audience = jwt.GetValue<string>("Audience") ?? throw new InvalidOperationException("JWT Audience not configured");
        _expirationMinutes = jwt.GetValue<int?>("ExpirationMinutes") ?? 60;
    }

    public string GenerateToken(string userId, string email, IEnumerable<string> roles)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId),
            new(ClaimTypes.Email, email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_expirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
