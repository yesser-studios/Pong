using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Pong.Game
{
    public class Ball : GameObject
    {
        const float BOUNCE_SPEED_UP = 1f;

        GameObject leftPad;
        GameObject rightPad;

        public Ball(
            Texture2D texture,
            Vector2 position,
            float scale,
            SpriteBatch spriteBatch,
            float screenWidth,
            float screenHeight,
            Vector2 initialVelocity,
            GameObject leftPad,
            GameObject rightPad) : base(
                texture,
                position,
                scale,
                spriteBatch,
                screenWidth,
                screenHeight)
        {

            Velocity = initialVelocity;
            this.leftPad = leftPad;
            this.rightPad = rightPad;
        }

        public override void MoveByVelocity()
        {
            base.MoveByVelocity();

            Bounce();
        }

        protected void Bounce()
        {
            ScreenSide side = CheckOOS();

            Vector2 newVelocity = Velocity;

            if (side == ScreenSide.Bottom
                || side == ScreenSide.Top)
                newVelocity.Y = -(newVelocity.Y + BOUNCE_SPEED_UP);

            if (CollidesWith(leftPad))
                newVelocity.X = MathF.Abs(newVelocity.X)
                    + BOUNCE_SPEED_UP;
            
            if (CollidesWith(rightPad))
                newVelocity.X = -MathF.Abs(newVelocity.X)
                    - BOUNCE_SPEED_UP;

            Velocity = newVelocity;
        }
        
        /// <summary>
        /// Checks whether the ball surpassed the left or right edge of the screen.
        /// </summary>
        public ScreenSide CheckScored()
        {
            if (X - (Width / 2) > screenWidth)
                return ScreenSide.Right;
            if (X + (Width / 2) < 0)
                return ScreenSide.Left;

            return ScreenSide.Center;
        }
    }
}
