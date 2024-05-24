using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace pizza_spy
{
    public class Game1 : Game
    {

        enum GameStates //set up gamestates
        {
            St_screen,
            Htp_screen,
            pause_screen,
            game_overScreen,
            Gameplayscreen,
            

        }

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private MouseState _mouseState;
        KeyboardState kb, oldkb;
        //RenderTarget2D DrawCanvas;
        //Vector2 CanvasLocation;
        
        int PlayerScore;
        SoundEffect bgm, shoot_pizza;


        public static readonly Random RNG = new Random();
        GamePadState padcurr;

        GameStates _currState, _oldState;

        SpriteFont debugFont;
        background start_screen, htp_screen;
        buttons st_button, htp_button,bk_button,resume_button,pause_screen,game_over_screen,exit_button;
        PlayerTruck player_truck;
        List<Npc_trucks> npcTrucks;
        List<htp_trucks> htp_Trucks;    
        List<PizzaProjectile> _projectile;
        float spawnRate, timeTillSpawn;
        int trucksPerSpawn, enemyTruck, motorbike;

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
            
            PlayerScore = 0;
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
            _projectile = new List<PizzaProjectile>();


            base.Initialize();
        }
        void setupplayertruck()
        {
            
            player_truck = new PlayerTruck(Content.Load<Texture2D>("player truck"), 400, 800);
            
        }
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            debugFont = Content.Load<SpriteFont>("Arial07");
            for (int i = 0; i < NOOFSC_BACKGROUNDS; i++)
            {
                bgds[i]._txr = Content.Load<Texture2D>("sc_background" + i);
            }

            start_screen = new background(Content.Load<Texture2D>("start_screen"));
            st_button = new buttons(Content.Load<Texture2D>("start_button"), 250, 500, 2, 24, 1);
            htp_button = new buttons(Content.Load<Texture2D>("how to play button"), 250, 600, 2, 24, 1);

            
           
            
            bk_button = new buttons(Content.Load<Texture2D>("back button"),525,900,2,24,1);
            pause_screen = new buttons(Content.Load<Texture2D>("pause_screen"), 0, 0, 2, 24, 1);
            resume_button = new buttons(Content.Load<Texture2D>("resume button"),300,800,2,24,1);
            game_over_screen = new buttons(Content.Load<Texture2D>("game over screen"), 0, 0, 3, 24, 1);
            exit_button = new buttons(Content.Load<Texture2D>("exit button"), 250, 700, 2, 24, 1);
            htp_screen = new background(Content.Load<Texture2D>("how to play screen"));
            player_truck = new PlayerTruck(Content.Load<Texture2D>("player truck"), 400, 800);
            shoot_pizza = Content.Load<SoundEffect>("footstep (high heels)_1");



            htp_Trucks = new List<htp_trucks>();
            htp_Trucks.Add(new htp_trucks(Content.Load<Texture2D>("truck0"), 50, 200));
            htp_Trucks.Add(new htp_trucks(Content.Load<Texture2D>("truck1"), 50, 350));
            htp_Trucks.Add(new htp_trucks(Content.Load<Texture2D>("truck2"), 50, 500));
            htp_Trucks.Add(new htp_trucks(Content.Load<Texture2D>("truck3"), 100, 700));
            htp_Trucks.Add(new htp_trucks(Content.Load<Texture2D>("truck4"), 50, 800));


            for (int i = 0; i < 1; i++)
            {
                Content.Load<Texture2D>("truck" + i);

            }
            for (int i = 0; i < 1; i++)
            {
                Content.Load<Texture2D>("pizza");

            }





        }
        public void ShootPizza()
        {
            // Prevent continuous shooting on single key press
            if (Keyboard.GetState().IsKeyUp(Keys.Space))
                return;

            // Create a new projectile
            _projectile.Add(new PizzaProjectile(Content.Load<Texture2D>("pizza"),player_truck.CollisionRect.Center.X, player_truck.CollisionRect.Top));
            shoot_pizza.Play();

            
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


           



            switch (_currState)
            {
                case GameStates.St_screen:
                    st_screenUpdate(_mouseState);
                    break;
                case GameStates.Gameplayscreen:
                    GameplayscreenUpdate(_mouseState, gameTime);
                    break;
                case GameStates.pause_screen:
                    pause_screenUpdate(_mouseState);
                    break;
                case GameStates.game_overScreen:
                    game_overScreenUpdate(_mouseState);
                    break;
                case GameStates.Htp_screen:
                    htp_ScreenUpdate(_mouseState);
                    break;


            }

            
            player_truck.UpdateMe(padcurr,kb);
            
            
            
            oldkb = kb;
            base.Update(gameTime);
        }

        
        void st_screenUpdate(MouseState ms)
        {
            if(st_button.CollisionRect.Contains(Mouse.GetState().X, Mouse.GetState().Y)&& _mouseState.LeftButton == ButtonState.Pressed)
            {
                _currState = GameStates.Gameplayscreen;
            }
            if(htp_button.CollisionRect.Contains(Mouse.GetState().X, Mouse.GetState().Y)&& _mouseState.LeftButton == ButtonState.Pressed)
            {
                _currState = GameStates.Htp_screen;
            }
            if(exit_button.CollisionRect.Contains(Mouse.GetState().X, Mouse.GetState().Y) && _mouseState.LeftButton == ButtonState.Pressed)
            {
                Exit();
            }
        }

        void htp_ScreenUpdate(MouseState ms)
        {
            if (bk_button.CollisionRect.Contains(Mouse.GetState().X, Mouse.GetState().Y) && _mouseState.LeftButton == ButtonState.Pressed)
            {
                _currState = GameStates.St_screen;

            }
        }
 #region gameplay update
        void GameplayscreenUpdate(MouseState ms, GameTime gameTime)
        {
            for (int i = 0; i < npcTrucks.Count; i++)
            {
                if (npcTrucks[i].Rect.Intersects(player_truck.CollisionRect))
                {
                    if (npcTrucks[i].enemyTruck == true)
                    {
                        PlayerScore -= 1;

                    }
                    else if (npcTrucks[i].motorbike == true)
                    {
                        PlayerScore -= 1;
                    }
                    else
                    {
                        _currState = GameStates.game_overScreen;
                        setupplayertruck();
                        PlayerScore = 0;
                    }
                    npcTrucks.RemoveAt(i);
                    break;
                }
            }
            if (timeTillSpawn < 0)
            {
                
                for (int i = 0; i < trucksPerSpawn; i++)
                {
                    enemyTruck = RNG.Next(0, 5);
                    motorbike = RNG.Next(0, 5);
                    npcTrucks.Add(new pizza_spy.Npc_trucks(Content.Load<Texture2D>("truck" + enemyTruck),
                    (RNG.Next(60, 670)), enemyTruck));
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
                
                if (npcTrucks[i].GetState() == TruckState.Crashed)
                {
                    PlayerScore += 1;
                    npcTrucks.RemoveAt(i);
                    break;
                }
                npcTrucks[i].UpdateMe(_graphics.PreferredBackBufferHeight);
            }


            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                ShootPizza();
            }

            for (int i = 0; i < _projectile.Count; i++)
            {

                if (_projectile[i].GetState() == ProjectileState.Crashed)
                {
                    _projectile.RemoveAt(i);
                    break;
                }
                for (int j = 0; j < npcTrucks.Count; j++)
                {
                    if (_projectile[i].CollisionRect.Intersects(npcTrucks[j].Rect))
                    {
                        if (npcTrucks[j].enemyTruck == true)
                        {
                            PlayerScore += 1;
                        }
                        else if (npcTrucks[j].motorbike == true)
                        {
                            PlayerScore -= 1;
                        }
                        else
                        {
                            _currState = GameStates.game_overScreen;
                            PlayerScore = 0;
                            setupplayertruck();
                        }

                        _projectile.RemoveAt(i);
                        npcTrucks.RemoveAt(j);


                    }
                }
                _projectile[i].UpdateMe(_graphics.PreferredBackBufferHeight);
            }
            if (PlayerScore < 0)
            {
                
                _currState = GameStates.game_overScreen;
                setupplayertruck();
                PlayerScore = 0;


            }
            if (player_truck.CollisionRect.X <= 40 || player_truck.CollisionRect.X >= 610)
            {
                _currState = GameStates.game_overScreen;
                PlayerScore = 0;
                setupplayertruck();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.P))//if space is pressed the game will be paused
            {
                _oldState = _currState;
                _currState = GameStates.pause_screen;

            }
        }
        #endregion

        void pause_screenUpdate(MouseState ms)
        {
            //this is the method for the pause screen with the buttons interactions
            if (bk_button.CollisionRect.Contains(Mouse.GetState().X, Mouse.GetState().Y) && _mouseState.LeftButton == ButtonState.Pressed)
            {

      
                _currState = GameStates.St_screen;

            }
            if (resume_button.CollisionRect.Contains(Mouse.GetState().X, Mouse.GetState().Y) && _mouseState.LeftButton == ButtonState.Pressed)
            {

                _currState = _oldState;
            }

        }
        void game_overScreenUpdate(MouseState ms)
        {
            if (bk_button.CollisionRect.Contains(Mouse.GetState().X, Mouse.GetState().Y) && _mouseState.LeftButton == ButtonState.Pressed)
            {


                _currState = GameStates.St_screen;

            }
        }

        protected override void Draw(GameTime gameTime)
        {
            switch (_currState)
            {
                case GameStates.St_screen:
                    st_screenDraw(gameTime);
                    break;
                case GameStates.Htp_screen:
                    htp_screenDraw(gameTime);
                    break;
                case GameStates.Gameplayscreen:
                    GameplayscreenDraw(gameTime);
                    break;
                case GameStates.pause_screen:
                    PausescreenDraw(gameTime);
                    break;
                case GameStates.game_overScreen:
                    game_overScreenDraw(gameTime);
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
            exit_button.DrawMe(_spriteBatch, gameTime);
            

            _spriteBatch.End();

        }

        void htp_screenDraw(GameTime gameTime)
        {
            _spriteBatch.Begin();

            htp_screen.DrawMe(_spriteBatch);
            for (int i = 0; i < htp_Trucks.Count; i++)
            {
                htp_Trucks[i].DrawMe(_spriteBatch);
            }
            _spriteBatch.DrawString(debugFont, "This is a civilian truck, if you successfully avoid one of them, you will gain 1 point,",
                                             new Vector2(160, 220), Color.White);
            _spriteBatch.DrawString(debugFont, "However, if you hit or collide with it, You Lose.",
                                             new Vector2(160, 270), Color.White);
            _spriteBatch.DrawString(debugFont, "This is a civilian truck, if you successfully avoid one of them, you will gain 1 point,",
                                             new Vector2(160, 370), Color.White);
            _spriteBatch.DrawString(debugFont, "However, if you hit or collide with it, You Lose.",
                                             new Vector2(160, 420), Color.White);
            _spriteBatch.DrawString(debugFont, "This is a civilian truck, if you successfully avoid one of them, you will gain 1 point,",
                                             new Vector2(160, 520), Color.White);
            _spriteBatch.DrawString(debugFont, "However, if you hit or collide with it, You Lose.",
                                             new Vector2(160, 570), Color.White);
            _spriteBatch.DrawString(debugFont, "This is a motorcycle, if you successfully avoid one of them, you will gain 1 point,",
                                             new Vector2(160, 690), Color.White);
            _spriteBatch.DrawString(debugFont, "However, if you hit or collide with it, You will lose 1 point but the game will continue.",
                                             new Vector2(160, 740), Color.White);
            _spriteBatch.DrawString(debugFont, "This is an enemy VAN!!!!, try to hit it with your pizza,",
                                             new Vector2(160, 840), Color.White);
            _spriteBatch.DrawString(debugFont, "if you destroy an enemy van, you will gain 1 point.",
                                             new Vector2(160, 890), Color.White);
            _spriteBatch.DrawString(debugFont, "if you hit the edge of the road, YOU LOSE!!!",
                                             new Vector2(140, 950), Color.White);
            _spriteBatch.DrawString(debugFont, "Oh, and don't forget, press 'P' to pause.",
                                             new Vector2(140, 970), Color.White);
            bk_button.DrawMe(_spriteBatch, gameTime);


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
            for (int i = 0;i < _projectile.Count; i++) 
            {
                _projectile[i].DrawMe(_spriteBatch);
            }
            _spriteBatch.DrawString(debugFont, "Res: " + _graphics.PreferredBackBufferWidth
                                              + " x " + _graphics.PreferredBackBufferHeight,
                                              Vector2.Zero, Color.White);
            _spriteBatch.DrawString(debugFont, "PlayerTruck: " + player_truck.CollisionRect, new Vector2(100, 12), Color.White);
            _spriteBatch.DrawString(debugFont, "Number of trucks: " + npcTrucks.Count, new Vector2(100, 36), Color.White);
            _spriteBatch.DrawString(debugFont, "Trucks per spawn: " + trucksPerSpawn + " Spawn in: " + timeTillSpawn, new Vector2(100, 48), Color.White);
            _spriteBatch.DrawString(debugFont, "Current Speed " + player_truck.speed, new Vector2(100, 60), Color.White);
            _spriteBatch.DrawString(debugFont, "Player Score: " + PlayerScore, new Vector2(500, 1000), Color.White);
            player_truck.DrawMe(_spriteBatch);
            _spriteBatch.End();
        }
        void PausescreenDraw(GameTime gameTime)
        {
            _spriteBatch.Begin();
            pause_screen.DrawMe(_spriteBatch, gameTime);
            bk_button.DrawMe(_spriteBatch, gameTime);
            resume_button.DrawMe(_spriteBatch, gameTime);

            _spriteBatch.End();
        }
        void game_overScreenDraw(GameTime gameTime)
        {
            _spriteBatch.Begin();
            game_over_screen.DrawMe(_spriteBatch, gameTime);
            bk_button.DrawMe(_spriteBatch,gameTime);
            _spriteBatch.DrawString(debugFont, "Your Final Score is: " + PlayerScore, new Vector2(100, 60), Color.Black);
            
            _spriteBatch.End();
        }
    }
}
