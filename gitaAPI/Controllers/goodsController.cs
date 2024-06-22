
using System.Data;
using gitaAPI.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using gitaAPI.Model;
using Microsoft.AspNetCore.Authorization;
using Azure.Core;
using Microsoft.Extensions.Configuration;
// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace gitaAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class goodsController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public goodsController(

           IConfiguration configuration)
        {

            _configuration = configuration;
        }

        [Authorize(Roles = UserRoles.User)]
        [HttpPost("getGoods")]
        [Produces("application/json")]
        public async Task<IActionResult> getGoods(GoodsGetData goodsGetData)
        {
            String EntityToJson = JsonConvert.SerializeObject(goodsGetData);
            string connMSSQL = _configuration.GetValue<string>("ConnectionStrings:connMSSQL");
            var accessToken = Request.Headers[HeaderNames.Authorization];
            var retJson = "";
            var errorMessage = "";
            try
            {

                using (SqlConnection con = new SqlConnection(connMSSQL))
                {
                    using (SqlCommand cmd = new SqlCommand("getGoods", con))
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

    

    [Authorize(Roles = UserRoles.User)]
    [HttpPost("setGoods")]
    [Produces("application/json")]
    public async Task<IActionResult> setGoods(GoodsSetGoods goodsSetGoods)
    {
        String EntityToJson = JsonConvert.SerializeObject(goodsSetGoods);
        string connMSSQL = _configuration.GetValue<string>("ConnectionStrings:connMSSQL");
        var accessToken = Request.Headers[HeaderNames.Authorization];
        var retJson = "";
        var errorMessage = "";
        try
        {

            using (SqlConnection con = new SqlConnection(connMSSQL))
            {
                using (SqlCommand cmd = new SqlCommand("setGoods", con))
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


    [Authorize(Roles = UserRoles.User)]
    [HttpPost("getUnits")]
    [Produces("application/json")]
    public async Task<IActionResult> getunits()
    {
        //String EntityToJson = JsonConvert.SerializeObject(goodsSetGoods);
        string connMSSQL = _configuration.GetValue<string>("ConnectionStrings:connMSSQL");
        var accessToken = Request.Headers[HeaderNames.Authorization];
        var retJson = "";
        var errorMessage = "";
        try
        {

            using (SqlConnection con = new SqlConnection(connMSSQL))
            {
                using (SqlCommand cmd = new SqlCommand("getUnits", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@retJSON", SqlDbType.NVarChar, -1).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@token", accessToken.ToString());
                 //   cmd.Parameters.AddWithValue("@jsonData", EntityToJson);
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

