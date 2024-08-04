using CoreOSC;
using CoreOSC.IO;
using System.Net.Sockets;

namespace FpsToOscAvatarParam;

public sealed class AvatarOscSender : IAvatarValueSender, IDisposable
{
    private readonly UdpClient _udpClient;
    private string _address = string.Empty;

    public AvatarOscSender()
    {
        _udpClient = new UdpClient("127.0.0.1", 9000);
    }

    public async Task SendValue(float value)
    {
        var message = new OscMessage(new Address(_address),
        [
            value
        ]);

        await _udpClient.SendMessageAsync(message);
    }

    public void SetAddress(string address)
    {
        _address = string.IsNullOrWhiteSpace(address) ? string.Empty : address;
    }

    public void Dispose()
    {
        _udpClient.Dispose();
        GC.SuppressFinalize(this);
    }
}
