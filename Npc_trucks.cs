﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace pizza_spy
{
    enum TruckState
    {
        Falling,
        Crashed
    }
    class Npc_trucks
    {
        TruckState _currState;
        public Rectangle Rect;
        Texture2D _txr;
        string _text;
        Vector2 _pos;
        Vector2 _vel;
        public bool enemyTruck;



        public Npc_trucks(Texture2D txr, int MaxX, int enemyT)
        {
            //start falling
            _currState = TruckState.Falling;

            _txr = txr;

            //pick a random point along the "top line" to start
            _pos = new Vector2(Game1.RNG.Next(50, 700), 0);
            Rect = new Rectangle(_pos.ToPoint(), txr.Bounds.Size);

            //pick a new random velocity to fall with
            _vel = new Vector2(0, (float)Game1.RNG.NextDouble() * 2 + 0.5f);
            /*if (santa == 7)
            {
                santaBauble = true;
            }
            else
            {
                santaBauble = false;
            }*/
        }


        public TruckState GetState()
        {
            return _currState;
        }
        public void UpdateMe(int maxY)
        {
            //move the bauble by it's velocity
            _pos += _vel;

            //if it's at the bottom, change to "crashes" state
            if (_pos.Y > maxY)
                _currState = TruckState.Crashed;

            //make sure the rectangle follows the position
            Rect.Location = _pos.ToPoint();

        }

        public void DrawMe(SpriteBatch sb)
        {
            sb.Draw(_txr, Rect, Color.White);
        }
    }
}