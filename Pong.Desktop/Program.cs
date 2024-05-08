using Pong.Game;

var platformSpecific = new PlatformSpecific();

using var game = new Game1(platformSpecific);
game.Run();

platformSpecific.Closing();
