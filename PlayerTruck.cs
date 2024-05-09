using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace pizza_spy
{
    internal class PlayerTruck
    {

        public Rectangle CollisionRect;
        private Texture2D _txr;
        private bool _goingRight,_goingUp;
        public float speed;


        public PlayerTruck(Texture2D txr, int xPos, int yPos)
        {
            _txr = txr;
            CollisionRect = new Rectangle(xPos, yPos, _txr.Width, _txr.Height);
            speed = 12f;
        }


        public void UpdateMe(GamePadState pad)
        {
            if (pad.ThumbSticks.Left.X < 0 && CollisionRect.X > 0 || Keyboard.GetState().IsKeyDown(Keys.Left) && CollisionRect.X > 0)
            {
                _goingRight = false;
                CollisionRect.X -= (int)speed;
            }

            else if (pad.ThumbSticks.Left.X > 0 && CollisionRect.X < 750 - _txr.Width || Keyboard.GetState().IsKeyDown(Keys.Right) && CollisionRect.X < 750 - _txr.Width)
            {
                _goingRight = true;
                CollisionRect.X += (int)speed;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Up) && CollisionRect.Y >= 0)
            {

                _goingUp = false;
                CollisionRect.Y -= (int)speed;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Down) && CollisionRect.Y < 1024 - _txr.Height)
            {
                _goingUp = true;
                CollisionRect.Y += (int)speed;
            }
        }

        public void DrawMe(SpriteBatch sb)
        {
            if (_goingUp)        
                sb.Draw(_txr, CollisionRect, null, Color.White, 0, Vector2.Zero, SpriteEffects.FlipVertically, 0);
            else
                sb.Draw(_txr, CollisionRect, Color.White);


            //sb.Draw(Game1.pixel, CollisionRect, Color.PaleGreen * 0.5f);

        }

    }
}
