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

    public class EnsureController : ControllerBase
    {

        private readonly IConfiguration _configuration;

        public EnsureController(
      
           IConfiguration configuration)
        {
          
            _configuration = configuration;
        }




        [HttpPost("getEnsureFile")]
        [Produces("application/octet-stream")]
        public async Task<IActionResult> GetEnsureFile(OperartionWithId operartionWithId)
        {
            String EntityToJson = JsonConvert.SerializeObject(operartionWithId);
            string connMSSQL = _configuration.GetValue<string>("ConnectionStrings:connMSSQLMedia");
            var accessToken = Request.Headers[HeaderNames.Authorization];
            byte[] fileData = null;
            string fileName = "";
            var errorMessage = "";

            try
            {
                using (SqlConnection con = new SqlConnection(connMSSQL))
                {
                    using (SqlCommand cmd = new SqlCommand("getEnsureFile", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@fileData", SqlDbType.VarBinary, -1).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add("@fileName", SqlDbType.NVarChar, 255).Direction = ParameterDirection.Output;
                      
                        cmd.Parameters.AddWithValue("@json", EntityToJson);
                        con.Open();
                        cmd.ExecuteNonQuery();

                        fileData = (byte[])cmd.Parameters["@fileData"].Value;
                        fileName = (string)cmd.Parameters["@fileName"].Value;
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
                return new JsonResult(jsonString);
            }

            if (fileData != null && fileData.Length > 0)
            {
                return File(fileData, "application/octet-stream", fileName);
            }
            else
            {
                return NotFound(); // or any other appropriate HTTP status code
            }
        }

        [HttpPost("getEnsurePdfFile")]
        [Produces("application/pdf")]
        public async Task<IActionResult> getEnsurePdfFile(OperartionWithId operationWithId)
        {
            String EntityToJson = JsonConvert.SerializeObject(operationWithId);
            string connMSSQL = _configuration.GetValue<string>("ConnectionStrings:connMSSQLMedia");
            var accessToken = Request.Headers[HeaderNames.Authorization];
            byte[] fileData = null;
            string fileName = "";
            var errorMessage = "";

            try
            {
                using (SqlConnection con = new SqlConnection(connMSSQL))
                {
                    using (SqlCommand cmd = new SqlCommand("getEnsureFile", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@fileData", SqlDbType.VarBinary, -1).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add("@fileName", SqlDbType.NVarChar, 255).Direction = ParameterDirection.Output;

                        cmd.Parameters.AddWithValue("@json", EntityToJson);
                        con.Open();
                        cmd.ExecuteNonQuery();

                        fileData = (byte[])cmd.Parameters["@fileData"].Value;
                        fileName = (string)cmd.Parameters["@fileName"].Value;
                        // Biztosítani, hogy a fájlnév kiterjesztése .pdf legyen
                        if (!fileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                        {
                            fileName += ".pdf";
                        }
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
                return new JsonResult(jsonString);
            }

            if (fileData != null && fileData.Length > 0)
            {
                return File(fileData, "application/pdf", fileName);
            }
            else
            {
                return NotFound(); // vagy bármely más megfelelő HTTP státuszkód
            }
        }


    }
}

