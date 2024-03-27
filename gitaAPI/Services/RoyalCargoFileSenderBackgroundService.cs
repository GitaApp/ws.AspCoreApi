using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Net.Http.Headers;

public class RoyalCargoFileSenderBackgroundService : BackgroundService
{
    private readonly ILogger<RoyalCargoFileSenderBackgroundService> _logger;
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;
    private string _token;

    public RoyalCargoFileSenderBackgroundService(ILogger<RoyalCargoFileSenderBackgroundService> logger, IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("File sender háttérfolyamat elindult.");

        string connMSSQL = _configuration.GetValue<string>("ConnectionStrings:connMSSQL");
        string apiUrl = _configuration.GetValue<string>("RoyalfileUploadUrl");

        // Token lekérése
        _token = await GetTokenAsync(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connMSSQL))
                {
                    await conn.OpenAsync(stoppingToken);
                    using (SqlCommand cmd = new SqlCommand("getRoyalCargoMedia", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync(stoppingToken))
                        {
                            while (await reader.ReadAsync(stoppingToken))
                            {
                                var id = reader["id"].ToString();
                                var mediaId = reader["mediaId"].ToString();
                                var fileName = reader["fileName"].ToString();
                                var docTypeId = reader["docTypeId"].ToString();
                                var fileContentBytes = (byte[])reader["fileContent"]; // Assuming 'fileContent' is the column name

                                using (var httpClient = _httpClientFactory.CreateClient())
                                {
                                    // Setting up headers
                                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
                                    httpClient.DefaultRequestHeaders.Add("id", id);
                                    httpClient.DefaultRequestHeaders.Add("fileName", fileName);
                                    httpClient.DefaultRequestHeaders.Add("docTypeId", docTypeId);

                                    using (var formData = new MultipartFormDataContent())
                                    {
                                        var fileContent = new ByteArrayContent(fileContentBytes);
                                        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
                                        formData.Add(fileContent, "file-to-upload", fileName);

                                        var response = await httpClient.PostAsync(apiUrl, formData, stoppingToken);

                                        if (response.IsSuccessStatusCode)
                                        {
                                            _logger.LogInformation("File successfully uploaded.");

                                            if (mediaId != null)
                                            {
                                                await UpdateDatabaseAfterSuccess(mediaId, stoppingToken);
                                            }
                                        }
                                        else
                                        {
                                            _logger.LogError($"Failed to upload file. Status code: {response.StatusCode}");

                                            int statusCode = (int)response.StatusCode;
                                            await UpdateDatabaseAfterError(mediaId, stoppingToken, statusCode);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during the background process.");
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); // Wait on error
            }
            await Task.Delay(TimeSpan.FromMinutes(2), stoppingToken); // Wait 10 minutes before the next iteration
        }

        _logger.LogInformation("File sender background process has stopped.");
    }

    private async Task<string> GetTokenAsync(CancellationToken stoppingToken)
    {
        string tokenEndpoint = _configuration.GetValue<string>("TeagetEndpointGetToken");

        try
        {
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                var requestData = new { user = "gitauser1@selester.hu", pass = "gxShaCYxvEKN" };

                var response = await httpClient.PostAsJsonAsync(tokenEndpoint, requestData, stoppingToken);

                if (response.IsSuccessStatusCode)
                {
                    var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>(cancellationToken: stoppingToken);
                    return tokenResponse?.Token;
                }
                else
                {
                    _logger.LogError($"Failed to retrieve token. Status code: {response.StatusCode}");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving the token.");
        }

        return null;
    }

    private async Task UpdateDatabaseAfterSuccess(string mediaId, CancellationToken stoppingToken)
    {
        string connMSSQL = _configuration.GetValue<string>("ConnectionStrings:connMSSQL");
        try
        {
            using (SqlConnection updateConn = new SqlConnection(connMSSQL))
            {
                await updateConn.OpenAsync(stoppingToken);
                using (SqlCommand cmd = new SqlCommand("RoyalCargoMediaSentSP", updateConn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", mediaId);
                    await cmd.ExecuteNonQueryAsync(stoppingToken);
                    _logger.LogInformation($"Sikeres frissítés az ID {mediaId} rekordhoz.");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Hiba történt az ID {mediaId} rekord frissítése során.");
        }
    }

    private async Task UpdateDatabaseAfterError(string mediaId, CancellationToken stoppingToken, int StatusCode)
    {
        string connMSSQL = _configuration.GetValue<string>("ConnectionStrings:connMSSQL");
        try
        {
            using (SqlConnection updateConn = new SqlConnection(connMSSQL))
            {
                await updateConn.OpenAsync(stoppingToken);
                using (SqlCommand cmd = new SqlCommand("RoyalMediaSentAfterErrorSP", updateConn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", mediaId);
                    cmd.Parameters.AddWithValue("@StatusCode", StatusCode);
                    await cmd.ExecuteNonQueryAsync(stoppingToken);
                    _logger.LogInformation($"Sikeres frissítés az ID {mediaId} rekordhoz.");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Hiba történt az ID {mediaId} rekord frissítése során.");
        }
    }
    class TokenResponse
    {
        public string Token { get; set; }
    }
}
