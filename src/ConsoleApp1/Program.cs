using ClassLibrary;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host
    .CreateDefaultBuilder(args)
    .UseOrleans(siloBuilder => siloBuilder
        .UseAdoNetClustering(x =>
        {
            x.ConnectionString = "Host=postgres;Port=5432;Username=repro;Password=repro;Database=repro;";
            x.Invariant = "Npgsql";
        })
        .UseDashboard(options => options.Port = 33333)
    )
    .ConfigureServices(_ => _.AddHostedService<Supervisor>())
    .Build();

host.Run();