namespace FpsToOscAvatarParam;

public interface IOSCAvatarSingleValueSender
{
    void SetAddress(string address);
    Task SendValue(float value);
}
