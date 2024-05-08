using DiscordRPC;
using DiscordRPC.Message;

namespace Pong.Game;

// The UWP class must have all the public methods as this class. This will be called from the shared project.
public class PlatformSpecific
{
    private const string ClientId = "1226792720207450143";
    
    public DiscordRpcClient Client { get; private set; }

    public PlatformSpecific()
    {
        Client = new DiscordRpcClient(ClientId)
        {
            SkipIdenticalPresence = true
        };

        Client.OnReady += RpcReady;
    }

    private void RpcReady(object e, ReadyMessage message)
    {
        if (Client.CurrentPresence != null)
            Client.SetPresence(null);
    }

    public void UpdateRpc(int scoreLeft, int scoreRight)
    {
        
    }

    public void UpdateFinished(int scoreLeft, int scoreRight)
    {
        UpdateRpc(scoreLeft, scoreRight);
    }
}