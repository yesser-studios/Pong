using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pong.Game
{
    public class Ball : GameObject
    {
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
                newVelocity.Y = -newVelocity.Y;

            if (CollidesWith(leftPad))
                newVelocity.X = MathF.Abs(newVelocity.X);

            if (CollidesWith(rightPad))
                newVelocity.X = -MathF.Abs(newVelocity.X);

            Velocity = newVelocity;
        }
    }
}
