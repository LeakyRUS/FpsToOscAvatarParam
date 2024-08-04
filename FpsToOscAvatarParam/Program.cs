using FpsToOscAvatarParam;
using Microsoft.Extensions.Configuration;

var configurationRoot = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

Console.WriteLine(configurationRoot.GetDebugView());

using var fpsProvider = new WindowsEventFpsProvider();
using var sender = new AvatarOscSender();
using var cts = new CancellationTokenSource();

Console.WriteLine("Press Ctrl + C to stop the program.");

Console.CancelKeyPress += (sender, e) =>
{
    cts.Cancel();
    e.Cancel = true;
};

new Runner(configurationRoot, fpsProvider, sender)
    .Run(cts.Token)
    .Wait();