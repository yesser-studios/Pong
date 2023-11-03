using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Pong.Game
{
    public class Ball : GameObject
    {
        #region Variables
        const float BOUNCE_SPEED_UP = 1f;

        GameObject leftPad;
        GameObject rightPad;

        ScreenSide lastTouched = ScreenSide.Center;
        #endregion

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

            if (side == ScreenSide.Bottom)
                newVelocity.Y = -MathF.Abs(newVelocity.Y)
                    - (lastTouched != ScreenSide.Bottom ?
                        BOUNCE_SPEED_UP
                        : 0);
            else if (side == ScreenSide.Top)
                newVelocity.Y = MathF.Abs(newVelocity.Y)
                    + (lastTouched != ScreenSide.Top ?
                        BOUNCE_SPEED_UP
                        : 0);

            if (CollidesWith(leftPad))
            {
                newVelocity.X = MathF.Abs(newVelocity.X)
                    + (lastTouched != ScreenSide.Left ?
                        BOUNCE_SPEED_UP
                        : 0);
                side = ScreenSide.Left;
            }
            else if (CollidesWith(rightPad))
            {
                newVelocity.X = -MathF.Abs(newVelocity.X)
                    - (lastTouched != ScreenSide.Right ?
                        BOUNCE_SPEED_UP
                        : 0);
                side = ScreenSide.Right;
            }

            if (side != ScreenSide.Center)
                lastTouched = side;

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
