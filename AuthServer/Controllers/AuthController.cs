using System.Globalization;
using System.Net;
using System.Net.Http;

namespace AuthServer.Controllers
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;

    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]")]

    public class AuthController : Controller
    {
        private readonly IOptions<Audience> _settings;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="settings"></param>
        public AuthController(IOptions<Audience> settings)
        {
            this._settings = settings;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Get(string name, string pwd)
        {
            HttpClient client = new HttpClient {BaseAddress = new Uri("http://localhost:62793")};
            client.DefaultRequestHeaders.Clear();
            var res2 = client.GetAsync($"/api/Customer?subscriberName={name}&pwd={pwd}").Result;
            if (res2.StatusCode == HttpStatusCode.NotFound)
                return Json("");
            var now = DateTime.UtcNow;

            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, name),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, now.ToUniversalTime().ToString(CultureInfo.InvariantCulture),
                    ClaimValueTypes.Integer64)
            };

            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_settings.Value.Secret));
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                ValidateIssuer = true,
                ValidIssuer = _settings.Value.Iss,
                ValidateAudience = true,
                ValidAudience = _settings.Value.Aud,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                RequireExpirationTime = true,
            };

            var jwt = new JwtSecurityToken(
                issuer: _settings.Value.Iss,
                audience: _settings.Value.Aud,
                claims: claims,
                notBefore: now,
                expires: now.Add(TimeSpan.FromMinutes(50)),
                signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
            );
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            var responseJson = new
            {
                access_token = encodedJwt,
                expires_in = (int) TimeSpan.FromMinutes(50).TotalSeconds
            };

            return Json(responseJson);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Audience
    {
        /// <summary>
        /// 
        /// </summary>
        public string Secret { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Iss { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Aud { get; set; }
    }
}
