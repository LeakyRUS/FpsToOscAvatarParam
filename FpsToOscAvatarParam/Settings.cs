namespace FpsToOscAvatarParam;

public class Settings
{
    public string AvatarParameterUrl { get; set; } = null!;
    public string TargetProcess { get; set; } = null!;
    public int MaxFps {  get; set; }
    public int UpdateMills { get; set; }
}
