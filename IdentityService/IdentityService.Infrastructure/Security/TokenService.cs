using IdentityService.Domain.Constants;
using IdentityService.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IdentityService.Infrastructure.ServicesJWT;

public class TokenService(IConfiguration configuration)
{
    private readonly IConfiguration _configuration = configuration;

    public string GenerateToken(string username, TipoRole role, long tenancyId, bool primeiroAcesso, TipoUserAtivo userAtivo, TipoTenancyAtivo tenancyAtiva)
    {
        var claims = GerarClaims(username, role, primeiroAcesso, userAtivo, tenancyAtiva);
        claims.Add(new Claim(AuthClaims.TenancyId, tenancyId.ToString()));

        var tokenDescriptor = GerarTokenDescriptor(_configuration, claims);

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private static List<Claim> GerarClaims(string username, TipoRole role, bool primeiroAcesso, TipoUserAtivo userAtivo, TipoTenancyAtivo TenancyAtiva)
    {
        return
        [
            new(ClaimTypes.Name, username),
            new(ClaimTypes.Role, role.ToString()),
            new(AuthClaims.PrimeiroAcesso, primeiroAcesso.ToString().ToLower()),
            new(AuthClaims.StatusAtivo, userAtivo.ToString()),
            new(AuthClaims.TenancyAtiva, TenancyAtiva.ToString())
        ];
    }

    private static SecurityTokenDescriptor GerarTokenDescriptor(IConfiguration configuration, List<Claim> claims)
    {
        var expireMinutes = double.TryParse(configuration["Jwt:ExpireMinutes"], out var value) && value > 0 ? value : 15;
        var issuedAt = DateTime.UtcNow;

        return new SecurityTokenDescriptor
        {
            NotBefore = issuedAt,
            Subject = new ClaimsIdentity(claims),
            Expires = issuedAt.AddMinutes(expireMinutes),
            Audience = configuration["Jwt:Audience"],
            Issuer = configuration["Jwt:Issuer"],
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)),
                SecurityAlgorithms.HmacSha256Signature)
        };
    }
}
