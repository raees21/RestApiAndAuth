using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using RestAPI.Models;

namespace RestAPI.Common.DevUtils;

public static class AccessTokenGenerator
{
    public static AuthResponse GenerateJWT(FakeAuthRequest fakeAuthRequest, string secret)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret));
        
        var tokenHandler = new JwtSecurityTokenHandler();
        
        Guid userId = fakeAuthRequest.UserId ?? Guid.NewGuid();
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim("UserId", userId.ToString()),
                new(ClaimTypes.Role, fakeAuthRequest.Type.ToString())
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
        };
        var generatedToken = tokenHandler.CreateToken(tokenDescriptor);
        var accessToken = tokenHandler.WriteToken(generatedToken);

        return new AuthResponse
        {
            AccessToken = accessToken,
            UserId = userId
        };
    }
}