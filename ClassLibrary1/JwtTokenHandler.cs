using JwtAuthenticationManager.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JwtAuthenticationManager
{
    public class JwtTokenHandler
    {
        public const string JWT_SECURITY_KEY = "sdlkdsfkdlnf344qKJN4sd4_ThisIsA32ByteStrongKey1234";
        private const int JWT_VALIDITY_MIN = 20;
        private readonly List<UserAccounts> _userAccounts;

        public JwtTokenHandler()
        {
            _userAccounts = new List<UserAccounts>
            {
                new UserAccounts{ UserName="admin1", Password="admin123", Role="Admin"},
                new UserAccounts{ UserName="manager1", Password="manager123", Role="manager"},
                new UserAccounts{ UserName="cust1", Password="cust123", Role="User"}
            };
        }
        public AuthenticationResponse? GenerateJwtToken(AuthenticationRequest authenticationRequest)
        {
            if (string.IsNullOrWhiteSpace(authenticationRequest.UserName) || string.IsNullOrWhiteSpace(authenticationRequest.Password))
                return null;
            
            /* Validation */
            var userAccount = _userAccounts.Where(x => x.UserName == authenticationRequest.UserName && x.Password == authenticationRequest.Password).FirstOrDefault();
            if (userAccount == null) return null;

            var tokenExpiryTimeStamp = DateTime.Now.AddMinutes(JWT_VALIDITY_MIN);
            var tokenKey = Encoding.ASCII.GetBytes(JWT_SECURITY_KEY);
            var claimsIdentity = new ClaimsIdentity(new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Name, authenticationRequest.UserName),
                new Claim("Role", userAccount.Role)
            });

            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(tokenKey),
                SecurityAlgorithms.HmacSha256Signature);

            var securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Expires = tokenExpiryTimeStamp,
                SigningCredentials = signingCredentials
            };

            var jwtSecurityHandler = new JwtSecurityTokenHandler();
            var securityToken = jwtSecurityHandler.CreateToken(securityTokenDescriptor);
            var token = jwtSecurityHandler.WriteToken(securityToken);

            return new AuthenticationResponse
            {
                UserName = authenticationRequest.UserName,
                ExpiresIn = (int)tokenExpiryTimeStamp.Subtract(DateTime.Now).TotalSeconds,
                JwtToken = token
            };
        }
    }
}
