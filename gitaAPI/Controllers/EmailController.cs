using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using gitaAPI.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;

namespace upformapi.Controllers
{
   // [Authorize(Roles = UserRoles.User)]
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public EmailController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("sendfile")]
        [Produces("application/json")]
        public async Task<IActionResult> SendFile([FromForm] IFormFile file, [FromForm] int id)
        {
            if (file == null || id <= 0)
            {
                return BadRequest("Invalid file or ID.");
            }

            string connMSSQL = _configuration.GetValue<string>("ConnectionStrings:connMSSQL");
            
            string email = await GetEmailByIdAsync(connMSSQL, id);

            if (string.IsNullOrEmpty(email))
            {
                return NotFound("Email not found.");
            }

            try
            {
                await SendEmailAsync(email, file);
                return Ok("File sent successfully.");
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Error sending email: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error sending email: {ex.Message}");
            }
        }
        
        private async Task<string> GetEmailByIdAsync(string connectionString, int id)
        {
            string email = null;
            var accessToken = Request.Headers[HeaderNames.Authorization];
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("GetEmailById", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
                        cmd.Parameters.Add("@email", SqlDbType.NVarChar, 256).Direction = ParameterDirection.Output;
                        cmd.Parameters.AddWithValue("@token", accessToken.ToString());

                        await con.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();

                        email = cmd.Parameters["@email"].Value.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Error retrieving email: {ex.Message}");
                throw;
            }

            return email;
        }
        private async Task SendEmailAsync(string email, IFormFile file)
        {
            try
            {
                using (var ms = new MemoryStream())
                {
                    await file.CopyToAsync(ms);
                    var fileBytes = ms.ToArray();

                    var smtpClient = new SmtpClient(_configuration["Smtp:Host"])
                    {
                        Port = int.Parse(_configuration["Smtp:Port"]),
                        Credentials = new NetworkCredential(_configuration["Smtp:Username"], _configuration["Smtp:Password"]),
                        EnableSsl = bool.Parse(_configuration["Smtp:EnableSsl"]),
                    };

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(_configuration["Smtp:From"]),
                        Subject = file.FileName,
                        Body = "Tisztelt Partnerünk!<br><br>Mellékletben küldjük a jármű listát.<br><br>A számlában kérjük feltüntetni a fuvart be azonosító sorszámot, és a számlázási időszakot.<br><br>Duka Yard Kft.<br>1186 Budapest, Besence utca 3.",
                        IsBodyHtml = true,
                    };
                    mailMessage.To.Add(email);
                    mailMessage.Attachments.Add(new Attachment(new MemoryStream(fileBytes), file.FileName));

                    smtpClient.Send(mailMessage);
                }
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Error sending email: {ex.Message}");
                throw; // Optionally, you can handle the error here or rethrow it to be handled elsewhere
            }
        }

    
}
}
