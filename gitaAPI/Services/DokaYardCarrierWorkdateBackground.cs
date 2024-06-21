using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class DukaYardCarrierWorkdateBackground : BackgroundService
{
    private readonly ILogger<DukaYardCarrierWorkdateBackground> _logger;
    private readonly IConfiguration _configuration;

    public DukaYardCarrierWorkdateBackground(ILogger<DukaYardCarrierWorkdateBackground> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("DukaYardCarrierWorkdateBackground elindult.");

        string connMSSQL = _configuration.GetConnectionString("connMSSQL");

        while (!stoppingToken.IsCancellationRequested)
        {
            var currentTime = DateTime.Now;
            var targetTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 23, 00, 0);

            if (currentTime > targetTime)
            {
                targetTime = targetTime.AddDays(1);
            }

            var delay = targetTime - currentTime;

            _logger.LogInformation("Várakozás a következő futásig: {delay}.", delay);

            await Task.Delay(delay, stoppingToken);

            if (stoppingToken.IsCancellationRequested)
            {
                break;
            }

            await TryRunStoredProcWithRetries(connMSSQL, stoppingToken);
        }

        _logger.LogInformation("DukaYardCarrierWorkdateBackground leállt.");
    }

    private async Task TryRunStoredProcWithRetries(string connMSSQL, CancellationToken stoppingToken, int maxRetries = 5)
    {
        int retryCount = 0;
        bool success = false;

        while (retryCount < maxRetries && !success && !stoppingToken.IsCancellationRequested)
        {
            try
            {
                await RunStoredProc(connMSSQL, stoppingToken);
                success = true;
            }
            catch (Exception ex)
            {
                retryCount++;
                _logger.LogError(ex, "Hiba történt a tárolt eljárás végrehajtása közben. Próbálkozás: {retryCount}/{maxRetries}.", retryCount, maxRetries);

                if (retryCount < maxRetries)
                {
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
            }
        }

        if (!success)
        {
            _logger.LogError("A tárolt eljárás nem futott le sikeresen {maxRetries} próbálkozás után.", maxRetries);
        }
    }

    private async Task RunStoredProc(string connMSSQL, CancellationToken stoppingToken)
    {
        using (SqlConnection conn = new SqlConnection(connMSSQL))
        {
            await conn.OpenAsync(stoppingToken);
            using (SqlCommand cmd = new SqlCommand("setCarrierWorkdate", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                await cmd.ExecuteNonQueryAsync(stoppingToken);
                _logger.LogInformation("Tárolt eljárás sikeresen lefutott ekkor: {time}.", DateTime.Now);
            }
        }
    }
}
