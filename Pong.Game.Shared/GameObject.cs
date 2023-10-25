using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Xml.Serialization;

namespace Pong.Game
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
            spriteBatch.Draw(Texture, Position, null, Color.White, 0, new Vector2(Texture.Width / 2, Texture.Height / 2), Scale, SpriteEffects.None, 0);
        }

        public void Move(float x, float y)
        {
            x += Position.X;
            y += Position.Y;            
            
            Position = new Vector2(x, y);
        }

        public void MoveByVelocity()
        {
            float x = Position.X + Velocity.X;
            float y = Position.Y + Velocity.Y;

            Position = new Vector2(x, y);
        }

        /// <summary>
        /// Moves and checks if the object is out of the screen with the given width and height.
        /// </summary>
        public void MoveNoOOS (float x, float y, float screenWidth, float screenHeight)
        {
            Move(x, y);

            NoOOS(screenWidth, screenHeight);
        }

        /// <summary>
        /// Checks whether or not the object is out of the screen.
        /// </summary>
        public void NoOOS(float screenWidth, float screenHeight)
        {
            float x = Position.X;
            float y = Position.Y;

            if (x - (Texture.Width * Scale / 2) < 0) x = Texture.Width * Scale / 2;
            if (y - (Texture.Height * Scale / 2) < 0) y = Texture.Height * Scale / 2;
            if (x + (Texture.Width / 2 * Scale) > screenWidth) x = screenWidth - (Texture.Width * Scale / 2);
            if (y + (Texture.Height / 2 * Scale) > screenHeight) y = screenHeight - (Texture.Height * Scale / 2);

            Position = new Vector2(x, y);
        }
    }
}
