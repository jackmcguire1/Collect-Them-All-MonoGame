using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace assignment_4
{
    /// <summary>
    /// This is the main class which declares level intialisation and GUI
    /// configuration.
    /// 
    /// </summary> 
    public enum GameState
    {
        StartScreen, Running, GameOverPlayer1, GameOverPlayer2, Instructions, instructions2
    }

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        public static bool COLLISION_DEBUG = false;
        //Same as before, graphics device and sprite batch as needed.
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //SpriteFont needed to draw text to the screen.
        SpriteFont spriteFont, largeSpriteFont, MediumSprite;

        //Level that Mario is playing in right now.
        Level currentLevel;

        //Current game state.
        GameState gameState;

        Texture2D texture;

        //Input management.
        InputHandler inputHandler;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //We set the input handler here, allowing us to access the information both here and in the Mario class.
            inputHandler = new InputHandler(PlayerIndex.One);


            //Set initial state and score.
            gameState = GameState.StartScreen;

            //Initialise the music to loop.
            MediaPlayer.IsRepeating = true;

            currentLevel = new Level(new Vector2(200, 200), new Vector2(250, 5), new InputHandler(PlayerIndex.One));
            currentLevel.Initialise();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            
            spriteBatch = new SpriteBatch(GraphicsDevice);

            texture = Content.Load<Texture2D>("Images/Backgrounds-Objects");
            //First, load in the sprite font like we did before...
            spriteFont = Content.Load<SpriteFont>("Fonts/SpriteFont6");
            largeSpriteFont = Content.Load<SpriteFont>("Fonts/SpriteFont5");
            MediumSprite = Content.Load<SpriteFont>("Fonts/SpriteFont4");


            //Load the level itself!
            currentLevel.LoadContent(Content);
           
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            Content.Unload();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the user to navigate around the menus.
            if (inputHandler.IsKeyTapped(Keys.Escape))
            {
                if (gameState == GameState.Instructions || gameState == GameState.GameOverPlayer1 || gameState == GameState.GameOverPlayer2 || gameState == GameState.instructions2)
                {
                    gameState = GameState.StartScreen;
                }
                else
                {
                    this.Exit();
                }
            }

            inputHandler.UpdateInput();

            if (inputHandler.IsKeyTapped(Keys.D1))
            {
                if (gameState == GameState.StartScreen)
                {
                    if (MediaPlayer.State != MediaState.Playing)
                    {
                        MediaPlayer.Play(currentLevel.Song);
                    }
                    if (currentLevel.GameOver)
                    {
                        currentLevel.NewLevel();
                        gameState = GameState.Running;
                    }
                    else
                    {
                        gameState = GameState.Running;
                    }

                }
            }

            if (inputHandler.IsKeyTapped(Keys.D2))
            {
                if (gameState == GameState.StartScreen)
                {
                    gameState = GameState.Instructions;
                }
            }

            if (inputHandler.IsKeyTapped(Keys.D3))
            {
                if (gameState == GameState.Instructions)
                {
                    gameState = GameState.instructions2;
                }
            }

            //while the Gamestate is running, update the materials within the level, check if gameover is true.
            if (gameState == GameState.Running)
            {
                currentLevel.Update(gameTime);
                if (currentLevel.GameOver)
                {
                    MediaPlayer.Stop();
                    GameOver(gameTime);
                }
            }
            base.Update(gameTime);
        }

        private void GameOver(GameTime gameTime)
        {
            int player1 = currentLevel.Player1Score;
            int player2 = currentLevel.Player2Score;

            if (player1 > player2)
            {
                gameState = GameState.GameOverPlayer1;
            }
            else
            {
                gameState = GameState.GameOverPlayer2;
            }
        }
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Tomato);
            spriteBatch.Begin();
            if (gameState == GameState.StartScreen)
            {
                //draw graphical fonts to screen.
                GraphicsDevice.Clear(Color.Purple);
                spriteBatch.DrawString(largeSpriteFont, "Collect'em All", new Vector2(180, 10), Color.White);
                spriteBatch.DrawString(spriteFont, "1.PLAY", new Vector2(170, 200), Color.White);
                spriteBatch.DrawString(spriteFont, "2.Controls", new Vector2(460, 200), Color.White);
                spriteBatch.DrawString(MediumSprite, "Jack McGuire - 100249558", new Vector2(1, 400), Color.White);

                spriteBatch.DrawString(MediumSprite, "Music by Robert Dunn", new Vector2(500, 400), Color.White);
                spriteBatch.DrawString(MediumSprite, "Programming 2 - 4CC511", new Vector2(500, 425), Color.White);
            }

            if (gameState == GameState.Instructions)
            {
                GraphicsDevice.Clear(Color.Red);

                spriteBatch.DrawString(largeSpriteFont, "Controls", new Vector2(280, 10), Color.White);
                spriteBatch.DrawString(MediumSprite, "Esc to go back!", new Vector2(70, 400), Color.White);
                spriteBatch.DrawString(MediumSprite, "3.How To Play", new Vector2(600, 400), Color.White);
                spriteBatch.DrawString(MediumSprite, "Note: All controls are found on the Keyboard!", new Vector2(150, 70), Color.White);

                spriteBatch.DrawString(spriteFont, "PLAYER 1", new Vector2(80, 150), Color.White);
                spriteBatch.DrawString(MediumSprite,"Jump - 'W'", new Vector2(100,210),Color.White);
                spriteBatch.DrawString(MediumSprite, "Strafe Left - 'A'", new Vector2(80, 250), Color.White);
                spriteBatch.DrawString(MediumSprite, "Strafe Right - 'D'", new Vector2(80, 290), Color.White);

                spriteBatch.DrawString(spriteFont, "PLAYER 2", new Vector2(470, 150), Color.White);
                spriteBatch.DrawString(MediumSprite, "Jump - 'Up Arrow key'", new Vector2(450, 210), Color.White);
                spriteBatch.DrawString(MediumSprite, "Strafe Left - 'Left Arrow key'", new Vector2(420, 250), Color.White);
                spriteBatch.DrawString(MediumSprite, "Strafe Right - 'Right Arrow Key'", new Vector2(420, 290), Color.White);
            }

            if (gameState == GameState.instructions2)
            {
                GraphicsDevice.Clear(Color.Red);
                spriteBatch.DrawString(largeSpriteFont, "How to Play", new Vector2(240, 10), Color.White);
                spriteBatch.DrawString(MediumSprite, "The objective of the game is to collect the items", new Vector2(100, 70), Color.White);
                spriteBatch.DrawString(MediumSprite, "some items give points, some items deduct points", new Vector2(100, 90), Color.White);
                spriteBatch.DrawString(MediumSprite, "the first player to reach 40 points wins!", new Vector2(100, 110), Color.White);

                //Mushroom
                Rectangle item = new Rectangle(155, 65, 28, 28);
                Texture2D simpleTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                simpleTexture.SetData(new Color[] { Color.DarkGreen });
                spriteBatch.Draw(texture,new Vector2(300, 170), item, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                spriteBatch.DrawString(MediumSprite, "= + 1 POINT", new Vector2(350, 170), Color.White);

                //Star
                Rectangle item1 = new Rectangle(134, 64, 28, 28);
                simpleTexture.SetData(new Color[] { Color.DarkGreen });
                spriteBatch.Draw(texture, new Vector2(300, 250), item1, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                spriteBatch.DrawString(MediumSprite, "= + 3 POINTS", new Vector2(350, 250), Color.White);

                //Flower
                Rectangle item2 = new Rectangle(176, 65, 28, 28);
                simpleTexture.SetData(new Color[] { Color.DarkGreen });
                spriteBatch.Draw(texture, new Vector2(300, 320), item2, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                spriteBatch.DrawString(MediumSprite, "= - 10 POINTS", new Vector2(350, 320), Color.White);

                //MysteryBox
                Rectangle item3 = new Rectangle(155, 135, 28, 28);
                spriteBatch.Draw(texture, new Vector2(300, 390), item3, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                spriteBatch.DrawString(MediumSprite, "= Random surprise!", new Vector2(350, 390), Color.White);

                spriteBatch.DrawString(MediumSprite, "Esc to return to start screen!", new Vector2(30, 430), Color.White);
            }

            if (gameState == GameState.Running)
            {
                GraphicsDevice.Clear(Color.CornflowerBlue);
                currentLevel.Draw(gameTime, spriteBatch);  
            }

            if (gameState == GameState.GameOverPlayer1)
            {
                GraphicsDevice.Clear(Color.DarkGreen);
                spriteBatch.DrawString(largeSpriteFont, "Game Over!", new Vector2(240, 10), Color.White);
                spriteBatch.DrawString(MediumSprite, "Player 1 WINS!!", new Vector2(300, 70), Color.White);

                spriteBatch.DrawString(spriteFont, "PLAYER 1", new Vector2(100, 150), Color.White);
                spriteBatch.DrawString(MediumSprite, currentLevel.Player1Score.ToString(), new Vector2(170, 210), Color.White);
                spriteBatch.DrawString(spriteFont, "PLAYER 2", new Vector2(470, 150), Color.White);
                spriteBatch.DrawString(MediumSprite, currentLevel.Player2Score.ToString(), new Vector2(550, 210), Color.White);
            }

            if (gameState == GameState.GameOverPlayer2)
            {
                GraphicsDevice.Clear(Color.Orange);
                spriteBatch.DrawString(largeSpriteFont, "Game Over!", new Vector2(240, 10), Color.White);
                spriteBatch.DrawString(MediumSprite, "Player 2 WINS!!", new Vector2(300, 70), Color.White);

                spriteBatch.DrawString(spriteFont, "PLAYER 1", new Vector2(100, 150), Color.White);
                spriteBatch.DrawString(MediumSprite, currentLevel.Player1Score.ToString(), new Vector2(170, 210), Color.White);
                spriteBatch.DrawString(spriteFont, "PLAYER 2", new Vector2(470, 150), Color.White);
                spriteBatch.DrawString(MediumSprite, currentLevel.Player2Score.ToString(), new Vector2(550, 210), Color.White);
            }

            
            // TODO: Add your drawing code here
            spriteBatch.End();
            base.Draw(gameTime);
        }

        
    }
}
