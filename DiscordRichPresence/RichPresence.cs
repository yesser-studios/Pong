namespace DiscordRichPresence;

public class RichPresence
{
    public const long CLIENT_ID = 1226792720207450143;
    
    public readonly Discord.Discord DiscordObject = new(CLIENT_ID, (ulong)Discord.CreateFlags.Default);

    public void ApplyChanges()
    {
        DiscordObject.RunCallbacks();
    }
}