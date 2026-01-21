using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pong.Game
{
    public class GameObject(
        Texture2D texture,
        Vector2 position,
        float scale,
        SpriteBatch spriteBatch,
        float screenWidth,
        float screenHeight)
    {
        #region Variables
        protected readonly float ScreenWidth = screenWidth;

        public Texture2D Texture { get; } = texture;
        public Vector2 Position { get; set; } = position;
        public float Scale { get; } = scale;
        public Vector2 Velocity {  get; set; } = Vector2.Zero;

        public float Width => Texture.Width * Scale;
        public float Height => Texture.Height * Scale;

        // ReSharper disable once MemberCanBeProtected.Global
        public float X => Position.X;
        public float Y => Position.Y;

        #endregion

        public void Draw()
            => spriteBatch.Draw(
                Texture,
                Position,
                null,
                Color.White,
                0,
                // ReSharper disable PossibleLossOfFraction
                new Vector2(Texture.Width / 2, Texture.Height / 2),
                // ReSharper restore PossibleLossOfFraction
                Scale,
                SpriteEffects.None,
                0);

        #region Movement

        private void Move(float x, float y)
        {
            x += X;
            y += Y;            
            
            Position = new Vector2(x, y);
        }

        public virtual void MoveByVelocity()
        {
            var x = X + Velocity.X;
            var y = Y + Velocity.Y;

            Position = new Vector2(x, y);
        }

        /// <summary>
        /// Moves and checks if the object is out of the screen with the given width and height.
        /// </summary>
        public void MoveNoOos (float x, float y)
        {
            Move(x, y);

            MoveOnScreen();
        }

        /// <summary>
        /// Checks whether the object is out of the screen.
        /// If so, it will move the object on the screen.
        /// </summary>
        private void MoveOnScreen()
        {
            var x = X;
            var y = Y;

            var screenSide = CheckOos();

            if (screenSide == ScreenSide.Left) x = Width / 2;
            if (screenSide == ScreenSide.Top) y = Height / 2;
            if (screenSide == ScreenSide.Right) x = ScreenWidth - (Width / 2);
            if (screenSide == ScreenSide.Bottom) y = screenHeight - (Height / 2);

            Position = new Vector2(x, y);
        }
        #endregion

        #region Checks
        public ScreenSide CheckOos()
        {
            var outOfScreen = ScreenSide.Center;
            if (X - (Width / 2) < 0) outOfScreen = ScreenSide.Left;
            if (Y - (Height / 2) < 0) outOfScreen = ScreenSide.Top;
            if (X + (Width / 2) > ScreenWidth) outOfScreen = ScreenSide.Right;
            if (Y + (Height / 2) > screenHeight) outOfScreen = ScreenSide.Bottom;

            return outOfScreen;
        }

        protected bool CollidesWith(GameObject other)
        {
            if (other == null) return false;

            return (X + (Width / 2) >= other.X - (other.Width / 2))
                && (other.X + (other.Width / 2) >= X - (Width / 2))
                && (Y + (Height / 2) >= other.Y - (other.Height / 2))
                && (other.Y + (other.Height / 2) >= Y - (Height / 2));
        }
        #endregion
    }
}
