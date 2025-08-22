using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TetrisPro.Core.Engine;
using TetrisPro.Core.Services;
using TetrisPro.Core.AI;
using TetrisPro.App.Services;
using TetrisPro.App.ViewModels;

namespace TetrisPro.App;

public partial class App : Application
{
    public static IHost? HostContainer { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        HostContainer = Host.CreateDefaultBuilder()
            .ConfigureServices((ctx, services) =>
            {
                services.AddSingleton<IRandomizer, SevenBagRandomizer>();
                services.AddSingleton<IInputService, InputService>();
                services.AddSingleton<ITimeProvider, SystemTimeProvider>();
                services.AddSingleton<IGameEngine, GameEngine>();
                services.AddSingleton<RenderTicker>();
                services.AddSingleton<AutoPlayer>();
                services.AddSingleton<MainViewModel>();
                services.AddTransient<Views.MainWindow>();
            })
            .Build();
        HostContainer.Start();
        var window = HostContainer.Services.GetRequiredService<Views.MainWindow>();
        window.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        HostContainer?.Dispose();
        base.OnExit(e);
    }
}
