using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

namespace CityInfo.API.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        // these classes won't be used outside of AuthenticationController, so we can scope it to this namespace.
        // this can always be moved to its own namespace or file.
        public class AuthenticationRequestBody
        {
            public string? UserName { get; set; }
            public string? Password { get; set; }
        }

        public class CityInfoUser
        {
            public int UserId { get; set; }
            public string UserName { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string City { get; set; }

            public CityInfoUser(
                int userId,
                string userName,
                string firstName,
                string lastName,
                string city)
            {
                UserId = userId;
                UserName = userName;
                FirstName = firstName;
                LastName = lastName;
                City = city;
            }

        }

        public AuthenticationController(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>
        /// Endpoint that takes in username and password and returns a token.
        /// </summary>
        /// <returns>Token as string</returns>
        [HttpPost("authenticate")]
        public ActionResult<string> Authenticate(AuthenticationRequestBody authenticationRequestBody)
        {
            // Step 1: Validate the username/password
            var user = ValidateUserCredentials(
                authenticationRequestBody.UserName, 
                authenticationRequestBody.Password);

            if (user == null)
            {
                return Unauthorized();
            }

            // Step 2: Create a token
            var securityKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(_configuration["Authentication:SecretForKey"]));
            var signingCredentials = new SigningCredentials(
                securityKey, SecurityAlgorithms.HmacSha256);

            var claimsForToken = new List<Claim>
            {
                new ("sub", user.UserId.ToString()), // "sub" is a standardized key for the unique user identifier
                new ("given_name", user.FirstName),
                new ("family_name", user.LastName),
                new ("city", user.City)
            };

            var jwtSecurityToken = new JwtSecurityToken(
                _configuration["Authentication:Issuer"],
                _configuration["Authentication:Audience"],
                claimsForToken,
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(1),
                signingCredentials);

            var tokenToReturn = new JwtSecurityTokenHandler()
                .WriteToken(jwtSecurityToken);

            return Ok(tokenToReturn);
        }

        private CityInfoUser ValidateUserCredentials(string? userName, string? password)
        {
            // we don't have a user DB or table. If you have, check that passed-through
            // username/password against what's stored in the database.
            // For demo purposes, we assume the credentials are valid.

            // return a new CityInfoUser (values would normally come from your user DB/table)
            return new CityInfoUser(
                1,
                userName ?? "",
                "Katherine",
                "Picazo",
                "Calgary");
        }
    }
}
