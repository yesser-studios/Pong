using DiscordRichPresence;

namespace Pong.Game;

// The UWP class must have all the public methods as this class. This will be called from the shared project.
public class PlatformSpecific
{
    private readonly RichPresence _presence = new RichPresence();
    
    public void UpdateFinished()
    {
        _presence.ApplyChanges();
    }
}