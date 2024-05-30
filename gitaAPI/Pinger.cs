public class Pinger : BackgroundService
{
    private readonly ILogger<Pinger> _logger;

    public Pinger(ILogger<Pinger> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var httpClient = new HttpClient())
            {
               // await httpClient.GetAsync("https://sample.com/", stoppingToken);
                await httpClient.GetAsync("https://ws.gitaapp.eu:4334", stoppingToken);

                _logger.LogInformation($"Ping at {DateTime.UtcNow:s}");
            }

            await Task.Delay(10 * 1000, stoppingToken);
        }
    }
}