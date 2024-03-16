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
// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace upformapi.Controllers
{

  //  [Authorize(Roles = UserRoles.User)]
    [Route("api/[controller]")]
    [ApiController]

    public class ShipmentController : ControllerBase
    {

        private readonly IConfiguration _configuration;

        public ShipmentController(
      
           IConfiguration configuration)
        {
          
            _configuration = configuration;
        }

        [Authorize(Roles = UserRoles.User)]
        [HttpPost("getTaskCodePrefix")]
        [Produces("application/json")]
        public async Task<IActionResult> getTaskCodePrefix( )
        {
           // String EntityToJson = JsonConvert.SerializeObject(shipmentData);
            string connMSSQL = _configuration.GetValue<string>("ConnectionStrings:connMSSQL");
            var accessToken = Request.Headers[HeaderNames.Authorization];
            var retJson = "";
            var errorMessage = "";
            try
            {

                using (SqlConnection con = new SqlConnection(connMSSQL))
                {
                    using (SqlCommand cmd = new SqlCommand("getTaskCodePrefix", con))
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

        [Authorize(Roles = UserRoles.User)]
        [HttpPost("setShipment")]
        [SwaggerResponseExample(200, typeof(ShipmentDataExample))]
        [Produces("application/json")]

        public async Task<IActionResult> setShipment(shipmentData shipmentData)
    {
        String EntityToJson = JsonConvert.SerializeObject(shipmentData);
            string connMSSQL = _configuration.GetValue<string>("ConnectionStrings:connMSSQL");
            var accessToken = Request.Headers[HeaderNames.Authorization];
        var retJson = "";
         var errorMessage = "";

            try
            {

                using (SqlConnection con = new SqlConnection(connMSSQL))
            {
                using (SqlCommand cmd = new SqlCommand("setShipment", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@retJSON", SqlDbType.NVarChar, -1).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@token", accessToken.ToString());
                    cmd.Parameters.AddWithValue("@json", EntityToJson);
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



        [Authorize(Roles = UserRoles.User)]
        [HttpPost("getShipmentWithTaskCode")]
        [Produces("application/json")]
    public async Task<IActionResult> getShipmentWithTaskCode(getShipmentDataWithTaskCode getShipmentDataWithTaskCode)
    {
        String EntityToJson = JsonConvert.SerializeObject(getShipmentDataWithTaskCode);
            string connMSSQL = _configuration.GetValue<string>("ConnectionStrings:connMSSQL");
            var accessToken = Request.Headers[HeaderNames.Authorization];
        var retJson = "";
         var errorMessage = "";
            try
            {

                using (SqlConnection con = new SqlConnection(connMSSQL))
            {
                using (SqlCommand cmd = new SqlCommand("getShipmentWithTaskCode", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@retJSON", SqlDbType.NVarChar, -1).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@token", accessToken.ToString());
                    cmd.Parameters.AddWithValue("@json", EntityToJson);
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

        [HttpPost("getLog")]

        [Produces("application/json")]
        public async Task<IActionResult> getLog(getShipmentDataWithTaskCode getShipmentDataWithTaskCode)
        {
            String EntityToJson = JsonConvert.SerializeObject(getShipmentDataWithTaskCode);
            string connMSSQL = _configuration.GetValue<string>("ConnectionStrings:connMSSQL");
            var accessToken = Request.Headers[HeaderNames.Authorization];
            var retJson = "";
            var errorMessage = "";
            try
            {

                using (SqlConnection con = new SqlConnection(connMSSQL))
                {
                    using (SqlCommand cmd = new SqlCommand("getSelexpedLog", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@retJSON", SqlDbType.NVarChar, -1).Direction = ParameterDirection.Output;
                        cmd.Parameters.AddWithValue("@token", accessToken.ToString());
                        cmd.Parameters.AddWithValue("@json", EntityToJson);
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





    }
}