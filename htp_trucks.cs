using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace pizza_spy
{
    internal class htp_trucks
    {
        public Rectangle Rect;
        private Texture2D _txr;


        public htp_trucks(Texture2D txr, int xPos, int yPos)
        {
            _txr = txr;
            Rect = new Rectangle(xPos, yPos, _txr.Width, _txr.Height);
        }

        public void UpdateMe() 
        {
        
        }

        public void DrawMe(SpriteBatch sb)
        {
            sb.Draw(_txr, Rect, Color.White);
        }

    }
}
