# FpsToOscAvatarParam

Шлет через OSC параметр от `0f` до `1f` в зависимости от FPS таргетируемого приложения.
Параметры задаются через конфиг `appsettings.json`:
```json
{
  "AvatarParameterUrl": "/path1/path2/path3",
  "TargetProcess": "ProcessName",
  "MaxFps": 60,
  "UpdateMills": 50
}
```
где:
+ `AvatarParameterUrl` - OSC url;
+ `TargetProcess` - процесс, по которому будет отслеживаться FPS;
+ `MaxFps` - максимальный FPS;
+ `UpdateMills` - время в миллисекундах, через которое будет совершено обновление.

Обновляет параметр каждые `UpdateMills` миллисекунд плавно, но FPS по `TargetProcess` - каждую секунду.
Конфиг `appsettings.json` можно обновлять прямо во время работы приложения, он сразу применится.

## Build
Нужен [.NET SDK 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
```
dotnet build
```

## Run
```
dotnet run
```

## External dependencies
[dotnet runtime](https://github.com/dotnet/runtime) - MIT
[CoreOSC](https://github.com/dastevens/CoreOSC) - MIT
[perfview](https://github.com/Microsoft/perfview) - MIT
