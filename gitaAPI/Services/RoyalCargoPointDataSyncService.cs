using System;
using System.Data;
using System.Data.SqlClient;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

public class RoyalCargoPointDataSyncService : BackgroundService
{
    private readonly ILogger<RoyalCargoPointDataSyncService> _logger;
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;

    public RoyalCargoPointDataSyncService(ILogger<RoyalCargoPointDataSyncService> logger, IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("RoyalCargoPointDataSyncService háttérfolyamat elindult.");

        string connMSSQL = _configuration.GetValue<string>("ConnectionStrings:connMSSQL");
        string targetEndpoint = _configuration.GetValue<string>("TargetEndpoint");

        while (!stoppingToken.IsCancellationRequested)
        {

            _logger.LogInformation("RoyalCargoPointDataSyncService runing at: {time}",DateTimeOffset.Now);
            await Task.Delay(1000, stoppingToken);

            var token = await GetTokenAsync(stoppingToken);
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogError("Token lekérés sikertelen. Az adatok szinkronizálása megszakadt.");
              
                continue;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connMSSQL))
                {
                    await conn.OpenAsync(stoppingToken);
                    using (SqlCommand cmd = new SqlCommand("getRoyalCargoPointsToSync", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync(stoppingToken))
                        {
                            while (await reader.ReadAsync(stoppingToken))
                            {
                                int pointId = reader.GetInt32(reader.GetOrdinal("pointId"));
                                int extPointId = reader.GetInt32(reader.GetOrdinal("extPointId"));
                                int status = reader.GetInt32(reader.GetOrdinal("status"));

                                var (isSuccess, responseJson) = await SendDataToEndpointAsync(targetEndpoint, token, pointId, extPointId, status, stoppingToken);

                                if (isSuccess)
                                {
                                    await UpdateDatabaseAfterSuccess(connMSSQL, pointId, extPointId, status, stoppingToken);
                                }
                                else
                                {
                                    await UpdateDatabaseAfterError(connMSSQL, pointId, extPointId, status, responseJson, stoppingToken);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Hiba történt a RoyalCargoPointDataSyncService háttérfolyamat során.");
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }

        _logger.LogInformation("RoyalCargoPointDataSyncService háttérfolyamat leállt.");
    }

    private async Task<string> GetTokenAsync(CancellationToken stoppingToken)
    {
        string tokenEndpoint = _configuration.GetValue<string>("TeagetEndpointGetToken");
        var httpClient = _httpClientFactory.CreateClient();
        var response = await httpClient.PostAsJsonAsync(tokenEndpoint, new { user = "gitauser1@selester.hu", pass = "gxShaCYxvEKN" }, stoppingToken);
        if (response.IsSuccessStatusCode)
        {
            var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>(cancellationToken: stoppingToken);
            return tokenResponse?.Token;
        }
        return null;
    }

    private async Task<(bool Success, string ResponseJson)> SendDataToEndpointAsync(string targetEndpoint, string token, int pointId, int extPointId, int status, CancellationToken stoppingToken)
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var dataToSend = new
            {
                PointId = pointId,
                ExtPointId = extPointId,
                Status = status
            };

            var response = await httpClient.PostAsJsonAsync(targetEndpoint, dataToSend, stoppingToken);

            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                return (true, jsonResponse);
            }
            else
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                string jsonData = JsonConvert.SerializeObject(dataToSend);
                LogAfterSendErrorError(pointId,extPointId, jsonData, jsonResponse);

                return (false, jsonResponse);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Hiba történt az adatküldés során.");
            return (false, null);
        }
    }

    private void LogAfterSendErrorError(int pointId, int extPointId, string jsonData, string jsonResponse)
    {
        string connMSSQL = _configuration.GetValue<string>("ConnectionStrings:connMSSQL");
        using (SqlConnection updateConn = new SqlConnection(connMSSQL))
        {
            
            using (SqlCommand cmd = new SqlCommand("RoyalCargoUpdateApiPointDataErroLogSP", updateConn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PointId", pointId);
                cmd.Parameters.AddWithValue("@extPointId", extPointId);
                cmd.Parameters.AddWithValue("@jsonData", jsonData);
                cmd.Parameters.AddWithValue("@jsonResponse", jsonResponse);

            }
        }
    }

    private async Task UpdateDatabaseAfterError(string connMSSQL, int pointId, int extPointId, int status, string errorMessage, CancellationToken stoppingToken)
    {
        using (SqlConnection updateConn = new SqlConnection(connMSSQL))
        {
            await updateConn.OpenAsync(stoppingToken);
            using (SqlCommand cmd = new SqlCommand("RoyalCargoUpdatePointDataAfterError", updateConn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PointId", pointId);
                cmd.Parameters.AddWithValue("@extPointId", extPointId);
                cmd.Parameters.AddWithValue("@status", status);
                cmd.Parameters.AddWithValue("@ErrorMessage", errorMessage);
                await cmd.ExecuteNonQueryAsync(stoppingToken);
            }
        }
    }

    // Sikeres adatküldés utáni adatbázis frissítése
    private async Task UpdateDatabaseAfterSuccess(string connMSSQL, int pointId, int extPointId, int status, CancellationToken stoppingToken)
    {
        using (SqlConnection updateConn = new SqlConnection(connMSSQL))
        {
            await updateConn.OpenAsync(stoppingToken);
            using (SqlCommand cmd = new SqlCommand("RoyalCargoUpdatePointDataAfterSuccess", updateConn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PointId", pointId);
                cmd.Parameters.AddWithValue("@extPointId", extPointId);
                cmd.Parameters.AddWithValue("@status", status);
                await cmd.ExecuteNonQueryAsync(stoppingToken);
            }
        }
    }

    class TokenResponse
    {
        public string Token { get; set; }
    }
}
