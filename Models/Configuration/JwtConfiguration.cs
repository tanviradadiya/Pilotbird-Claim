using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;

namespace Models.Configuration
{
    public class JwtConfiguration
    {
        public bool ShouldValidateIssuer { get; set; }

        public bool ShouldValidateAudience { get; set; }

        public string ValidIssuer { get; set; }

        public string ValidAudience { get; set; }

        public string IssuerSigningKey { get; set; }

        public string SecurityAlgorithmForSigningCredential { get; set; } = SecurityAlgorithms.HmacSha256;

        public int NumberOfSecondsUntilExpiration { get; set; }

        private byte[] GetIssuerSigningKeyBytes() =>
           string.IsNullOrWhiteSpace(IssuerSigningKey)
                ? Enumerable.Empty<byte>().ToArray()
                : Encoding.UTF8.GetBytes(IssuerSigningKey);

        public SymmetricSecurityKey GenerateSymmetricSecurityKey()
        {
            var issuerSigningKeyBytes = GetIssuerSigningKeyBytes();

            return issuerSigningKeyBytes.Length > 0
                ? new SymmetricSecurityKey(issuerSigningKeyBytes)
                : null;
        }

        public SigningCredentials GenerateSigningCredentials()
        {
            var symmeticSecurityKey = GenerateSymmetricSecurityKey();

            return symmeticSecurityKey != null
                ? new SigningCredentials(symmeticSecurityKey, SecurityAlgorithmForSigningCredential)
                : null;
        }

        public JwtSecurityToken ParseJwtSecurityTokenFromText(string jwtTokenText)
        {
            if (string.IsNullOrWhiteSpace(jwtTokenText))
            {
                throw new ArgumentException(nameof(jwtTokenText));
            }

            return new JwtSecurityTokenHandler().ReadJwtToken(jwtTokenText);
        }
    }
}
