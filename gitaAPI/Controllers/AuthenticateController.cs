using Azure.Core;
using gitaAPI.Entities;
using gitaAPI.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

using static System.Net.Mime.MediaTypeNames;

namespace gitaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthenticateController(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }
        
   


    


       [HttpPost("login")]
        public async Task<IActionResult> login(loginUser loginUser)
        {
            var retJson = "";
            var retValue = 0;
            var user = "";
            var userId = 0;
            string connMSSQL = _configuration.GetValue<string>("ConnectionStrings:connMSSQL");
            string EntityToJson = JsonConvert.SerializeObject(loginUser);
          
            using (SqlConnection con = new SqlConnection(connMSSQL))
            {
                using (SqlCommand cmd = new SqlCommand("AspNetCoreUserLogin", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@retJSON", SqlDbType.NVarChar, -1).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("@retValue",SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("@user", SqlDbType.NVarChar, -1).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("@userId", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@jsonData", EntityToJson);

                    con.Open();
                    cmd.ExecuteNonQuery();

                    retJson = (string)cmd.Parameters["@retJSON"].Value;
                    retValue = (int)cmd.Parameters["@retValue"].Value;
                    user = (string)cmd.Parameters["@user"].Value;
                    userId = (int)cmd.Parameters["@userId"].Value;
                }
            }

            
         

            var authClaims = new List < Claim> 
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, user),
                new Claim(ClaimTypes.Role, "User")
            };

          
            var token = GetToken(authClaims);
           
            ResponseWeb responseWeb = new ResponseWeb();
            responseWeb.token = new JwtSecurityTokenHandler().WriteToken(token);
            responseWeb.expiration = token.ValidTo;


            retJson =updateUsertoken(userId, responseWeb.token, token.ValidTo);

            JObject json = JObject.Parse(retJson);





         



            return

                new JsonResult(json);
        }


      
     






        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(24),
                claims: authClaims,
                
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }

        private string CreateToken(loginUser loginUser)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, loginUser.email)
            };
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("Appsettings:JWT:Secret").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            DateTime d1 = DateTime.Now;
            DateTime d2 = d1.AddSeconds(60);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: d2,
                signingCredentials: creds
                );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }


        private string updateUsertoken(int userId, string token ,DateTime expire)
        {


            Double OADate = 86400000;//expire.ToOADate();
            //DateTime dt =   DateTime.FromOADate(OADate);
            var retJson = "";
            string connMSSQL = _configuration.GetValue<string>("ConnectionStrings:connMSSQL");
            using (SqlConnection con = new SqlConnection(connMSSQL))
            {
                using (SqlCommand cmd = new SqlCommand("userLoginUpdateToken", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@retJSON", SqlDbType.NVarChar, -1).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("@retValue", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@token", token);
                    cmd.Parameters.AddWithValue("@expire", expire);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@OADate", OADate);


                    con.Open();
                    cmd.ExecuteNonQuery();

                    retJson = (string)cmd.Parameters["@retJSON"].Value;
                    
                    
                }
            }


            return retJson;
        }



    }
}
