using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XNA_Lookat
{
    //Camera class so we can follow the X coordinate of the sprite !!
    class Camera
    {
        public Matrix transform;
        Viewport view;
        Vector2 centre;

        public Camera(Viewport newview)
        {
            view = newview;
        }

        public void Update(Vector2 pos,Rectangle rect)
        {
            centre = new Vector2(pos.X + (rect.Width / 2) - 512,pos.Y + (rect.Height / 2) - 300);
            transform = Matrix.CreateScale(new Vector3(1, 1, 0)) * Matrix.CreateTranslation(new Vector3(-centre.X, -centre.Y, 0));
        }
    }
}
