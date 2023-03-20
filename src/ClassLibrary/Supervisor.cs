using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ClassLibrary;

public class Supervisor : BackgroundService
{
    private readonly IClusterClient _clusterClient;
    private readonly ILogger<Supervisor> _logger;

    public Supervisor(IClusterClient clusterClient, ILogger<Supervisor> logger)
    {
        _clusterClient = clusterClient;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                var grain = _clusterClient.GetGrain<IProcessor>("1");
                await grain.Ping();
                await Task.Delay(1000, ct);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong");
            }
        }
    }
}