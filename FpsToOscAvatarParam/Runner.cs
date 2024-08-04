using Microsoft.Extensions.Configuration;

namespace FpsToOscAvatarParam;

public class Runner
{
    private readonly IConfiguration _configuration;
    private readonly IFpsProvider _fpsProvider;
    private readonly IOSCAvatarSingleValueSender _sender;

    public Runner(
        IConfiguration configuration,
        IFpsProvider fpsProvider,
        IOSCAvatarSingleValueSender sender)
    {
        _configuration = configuration;
        _fpsProvider = fpsProvider;
        _sender = sender;
    }

    public async Task Run(CancellationToken cancellationToken)
    {
        var countUpdates = 0f;
        var prevResult = 0f;
        var newResult = 0f;

        while (!cancellationToken.IsCancellationRequested)
        {
            var settings = new Settings();
            _configuration.Bind(settings);

            var updateMills = settings.UpdateMills;
            countUpdates += 1f / updateMills;

            var fps = _fpsProvider.GetFps(settings.TargetProcess);
            var maxFps = settings.MaxFps;
            var resultFps = fps < maxFps ? 1 - (fps / maxFps) : 0;

            if (countUpdates >= 1f)
            {
                countUpdates = 0f;
                prevResult = newResult;
                newResult = resultFps;
            }

            var toSend = float.Lerp(prevResult, newResult, countUpdates);

            //Console.Write($"\r{toSend:F2}       ");

            _sender.SetAddress(settings.AvatarParameterUrl);
            await _sender.SendValue(toSend);

            Thread.Sleep(updateMills);
        }
    }
}
