using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.Direct3D9;
using System;
namespace pizza_spy
{
    enum ProjectileState
    {
        Shooting,
        Crashed
    }
    class PizzaProjectile
    {
        ProjectileState _currState;
        public Rectangle CollisionRect;
        Texture2D _txr;
        public bool IsActive;




        public PizzaProjectile(Texture2D txr, int xPos, int yPos)
        {
            //start falling
            _currState = ProjectileState.Shooting;

            _txr = txr;
            IsActive = true;
            CollisionRect = new Rectangle(xPos, yPos, txr.Width, txr.Height);

        }


        public ProjectileState GetState()
        {
            return _currState;
        }
        public void UpdateMe(int minY)
        {
            
            
                // Move the projectile upwards
           CollisionRect.Y -= 20;

                // Deactivate if it goes off screen
           if (CollisionRect.Y + _txr.Height < 0)
           {
                IsActive = false;
           }
                
           if (CollisionRect.Y > minY)
           {
                _currState = ProjectileState.Crashed;
           }
            
                 
            

        }

        public void DrawMe(SpriteBatch sb)
        {
            if(IsActive)
            {
                sb.Draw(_txr, CollisionRect, Color.White);
            }
            
        }
    }
}

