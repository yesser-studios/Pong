using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pong
{
    internal class Ball : GameObject
    {
        public Ball(Texture2D texture, Vector2 position, float scale, SpriteBatch spriteBatch) : base(texture, position, scale, spriteBatch)
        { }
    }
}
