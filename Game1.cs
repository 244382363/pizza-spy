using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace pizza_spy
{
    public class Game1 : Game
    {

        enum GameStates //set up gamestates
        {
            St_screen,
            Skin_select,
            Htp_screen,
            cut_scene1,
            pause_screen,
            game_overScreen,
            Gameplayscreen,
            GameWon_screen

        }

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private MouseState _mouseState;
        KeyboardState kb, oldkb;
        RenderTarget2D DrawCanvas;
        Vector2 CanvasLocation;
        int bouncesLeft;
        SoundEffect bgm;


        public static readonly Random RNG = new Random();
        GamePadState padcurr;

        GameStates _currState, _oldState;

        SpriteFont debugFont, GuideFont;
        background start_screen;
        buttons st_button, htp_button;
        PlayerTruck player_truck;
        List<Npc_trucks> npcTrucks;
        float spawnRate, timeTillSpawn;
        int trucksPerSpawn, enemyTruck;

        const int NOOFSC_BACKGROUNDS = 2;
        const float BASESPAWNRATE = 5;

        struct scrollingBG
        {
            public Rectangle _rect;
            public Texture2D _txr;
        }
        scrollingBG[] bgds;
        

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _graphics.PreferredBackBufferWidth = 750;
            _graphics.PreferredBackBufferHeight = 1024;
        }

        protected override void Initialize()
        {


            bgds = new scrollingBG[NOOFSC_BACKGROUNDS];
            //DrawCanvas = new RenderTarget2D(_graphics.GraphicsDevice, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
            //CanvasLocation = Vector2.Zero;
            // TODO: Add your initialization logic here
            for (int i = 0; i < NOOFSC_BACKGROUNDS; i++)
            {
                // Set their X, Y, Height and Width
                bgds[i]._rect = new Rectangle(0, i * -1024, 750, 1024);
            }
            npcTrucks = new List<Npc_trucks>();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            debugFont = Content.Load<SpriteFont>("Arial07");
            for (int i = 0; i < NOOFSC_BACKGROUNDS; i++)
            {
                bgds[i]._txr = Content.Load<Texture2D>("sc_background" + i);
            }

            start_screen = new background(Content.Load<Texture2D>("start_screen_bgd"));
            st_button = new buttons(Content.Load<Texture2D>("start_button"), 250, 500, 2, 24, 1);
            htp_button = new buttons(Content.Load<Texture2D>("how to play button"), 250, 600, 2, 24, 1);
            player_truck = new PlayerTruck(Content.Load<Texture2D>("player truck"), 400, 800);
            for (int i = 0; i < 1; i++)
            {
                Content.Load<Texture2D>("truck" + i);

            }


        }

        protected override void Update(GameTime gameTime)
        {
            _mouseState = Mouse.GetState();
            kb = Keyboard.GetState();

            padcurr = GamePad.GetState(PlayerIndex.One);
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            

            

            // Move the backgrounds downward
            for (int i = 0; i < NOOFSC_BACKGROUNDS; i++)
            {
                bgds[i]._rect.Y += 5;  // Increment Y to move the background down
            }

            // Go through the backgrounds one by one
            for (int i = 0; i < NOOFSC_BACKGROUNDS; i++)
            {
                // Check if the current background has completely moved past the bottom of the screen
                if (bgds[i]._rect.Y > 1024)  // Use screenHeight to determine when to reset
                {
                    // Find the topmost Y position of the current backgrounds to stack this one right above the highest
                    int highestY = int.MaxValue;
                    for (int j = 0; j < NOOFSC_BACKGROUNDS; j++)
                    {
                        if (bgds[j]._rect.Y < highestY && i != j)
                        {
                            highestY = bgds[j]._rect.Y;
                        }
                    }

                    // Reset the background to appear above the topmost visible background
                    bgds[i]._rect.Y = highestY - 1024;  // This resets the Y position to just above the highest visible background
                }
            }

            for (int i = 0; i < npcTrucks.Count; i++)
            {
                if (npcTrucks[i].Rect.Intersects(player_truck.CollisionRect))
                {
                    if (npcTrucks[i].enemyTruck == true)
                    {
                        player_truck.speed = player_truck.speed * 1.2f;

                    }
                    npcTrucks.RemoveAt(i);
                    break;
                }
            }
            if (timeTillSpawn < 0)
            {
                //add as many baubles as we are currently allowed
                for (int i = 0; i < trucksPerSpawn; i++)
                {
                    enemyTruck = RNG.Next(0, 3);
                    npcTrucks.Add(new pizza_spy.Npc_trucks(Content.Load<Texture2D>("truck" + enemyTruck),
                    _graphics.PreferredBackBufferWidth, enemyTruck));
                }

                //if the current spawn rate is half the base, it's time to reset and add an extra bauble
                if (spawnRate < BASESPAWNRATE / 2)
                {
                    spawnRate = BASESPAWNRATE;
                    trucksPerSpawn++;
                }
                else
                {
                    //decrease the spawnRate so the next one will come slightly faster
                    spawnRate -= 0.2f;
                }

                timeTillSpawn = spawnRate;
            }

            else
            {
                timeTillSpawn -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }


            for (int i = 0; i < npcTrucks.Count; i++)
            {
                /*baubles.Add(new baubles(Content.Load<Texture2D>("bauble" + RNG.Next(0, 8)),
                    _graphics.PreferredBackBufferWidth));*/
                if (npcTrucks[i].GetState() == TruckState.Crashed)
                {
                    npcTrucks.RemoveAt(i);
                    break;
                }
                npcTrucks[i].UpdateMe(_graphics.PreferredBackBufferHeight);
            }














            switch (_currState)
            {
                case GameStates.St_screen:
                    st_screenUpdate(_mouseState);
                    break;
                case GameStates.Gameplayscreen:
                    GameplayscreenUpdate(_mouseState);
                    break;

                    
            }
            player_truck.UpdateMe(padcurr);
            
            oldkb = kb;
            base.Update(gameTime);
        }
        void st_screenUpdate(MouseState ms)
        {
            if(st_button.CollisionRect.Contains(Mouse.GetState().X, Mouse.GetState().Y)&& _mouseState.LeftButton == ButtonState.Pressed)
            {
                _currState = GameStates.Gameplayscreen;
            }
        }
        void GameplayscreenUpdate(MouseState ms)
        {

        }

        protected override void Draw(GameTime gameTime)
        {
            switch (_currState)
            {
                case GameStates.St_screen:
                    st_screenDraw(gameTime);
                    break;
                case GameStates.Gameplayscreen:
                    GameplayscreenDraw(gameTime);
                    break;
            }
            base.Draw(gameTime);  
        }

        void st_screenDraw(GameTime gameTime) //draw methods for start screen
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();

            start_screen.DrawMe(_spriteBatch);
            st_button.DrawMe(_spriteBatch, gameTime);
            htp_button.DrawMe(_spriteBatch, gameTime);
            

            _spriteBatch.End();

        }
        void GameplayscreenDraw(GameTime gameTime)
        {
            _spriteBatch.Begin();
            // go through the backgrounds, drawing each one in its position
            for (int i = 0; i < NOOFSC_BACKGROUNDS; i++)
                _spriteBatch.Draw(bgds[i]._txr, bgds[i]._rect, Color.White);

            for (int i = 0;i < npcTrucks.Count; i++)
            {
                npcTrucks[i].DrawMe(_spriteBatch);
            }
            _spriteBatch.DrawString(debugFont, "Res: " + _graphics.PreferredBackBufferWidth
                                              + " x " + _graphics.PreferredBackBufferHeight,
                                              Vector2.Zero, Color.White);
            _spriteBatch.DrawString(debugFont, "Sleigh: " + player_truck.CollisionRect, new Vector2(0, 12), Color.White);
            _spriteBatch.DrawString(debugFont, "Number of baubles: " + npcTrucks.Count, new Vector2(0, 36), Color.White);
            _spriteBatch.DrawString(debugFont, "Baubles per spawn: " + trucksPerSpawn + " Spawn in: " + timeTillSpawn, new Vector2(0, 48), Color.White);
            _spriteBatch.DrawString(debugFont, "Current Speed " + player_truck.speed, new Vector2(0, 60), Color.White);
            
            player_truck.DrawMe(_spriteBatch);
            _spriteBatch.End();
        }
    }
}
