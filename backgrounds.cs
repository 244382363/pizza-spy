using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace pizza_spy
{
    class background
    {
        Texture2D _tex;

        Rectangle _rect;


        //can be used for background that uses vector
        public background(Texture2D tex)
        {
            _tex = tex;
            _rect = new Rectangle(0, 0, 750, 1024);
        }
        //can be used for background that does not use vector

        //lines above uses constructor overload
        public void DrawMe(SpriteBatch sb)
        {
            sb.Draw(_tex, _rect, Color.White);

        }
    }
}
