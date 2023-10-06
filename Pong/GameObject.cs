using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pong
{
    internal class GameObject
    {
        private readonly SpriteBatch spriteBatch;

        public Texture2D Texture { get; }
        public Vector2 Position { get; set; }
        public float Scale { get; }
        public Vector2 Velocity {  get; set; } = Vector2.Zero;

        public GameObject(Texture2D texture, Vector2 position, float scale, SpriteBatch spriteBatch)
        {
            Texture = texture;
            Position = position;
            Scale = scale;
            this.spriteBatch = spriteBatch;
        }

        public void Draw()
        {
            spriteBatch.Draw(Texture, Position, null, Color.White, 0, new(Texture.Width / 2, Texture.Height / 2), Scale, SpriteEffects.None, 0);
        }
    }
}
