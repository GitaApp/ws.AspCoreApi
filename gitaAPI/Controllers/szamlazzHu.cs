using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using upformapi.Model;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using Microsoft.AspNetCore.Identity;
using gitaAPI.Model;
using gitaAPI.Entities;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.Annotations;
using gitaAPI.Examples;
using System.ComponentModel;
// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace upformapi.Controllers
{

  //  [Authorize(Roles = UserRoles.User)]
    [Route("api/[controller]")]
    [ApiController]

    public class szamlazzHuController : ControllerBase
    {

        private readonly IConfiguration _configuration;

        public szamlazzHuController(
      
           IConfiguration configuration)
        {
          
            _configuration = configuration;
        }




        [HttpPost("getInvSettings")]
        [Produces("application/json")]
        public IActionResult GetInvSettings()
        {
            // String EntityToJson = JsonConvert.SerializeObject(shipmentData);
            string connMSSQLszamlazz = _configuration.GetValue<string>("ConnectionStrings:connMSSQLszamlazz");
            var accessToken = Request.Headers[HeaderNames.Authorization];
            var retJson = "";
            var errorMessage = "";
            try
            {

                using (SqlConnection con = new SqlConnection(connMSSQLszamlazz))
                {
                    using (SqlCommand cmd = new SqlCommand("getInvSettings", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@retJSON", SqlDbType.NVarChar, -1).Direction = ParameterDirection.Output;
                        cmd.Parameters.AddWithValue("@token", accessToken.ToString());
                        //  cmd.Parameters.AddWithValue("@json", EntityToJson);
                        con.Open();
                        cmd.ExecuteNonQuery();

                        retJson = (string)cmd.Parameters["@retJSON"].Value;
                    }
                }

            }
            catch (Exception e)
            {

                errorMessage = e.Message;


                var errorHandling = new ErrorHandling
                {
                    error = "error",
                    errormessage = errorMessage
                };


                string jsonString = JsonConvert.SerializeObject(errorHandling);

                retJson = jsonString;


            }

            JObject json = JObject.Parse(retJson);

            int errorCode = (int)json["errorCode"];

            if (errorCode == 401)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, json);
            }

            return new JsonResult(json);
        }

        [HttpPost("setInvSettings")]
        [Produces("application/json")]
        public async Task<IActionResult> setInvSettings(InvSettings invSettings) 
        {
            String EntityToJson = JsonConvert.SerializeObject(invSettings);
            string connMSSQLszamlazz = _configuration.GetValue<string>("ConnectionStrings:connMSSQLszamlazz");
            var accessToken = Request.Headers[HeaderNames.Authorization];
            
            var retJson = "";
            var errorMessage = "";
            try
            {

                using (SqlConnection con = new SqlConnection(connMSSQLszamlazz))
                {
                    using (SqlCommand cmd = new SqlCommand("setInvSettings", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@retJSON", SqlDbType.NVarChar, -1).Direction = ParameterDirection.Output;
                        cmd.Parameters.AddWithValue("@token", accessToken.ToString());
                        cmd.Parameters.AddWithValue("@jsonData", EntityToJson);
                        con.Open();
                        cmd.ExecuteNonQuery();

                        retJson = (string)cmd.Parameters["@retJSON"].Value;
                    }
                }

            }
            catch (Exception e)
            {

                errorMessage = e.Message;


                var errorHandling = new ErrorHandling
                {
                    error = "error",
                    errormessage = errorMessage
                };


                string jsonString = JsonConvert.SerializeObject(errorHandling);

                retJson = jsonString;


            }

            JObject json = JObject.Parse(retJson);

            int errorCode = (int)json["errorCode"];

            if (errorCode == 401)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, json);
            }

            return new JsonResult(json);
        }

        [HttpPost("invoiceQuery")]
        [Produces("application/json")]
        public async Task<IActionResult> invoiceQuery(SetId setId)
        {
            try
            {
                string connMSSQL = _configuration.GetValue<string>("ConnectionStrings:connMSSQLszamlazz");
                var accessToken = Request.Headers[HeaderNames.Authorization];

                using (SqlConnection con = new SqlConnection(connMSSQL))
                {
                    using (SqlCommand cmd = new SqlCommand("invoiceQuery", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@retJSON", SqlDbType.NVarChar, -1).Direction = ParameterDirection.Output;
                        cmd.Parameters.AddWithValue("@token", accessToken.ToString());
                        cmd.Parameters.AddWithValue("@json", JsonConvert.SerializeObject(setId));

                        con.Open();
                        cmd.ExecuteNonQuery();

                        string retJson = (string)cmd.Parameters["@retJSON"].Value;

                        JObject json = JObject.Parse(retJson);

                        

                        int errorCode = (int)json["errorCode"];

                        if (errorCode == 401)
                        {
                            return StatusCode(StatusCodes.Status401Unauthorized, json);
                        }

                        return new JsonResult(json);


                    }
                }
            }
            catch (Exception e)
            {
                var errorHandling = new ErrorHandling
                {
                    error = "error",
                    errormessage = e.Message
                };

                string jsonString = JsonConvert.SerializeObject(errorHandling);

                return new JsonResult(JObject.Parse(jsonString));
            }
        }

       


       
      


    }

    public class SetId
    {
        public int id { get; set; }
    }



}

