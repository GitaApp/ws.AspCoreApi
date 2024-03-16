
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using gitaAPI.Entities;
namespace upformapi.Controllers
{

    //  [Authorize(Roles = UserRoles.User)]
    [Route("api/[controller]")]
    [ApiController]

    public class ContainerController : ControllerBase
    {

        private readonly IConfiguration _configuration;

        public ContainerController(
      
           IConfiguration configuration)
        {
          
            _configuration = configuration;
        }


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

       
        [HttpPost("setContainerShipment")]
        [Produces("application/json")]
    public async Task<IActionResult> setContainerShipment(Container container)
    {
        String EntityToJson = JsonConvert.SerializeObject(container);
            string connMSSQL = _configuration.GetValue<string>("ConnectionStrings:connMSSQL");
            var accessToken = Request.Headers[HeaderNames.Authorization];
        var retJson = "";
         var errorMessage = "";
         
            try
            {

                using (SqlConnection con = new SqlConnection(connMSSQL))
            {
                using (SqlCommand cmd = new SqlCommand("setContainerShipment", con))
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
            catch (SqlException ex)
            {

                errorMessage = ex.Message;


                var errorHandling = new ErrorHandling
                {

                    error = "error",
                    errorCode = ex.ErrorCode,
                    errormessage = errorMessage

                };


              //  string jsonString = JsonConvert.SerializeObject(errorHandling);

              
               
                return StatusCode(StatusCodes.Status400BadRequest, JsonConvert.SerializeObject(errorHandling));

            }

            JObject json = JObject.Parse(retJson);

            int errorCode = (int)json["errorCode"];



           

            if (errorCode == 401)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, json);
            }


            if (errorCode != 200)
            {
                return StatusCode(StatusCodes.Status400BadRequest, json);
            }




            return new JsonResult(json);
        }

        //getContainerShipmentWithTaskcode

       
        [HttpPost("getContainerShipmentWithTaskcode")]
        [Produces("application/json")]
        public async Task<IActionResult> getContainerShipmentWithTaskcode(getShipmentDataWithTaskCode getShipmentDataWithTaskCode)
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
                    using (SqlCommand cmd = new SqlCommand("getContainerShipmentWithTaskcode", con))
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


        //   [HttpPost("getShipmentWithTaskCode")]

        //    [Produces("application/json")]
        //public async Task<IActionResult> getShipmentWithTaskCode(getShipmentDataWithTaskCode getShipmentDataWithTaskCode)
        //{
        //    String EntityToJson = JsonConvert.SerializeObject(getShipmentDataWithTaskCode);
        //        string connMSSQL = _configuration.GetValue<string>("ConnectionStrings:connMSSQL");
        //        var accessToken = Request.Headers[HeaderNames.Authorization];
        //    var retJson = "";
        //     var errorMessage = "";
        //        try
        //        {

        //            using (SqlConnection con = new SqlConnection(connMSSQL))
        //        {
        //            using (SqlCommand cmd = new SqlCommand("getShipmentWithTaskCode", con))
        //            {
        //                cmd.CommandType = CommandType.StoredProcedure;
        //                cmd.Parameters.Add("@retJSON", SqlDbType.NVarChar, -1).Direction = ParameterDirection.Output;
        //                cmd.Parameters.AddWithValue("@token", accessToken.ToString());
        //                cmd.Parameters.AddWithValue("@json", EntityToJson);
        //                con.Open();
        //                cmd.ExecuteNonQuery();

        //                retJson = (string)cmd.Parameters["@retJSON"].Value;
        //            }
        //        }

        //        }
        //        catch (Exception e)
        //        {

        //            errorMessage = e.Message;


        //            var errorHandling = new ErrorHandling
        //            {
        //                error = "error",
        //                errormessage = errorMessage
        //            };


        //            string jsonString = JsonConvert.SerializeObject(errorHandling);

        //            retJson = jsonString;


        //        }

        //        JObject json = JObject.Parse(retJson);

        //        return

        //            new JsonResult(json);
    }








}