﻿using System;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Prime.Account.API.Helpers;

namespace Prime.Account.API.OAuth
{
    public class CustomJwtFormat : ISecureDataFormat<AuthenticationTicket>
    {
        private static readonly byte[] _secret = TextEncodings.Base64Url.Decode(ConfigHelper.GetSecretKey);
        private static readonly string _audience = ConfigHelper.GetAudience;
        private readonly string _issuer;

        public CustomJwtFormat(string issuer)
        {
            _issuer = issuer;
        }

        public string Protect(AuthenticationTicket data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            var securityKey = new SymmetricSecurityKey(_secret);
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            var issued = data.Properties.IssuedUtc;
            var expires = data.Properties.ExpiresUtc;

            var token = new JwtSecurityTokenHandler().WriteToken(
                new JwtSecurityToken(_issuer, _audience, data.Identity.Claims, 
                                    issued.Value.UtcDateTime, 
                                    expires.Value.UtcDateTime, signingCredentials));
            return token;
        }

        public AuthenticationTicket Unprotect(string protectedText)
        {
            throw new NotImplementedException();
        }
    }
}