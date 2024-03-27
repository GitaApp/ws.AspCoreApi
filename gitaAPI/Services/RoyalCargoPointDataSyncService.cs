
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;


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
            var token = await GetTokenAsync(stoppingToken);
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogError("Token lekérés sikertelen. Az adatok szinkronizálása megszakadt.");
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                continue;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connMSSQL))
                {
                    await conn.OpenAsync(stoppingToken);
                    // Tárolt eljárás meghívása az adatok lekérdezéséhez
                    using (SqlCommand cmd = new SqlCommand("getRoyalCargoPointsToSync", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync(stoppingToken))
                        {
                            while (await reader.ReadAsync(stoppingToken))
                            {
                                // Itt olvassuk ki az adatokat az SqlDataReader-ből
                                int pointId = reader.GetInt32(reader.GetOrdinal("pointId"));
                                int extPointId = reader.GetInt32(reader.GetOrdinal("extPointId"));
                                int status = reader.GetInt32(reader.GetOrdinal("status"));
                                // Az adatok elküldésének logikája a cél API-hoz
                                var (isSuccess, responseJson) = await SendDataToEndpointAsync(targetEndpoint, token, pointId, extPointId, status, stoppingToken);

                                if (isSuccess)
                                {
                                    // Sikeres adatküldés utáni adatbázis frissítése
                                    await UpdateDatabaseAfterSuccess(connMSSQL, pointId, extPointId, status, stoppingToken);
                                }
                                else
                                {
                                    // Sikertelen adatküldés kezelése, használva a válasz JSON tartalmát
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
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); // Várakozás hiba esetén
            }
            await Task.Delay(TimeSpan.FromMinutes(2), stoppingToken); // 1 perc várakozás a következő iteráció előtt
        }

        _logger.LogInformation("RoyalCargoPointDataSyncService háttérfolyamat leállt.");
    }

    private async Task<string> GetTokenAsync(CancellationToken stoppingToken)
    {
        // Itt implementáljuk a token lekérését
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
            // Az HTTP kliens létrehozása
            var httpClient = _httpClientFactory.CreateClient();

            // Autorizációs fejléc beállítása
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Az adatküldéshez szükséges objektum összeállítása
            var dataToSend = new
            {
                PointId = pointId,
                ExtPointId = extPointId,
                Status = status
                // Ide írd be az adatküldéshez szükséges további tulajdonságokat
            };

            // Az adatok elküldése a távoli API végpontjára HTTP POST kéréssel
            var response = await httpClient.PostAsJsonAsync(targetEndpoint, dataToSend, stoppingToken);

            // A válasz ellenőrzése
            if (response.IsSuccessStatusCode)
            {
                // Az adatküldés sikeres volt, visszaadjuk a válasz tartalmát
                string jsonResponse = await response.Content.ReadAsStringAsync();
                dynamic responseObject = JsonConvert.DeserializeObject(jsonResponse);
                return (true, jsonResponse);
            }
            else
            {
                // Az adatküldés sikertelen volt, visszaadjuk a státuszkódot és a válasz tartalmát
                string jsonResponse = await response.Content.ReadAsStringAsync();
                dynamic responseObject = JsonConvert.DeserializeObject(jsonResponse);
                return (false, jsonResponse);
            }
        }
        catch (Exception ex)
        {
            // Hiba történt az adatküldés során, logoljuk az eseményt
            _logger.LogError(ex, "Hiba történt az adatküldés során.");
            return (false, null);
        }
    }


    // Sikeres adatküldés utáni adatbázis frissítése
    private async Task UpdateDatabaseAfterSuccess(string connMSSQL, int pointId, int extPointId ,int status, CancellationToken stoppingToken)
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

    // Sikertelen adatküldés utáni adatbázis frissítése
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

    class TokenResponse
    {
        public string Token { get; set; }
    }
}
