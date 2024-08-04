namespace FpsToOscAvatarParam;

public interface IAvatarValueSender
{
    void SetAddress(string address);
    Task SendValue(float value);
}
