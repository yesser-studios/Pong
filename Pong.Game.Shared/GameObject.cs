using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pong.Game
{
    public enum ScreenSide
    {
        Top,
        Bottom,
        Left,
        Right,
        Center
    }

    public class GameObject
    {
        protected readonly float screenWidth;
        protected readonly float screenHeight;
        protected readonly SpriteBatch spriteBatch;

        public Texture2D Texture { get; }
        public Vector2 Position { get; set; }
        public float Scale { get; }
        public Vector2 Velocity {  get; set; } = Vector2.Zero;

        public float Width { get => Texture.Width * Scale; }
        public float Height { get => Texture.Height * Scale; }
        public float X { get => Position.X; }
        public float Y { get => Position.Y; }

        public GameObject(
            Texture2D texture,
            Vector2 position,
            float scale,
            SpriteBatch spriteBatch,
            float screenWidth,
            float screenHeight)
        {
            Texture = texture;
            Position = position;
            Scale = scale;
            this.spriteBatch = spriteBatch;
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
        }

        public void Draw()
            => spriteBatch.Draw(
                Texture,
                Position,
                null,
                Color.White,
                0,
                new Vector2(Texture.Width / 2, Texture.Height / 2),
                Scale,
                SpriteEffects.None,
                0);

        public void Move(float x, float y)
        {
            x += Position.X;
            y += Position.Y;            
            
            Position = new Vector2(x, y);
        }

        public virtual void MoveByVelocity()
        {
            float x = Position.X + Velocity.X;
            float y = Position.Y + Velocity.Y;

            Position = new Vector2(x, y);
        }

        /// <summary>
        /// Moves and checks if the object is out of the screen with the given width and height.
        /// </summary>
        public void MoveNoOOS (float x, float y)
        {
            Move(x, y);

            MoveOnScreen();
        }

        /// <summary>
        /// Checks whether or not the object is out of the screen.
        /// </summary>
        public void MoveOnScreen()
        {
            float x = Position.X;
            float y = Position.Y;

            ScreenSide screenSide = CheckOOS();

            if (screenSide == ScreenSide.Left) x = Texture.Width * Scale / 2;
            if (screenSide == ScreenSide.Top) y = Texture.Height * Scale / 2;
            if (screenSide == ScreenSide.Right) x = screenWidth - (Texture.Width * Scale / 2);
            if (screenSide == ScreenSide.Bottom) y = screenHeight - (Texture.Height * Scale / 2);

            Position = new Vector2(x, y);
        }

        public ScreenSide CheckOOS()
        {
            ScreenSide outOfScreen = ScreenSide.Center;
            if (Position.X - (Texture.Width * Scale / 2) < 0) outOfScreen = ScreenSide.Left;
            if (Position.Y - (Texture.Height * Scale / 2) < 0) outOfScreen = ScreenSide.Top;
            if (Position.X + (Texture.Width / 2 * Scale) > screenWidth) outOfScreen = ScreenSide.Right;
            if (Position.Y + (Texture.Height / 2 * Scale) > screenHeight) outOfScreen = ScreenSide.Bottom;

            return outOfScreen;
        }

        public bool CollidesWith(GameObject other)
        {
            if (other == null) return false;

            return (X + Width >= other.X) && (other.X + other.Width >= X) && (Y + Height >= other.Y) && (other.Y + other.Height >= Y);
        }
    }
}
