using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Lab2.Requests.Token;
using MediatR;
using Microsoft.IdentityModel.Tokens;

namespace Lab2.Handlers;

public class TokenHandler(IConfiguration configuration) : IRequestHandler<CreateTokenRequest, string>
{
    public Task<string> Handle(CreateTokenRequest request, CancellationToken cancellationToken)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, request.Id.ToString()),
            new(ClaimTypes.Name, request.Username),
            new(ClaimTypes.Email, request.Email)
        };
        
        claims.AddRange(request.Roles.Select(role => new Claim(ClaimTypes.Role, role)));
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? string.Empty));
        if (key is null) throw new InvalidOperationException("Jwt:Key is missing in appsettings.json");
        
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var token = new JwtSecurityToken(
            configuration["Jwt:Issuer"],
            configuration["Jwt:Audience"],
            claims,
            expires: DateTime.Now.AddMinutes(15),
            signingCredentials: credentials);
        
        return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token)); 
    }
}