# FpsToOscAvatarParam

Шлет через OSC параметр от `0f` до `1f` в зависимости от FPS таргетируемого приложения.
Параметры задаются через конфиг:
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
+ `TargetProcess` - процесс, по которорому будет олтслеживаться FPS;
+ `MaxFps` - максимальный FPS;
+ `UpdateMills` - время в миллисекундах, через которое будет совершено обновление.

Обновляет параметр каждые `UpdateMills` миллисекунд плавно, но FPS по `TargetProcess` - каждую секунду.

## Build
```
dotnet build
```

## Run
```
dotnet run
```
