using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ProyectoCorporativoMvc.Services;

public class ServicioJwt : IServicioJwt
{
    private readonly IConfiguration _configuration;

    public ServicioJwt(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerarToken(IEnumerable<Claim> claims)
    {
        var key = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("No se configuró Jwt:Key.");
        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];
        var expireHours = int.TryParse(_configuration["Jwt:ExpireHours"], out var horas) ? horas : 8;

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(expireHours),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
