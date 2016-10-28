using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XNA_Lookat
{
    class Bullet
    {
        public Texture2D Texture;
        public Vector2 Position;
        public Vector2 Velocity;
        public Vector2 Origin = Vector2.Zero;
        public bool isVisible;

        public Bullet(Texture2D newTexture)
        {
            Texture = newTexture;
            isVisible = false;
        }

        public void Draw(SpriteBatch spriteBatch,float rotation)
        {
            spriteBatch.Draw(Texture, Position, null, Color.White, rotation, Origin,1f, SpriteEffects.None, 0f);
        }
    }
}
