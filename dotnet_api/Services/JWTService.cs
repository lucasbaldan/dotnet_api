using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace dotnet_api.Services;

public class JWTService : IJWTService
{
    public string GenerateRefreshToken()
    {
        throw new NotImplementedException();
    }

    public JwtSecurityToken GerarToken(IEnumerable<Claim> claims, IConfiguration _config)
    {
        var key = _config.GetSection("JWT").GetValue<string>("SecretKey") ?? throw new InvalidOperationException("ERRO CRÍTICO, chave de acesso secreta inválida");

        var credentialSign = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256Signature);

        var conteudoToken = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_config.GetSection("JWT").GetValue<double>("AccessExpiration")),
            Audience = _config.GetSection("JWT").GetValue<string>("Audience"),
            Issuer = _config.GetSection("JWT").GetValue<string>("Issuer"),
            SigningCredentials = credentialSign
        };

        var tokenClass = new JwtSecurityTokenHandler();
        var token = tokenClass.CreateJwtSecurityToken(conteudoToken);
        return token;

    }

    public ClaimsPrincipal GetDataFromExpiredToken(string token, IConfiguration _config)
    {
        var key = _config.GetSection("JWT").GetValue<string>("SecretKey") ?? throw new InvalidOperationException("ERRO CRÍTICO, chave de acesso secreta inválida");

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _config.GetSection("JWT").GetValue<string>("Issuer"),
            ValidAudience = _config.GetSection("JWT").GetValue<string>("Audience"),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var dadosToken = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

        if(securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Token de autenticação inválido");
        }

        return dadosToken;
    }
}

public interface IJWTService
{
    JwtSecurityToken GerarToken(IEnumerable<Claim> claims, IConfiguration _config);

    string GenerateRefreshToken();

    ClaimsPrincipal GetDataFromExpiredToken(string token, IConfiguration _config);
}
