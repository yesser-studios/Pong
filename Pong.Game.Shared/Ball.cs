using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Pong.Game
{
    public class Ball : GameObject
    {
        #region Variables
        public const float BounceSpeedUp = 1f;

        private readonly GameObject _leftPad;
        private readonly GameObject _rightPad;

        private ScreenSide _lastTouched = ScreenSide.Center;

        public delegate void OnBallBounced(ScreenSide bounceSide);
        public event OnBallBounced OnBallBouncedEvent;
        
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
            this._leftPad = leftPad;
            this._rightPad = rightPad;
        }

        #region Movement
        public override void MoveByVelocity()
        {
            base.MoveByVelocity();

            Bounce();
        }

        protected void Bounce()
        {
            var side = CheckOos();

            var newVelocity = Velocity;

            if (side == ScreenSide.Bottom)
                newVelocity.Y = -MathF.Abs(newVelocity.Y)
                    - (_lastTouched != ScreenSide.Bottom ?
                        BounceSpeedUp
                        : 0);
            else if (side == ScreenSide.Top)
                newVelocity.Y = MathF.Abs(newVelocity.Y)
                    + (_lastTouched != ScreenSide.Top ?
                        BounceSpeedUp
                        : 0);

            if (CollidesWith(_leftPad))
            {
                newVelocity.X = MathF.Abs(newVelocity.X)
                    + (_lastTouched != ScreenSide.Left ?
                        BounceSpeedUp
                        : 0);
                side = ScreenSide.Left;
            }
            else if (CollidesWith(_rightPad))
            {
                newVelocity.X = -MathF.Abs(newVelocity.X)
                    - (_lastTouched != ScreenSide.Right ?
                        BounceSpeedUp
                        : 0);
                side = ScreenSide.Right;
            }

            if (side != ScreenSide.Center)
                _lastTouched = side;

            Velocity = newVelocity;

            if (side != ScreenSide.Center)
                OnBallBouncedEvent?.Invoke(side);
        }
        #endregion

        /// <summary>
        /// Checks whether the ball surpassed the left or right edge of the screen.
        /// </summary>
        public ScreenSide CheckScored()
        {
            if (X - (Width / 2) > ScreenWidth)
                return ScreenSide.Right;
            if (X + (Width / 2) < 0)
                return ScreenSide.Left;

            return ScreenSide.Center;
        }
    }
}
