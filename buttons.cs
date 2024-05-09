using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace pizza_spy
{
    class buttons
    {

        private Rectangle _rect;
        private Texture2D st_SpriteSheet;
        private Vector2 st_position;
        private Rectangle st_animCell;
        public Rectangle CollisionRect;
        private float st_frameTimer;
        private float st_fps;
        public bool anim_button;

        public buttons(Texture2D spriteSheet, int xpos, int ypos, int frameCount, int fps, int an_button)
        {
            st_SpriteSheet = spriteSheet;
            CollisionRect = new Rectangle(xpos, ypos,
                st_SpriteSheet.Width / frameCount, spriteSheet.Height);
            if (an_button == 1)
            {
                anim_button = true;

                st_animCell = new Rectangle(0, 0, st_SpriteSheet.Width / frameCount, spriteSheet.Height);
                st_position = new Vector2(xpos, ypos);
                st_frameTimer = 1;
                st_fps = fps;

                CollisionRect = new Rectangle(xpos, ypos,
                    st_SpriteSheet.Width / frameCount, spriteSheet.Height);
            }
            else if (an_button == 0)
            {
                anim_button = false;

                st_animCell = new Rectangle(0, 0, st_SpriteSheet.Width, spriteSheet.Height);
                st_position = new Vector2(xpos, ypos);
            }

            
        }

        public void Clicked()
        {
            st_frameTimer = 16;
        }

        public void DrawMe(SpriteBatch sb, GameTime gt)
        {
            if (anim_button == true)
            {

                if (CollisionRect.Contains(Mouse.GetState().X, Mouse.GetState().Y) && st_frameTimer <= 0)//if animated buttons rectangle contains mouse
                {
                    st_animCell.X = (st_animCell.X + st_animCell.Width);//shifts the frame of the buttons sprite sheet by 1 each frame
                    if (st_animCell.X >= st_SpriteSheet.Width)
                        st_animCell.X = 0;



                    st_frameTimer = 1;
                }

                else
                {



                    st_frameTimer -= (float)gt.ElapsedGameTime.TotalSeconds * st_fps;

                }
                if (!CollisionRect.Contains(Mouse.GetState().X, Mouse.GetState().Y))//if the animated button doesnt contain the mouse rectangle
                {
                    st_animCell.X = 0;
                    sb.Draw(st_SpriteSheet, st_position, st_animCell, Color.White);//changes the frame of the button sprite sheet back to the first
                }
                else
                    sb.Draw(st_SpriteSheet, st_position, st_animCell, Color.White);
            }
            if (!anim_button)
            {
                if (CollisionRect.Contains(Mouse.GetState().X, Mouse.GetState().Y))//if the non animated buttons rectangle contains mouse changes the brightness of the button by 50%
                {
                    sb.Draw(st_SpriteSheet, st_position, st_animCell, Color.White * 0.5f);
                }
                else
                {
                    sb.Draw(st_SpriteSheet, st_position, st_animCell, Color.White);
                }
            }

        }
    }
}
