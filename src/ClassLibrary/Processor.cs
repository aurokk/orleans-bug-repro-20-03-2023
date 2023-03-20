using Microsoft.Extensions.Logging;
using Orleans.Placement;

namespace ClassLibrary;

public interface IProcessor : IGrainWithStringKey
{
    Task Ping();
}

[HashBasedPlacement]
public class Processor : Grain, IProcessor
{
    private readonly ILogger<Processor> _logger;

    private Task? _task;
    private CancellationTokenSource? _cts;
    private DateTime? _createdAtUtc;

    public Processor(ILogger<Processor> logger)
    {
        _logger = logger;
    }

    public override Task OnActivateAsync(CancellationToken ct)
    {
        _createdAtUtc = DateTime.UtcNow;
        _cts = new CancellationTokenSource();
        _task = Task.Run(async () =>
        {
            while (!_cts.IsCancellationRequested)
            {
                _logger.LogInformation("Hello World!");
                await Task.Delay(1000, ct);
            }
        }, ct);

        return Task.CompletedTask;
    }

    public Task Ping()
    {
        if (_createdAtUtc?.AddMinutes(1) < DateTime.UtcNow)
        {
            DeactivateOnIdle();
        }

        return Task.CompletedTask;
    }

    public override async Task OnDeactivateAsync(DeactivationReason reason, CancellationToken ct)
    {
        try
        {
            _cts?.Cancel();
            if (_task != null)
                await _task;
            _cts?.Dispose();
        }
        catch (Exception e)
        {
            // _logger.LogError(e, "Something went wrong");
        }
    }
}