using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace pizza_spy
{
    internal class PlayerTruck
    {

        public Rectangle CollisionRect;
        private Texture2D _trucktex;
        private bool _goingRight,_goingUp;
        public float speed;

        public PlayerTruck(Texture2D truckTexture, int xPos, int yPos)
        {
            _trucktex = truckTexture;
            CollisionRect = new Rectangle(xPos, yPos, truckTexture.Width, truckTexture.Height);
            speed = 12f;
        }

        
    



        public void UpdateMe(GamePadState pad, KeyboardState keyState)
        {
            if (pad.ThumbSticks.Left.X < 0 && CollisionRect.X > 0 || Keyboard.GetState().IsKeyDown(Keys.Left) && CollisionRect.X > 0)
            {
                _goingRight = false;
                CollisionRect.X -= (int)speed;
            }

            else if (pad.ThumbSticks.Left.X > 0 && CollisionRect.X < 750 - _trucktex.Width || Keyboard.GetState().IsKeyDown(Keys.Right) && CollisionRect.X < 750 - _trucktex.Width)
            {
                _goingRight = true;
                CollisionRect.X += (int)speed;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Up) && CollisionRect.Y >= 0)
            {

                _goingUp = false;
                CollisionRect.Y -= (int)speed;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Down) && CollisionRect.Y < 1024 - _trucktex.Height)
            {
                _goingUp = true;
                CollisionRect.Y += (int)speed;
            }

            
        }

        public void DrawMe(SpriteBatch sb)
        {
        if (_goingUp)
            sb.Draw(_trucktex, CollisionRect, null, Color.White, 0, Vector2.Zero, SpriteEffects.FlipVertically, 0);
        else
            sb.Draw(_trucktex, CollisionRect, Color.White);
       


        //sb.Draw(Game1.pixel, CollisionRect, Color.PaleGreen * 0.5f);
        }



    }
}
