using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace assignment_4
{
    /// <summary>
    /// The level class contains items - tiles, pick-ups and characters.
    /// </summary>
    public class Level
    {
        //Vector2 variables for starting positions
        Vector2 mariostartPosition,peachstartPosition, goalPosition;

        //declarations for characters.
        Mario mario;
        Peach peach;

        //A collection of tiles and items, characters can collide with.
        List<Tile> tiles;
        List<items> items = new List<items>();

        //Texture2D for the terrain tiles.  
        Texture2D terrainTiles;
        Texture2D terrainMushroom;
        Texture2D terrainStar;
        Texture2D terrainFlower;
        Texture2D terrainMysteryBox;

        //Current Score
        int Player1score;
        int Player2score;

        //Music for the level.
        Song levelSong;

        //The InputHandler that is being used.
        InputHandler inputHandler;

        //The Fonts used to draw text to the screen.
        SpriteFont spriteFont;
        SpriteFont spriteFont2;
        SpriteFont spriteFont3;

        //Boolean for the gameover state.
        bool gameOver;

        public Level(Vector2 startPosition, Vector2 goalPosition, InputHandler inputHandler)
        {
            this.mariostartPosition = startPosition;
            this.peachstartPosition = goalPosition;
            this.goalPosition = goalPosition;
            this.inputHandler = inputHandler;
        }

        //If a previous game has ended, values are set for a new one to begin.
        public void NewLevel()
        {
            gameOver = false;
            MediaPlayer.Play(levelSong);
            mario.KillMario();
            peach.KillMario();
            Player1score = 0;
            Player2score = 0;
            items.Add(new Item2(this, new Vector2(150, 30)));
            items.Add(new Item2(this, new Vector2(250, 30)));
            items.Add(new Item2(this, new Vector2(500, 30)));
        }

        /// <summary>
        /// Initialises the Level, 
        /// sets items and tiles.
        /// </summary>
        public void Initialise()
        {
            //Create an instance of both characters, for players to use.
            mario = new Mario(this, new Vector2(500, 155), inputHandler);
            peach = new Peach(this, mariostartPosition, inputHandler);
            
           
            //Create a collection of tiles.
            tiles = new List<Tile>();
            //creates a list for different types of items to be inputted.
            items = new List<items>();
            items.Add(new Item2(this,new Vector2(150,0)));
            items.Add(new Item2(this, new Vector2(250,0)));
            items.Add(new Item2(this,new Vector2(500,0)));
            

            //Set scores and state of the game
            Player1score = 0;
            Player2score = 0;
            gameOver = false;

            //Tiles displayed on y-Axis prevent players from leaving level.
            for (int y = 100; y < 430; y += Tile.TILE_HEIGHT)
            {
                tiles.Add(new Tile(new Vector2(72, 77), new Vector2(0, y)));
            }
            for (int y = 100; y < 430; y += Tile.TILE_HEIGHT)
            {
                tiles.Add(new Tile(new Vector2(72, 77), new Vector2(790, y)));
            }
            

            //tiles on the x-axis.
            for (int x = -100; x < 900; x += Tile.TILE_WIDTH)
            {
                tiles.Add(new Tile(new Vector2(72, 77), new Vector2(x, 430)));
            }
            for (int x = -100; x < 900; x += Tile.TILE_WIDTH)
            {
                tiles.Add(new Tile(new Vector2(72, 77), new Vector2(x, 440)));
            }
        }




        /// <summary>
        /// Loads in the content for the level.
        /// textures, fonts, music.
        /// </summary>
        /// <param name="content">The games content manager</param>
        public void LoadContent(ContentManager content)
        {
            //Font files.
            spriteFont = content.Load<SpriteFont>("Fonts/SpriteFont6");
            spriteFont2 = content.Load<SpriteFont>("Fonts/SpriteFont5");
            spriteFont3 = content.Load<SpriteFont>("Fonts/SpriteFont4");

            //assigns texture 2D variables to the correct texture files.
            terrainTiles = content.Load<Texture2D>("Images/Blocks");
            terrainMushroom = content.Load<Texture2D>("Images/Backgrounds-Objects");
            terrainStar = content.Load<Texture2D>("Images/Backgrounds-Objects");
            terrainFlower = content.Load<Texture2D>("Images/Backgrounds-Objects");
            terrainMysteryBox = content.Load<Texture2D>("Images/Backgrounds-Objects");
            
            //Sets each classes texture values.
            Tile.Texture = terrainTiles;
            Item1.Texture = terrainMushroom;
            Item2.Texture = terrainStar;
            Item3.Texture = terrainFlower;
            Item4.Texture = terrainMysteryBox;

            //Loads sprite content for both characters.
            mario.LoadContent(content);
            peach.LoadContent(content);
            
            //Load in the music.
            levelSong = content.Load<Song>("Sound/Game");
        }


        /// <summary>
        /// Access the levels song.
        /// </summary>
        public Song Song
        {
            get
            {
                return levelSong;
            }
        }

        /// <summary>
        /// Accessor for Mario
        /// </summary>
        public Mario Mario
        {
            get
            {
                return mario;
            }
        }

        public Peach Peach
        {
            get
            {
                return peach;
            }
        }

        /// <summary>
        /// Accessor for the score.
        /// </summary>
        public int Player1Score
        {
            get
            {
                return Player1score;
            }
        }

        public int Player2Score
        {
            get
            {
                return Player2score;
            }
        }

        /// <summary>
        /// An accessor for the list of tiles in the game.
        /// </summary>
        public List<Tile> Tiles
        {
            get
            {
                return tiles;
            }
        }
        
        //Property that dictates whether game is over.
        public bool GameOver
        {
            get
            {
                return gameOver;
            }
        }

        /// <summary>
        /// Updates character & item positioning.
        /// this function contains fixes for bugs and is used for collision detection between 
        /// players and items.
        /// </summary>
        /// <param name="gameTime">Current game time</param>
        public void Update(GameTime gameTime)
        {

            //Player Positions
            mario.Update(gameTime);
            peach.Update(gameTime);

            //error checking if player scores go below 0 due to deductions.
            if (Player1score < 0)
            {
                Player1score = 0;
            }

            if (Player2score < 0)
            {
                Player2score = 0;
            }

            //updates item positions.
            foreach (items currentball in items)
            {
                currentball.Update(gameTime);
            }

            foreach (items currentball in items)
            {
                //if player 1 or player 2 hits either of the four items, it either de-ducts or adds points on to their score, then adds new items into the game.
                #region player1
                if (currentball.BoundingBox().Intersects(peach.BoundingBox))
                {
                    #region Item1 collision + score
                    if (currentball.GetType().ToString() == "assignment_4.Item1")
                    {
                        items.Remove(currentball);
                        Player1score += 1;
                        Random newposition = new Random();
                        int x = newposition.Next(0, 4);
                        int y = 10;
                        if (x == 0)
                        {
                            x = newposition.Next(10, 700);
                            items.Add(new Item1(this, new Vector2(x, y)));
                        }
                        if (x == 1)
                        {
                            x = newposition.Next(10, 700);
                            items.Add(new Item2(this, new Vector2(x, y)));
                        }
                        if (x == 2)
                        {
                            x = newposition.Next(10, 700);
                            items.Add(new Item3(this, new Vector2(x, y)));
                        }
                        if (x == 3)
                        {
                            x = newposition.Next(10, 700);
                            items.Add(new Item2(this, new Vector2(x, y)));
                        }
                        break;
                    }
                    #endregion

                    #region Item2 collision - score
                    if (currentball.GetType().ToString() == "assignment_4.Item2")
                    {
                        items.Remove(currentball);
                        Player1score -= 10;
                        Random newposition = new Random();
                        int x = newposition.Next(0, 4);
                        int y = 10;
                        if (x == 0)
                        {
                            x = newposition.Next(10, 700);
                            items.Add(new Item1(this, new Vector2(x, y)));
                        }
                        if (x == 1)
                        {
                            x = newposition.Next(10, 700);
                            items.Add(new Item2(this, new Vector2(x, y)));
                        }
                        if (x == 2)
                        {
                            x = newposition.Next(10, 700);
                            items.Add(new Item3(this, new Vector2(x, y)));
                        }
                        if (x == 3)
                        {
                            x = newposition.Next(10, 700);
                            items.Add(new Item2(this, new Vector2(x, y)));
                        }
                        break;
                    }
                    #endregion

                    #region Item3 collision + 3 to score
                    if (currentball.GetType().ToString() == "assignment_4.Item3")
                    {
                        items.Remove(currentball);
                        Player1score += 3;
                        Random newposition = new Random();
                        int x = newposition.Next(0, 2);
                        int y = 10;
                        if (x == 1)
                        {
                            x = newposition.Next(10, 700);
                            items.Add(new Item1(this, new Vector2(x, y)));
                        }
                        else
                        {
                            x = newposition.Next(10, 700);
                            items.Add(new Item2(this, new Vector2(x, y)));
                        }
                        break;
                    }
                    #endregion

                    #region Item4 - MysteryBox
                    if (currentball.GetType().ToString() == "assignment_4.Item4")
                    {
                        items.Remove(currentball);
                        Player1score += 3;
                        Random newposition = new Random();
                        int x = newposition.Next(0, 3);
                        if (x == 0)
                        {
                            mario.setSpeed(8);
                        }
                        if (x == 1)
                        {
                             x = newposition.Next(0, 10);
                             Player1score = Player1score + x; 
                        }
                        if (x == 2)
                        {
                            x = newposition.Next(0, 10);
                            Player1score = Player1score - x;
                        }

                        x = newposition.Next(0, 4);
                        int y = 10;
                        if (x == 0)
                        {
                            x = newposition.Next(10, 700);
                            items.Add(new Item1(this, new Vector2(x, y)));
                        }
                        if (x == 1)
                        {
                            x = newposition.Next(10, 700);
                            items.Add(new Item2(this, new Vector2(x, y)));
                        }
                        if (x == 2)
                        {
                            x = newposition.Next(10, 700);
                            items.Add(new Item3(this, new Vector2(x, y)));
                        }
                        if (x == 3)
                        {
                            x = newposition.Next(10, 700);
                            items.Add(new Item2(this, new Vector2(x, y)));
                        }
                        break;
                    }
                    #endregion
                }
                #endregion

                #region player2
                if (currentball.BoundingBox().Intersects(mario.BoundingBox))
                {
                    #region Item1 collision + 10 to score
                    if (currentball.GetType().ToString() == "assignment_4.Item1")
                    {
                        items.Remove(currentball);
                        Player2score += 1;
                        Random newposition = new Random();
                        int x = newposition.Next(0, 4);
                        int y = 10;
                        if (x == 0)
                        {
                            x = newposition.Next(10, 700);
                            items.Add(new Item1(this, new Vector2(x, y)));
                        }
                        if (x == 1)
                        {
                            x = newposition.Next(10, 700);
                            items.Add(new Item2(this, new Vector2(x, y)));
                        }
                        if (x == 2)
                        {
                            x = newposition.Next(10, 700);
                            items.Add(new Item3(this, new Vector2(x, y)));
                        }
                        if (x == 3)
                        {
                            x = newposition.Next(10, 700);
                            items.Add(new Item2(this, new Vector2(x, y)));
                        }
                        break;
                    }
                    #endregion

                    #region Item2 collision - 10 from score
                    if (currentball.GetType().ToString() == "assignment_4.Item2")
                    {
                        items.Remove(currentball);
                        Player2score -= 10;
                        Random newposition = new Random();
                        int x = newposition.Next(0, 4);
                        int y = 10;
                        if (x == 0)
                        {
                            x = newposition.Next(10, 700);
                            items.Add(new Item1(this, new Vector2(x, y)));
                        }
                        if (x == 1)
                        {
                            x = newposition.Next(10, 700);
                            items.Add(new Item2(this, new Vector2(x, y)));
                        }
                        if (x == 2)
                        {
                            x = newposition.Next(10, 700);
                            items.Add(new Item3(this, new Vector2(x, y)));
                        }
                        if (x == 3)
                        {
                            x = newposition.Next(10, 700);
                            items.Add(new Item2(this, new Vector2(x, y)));
                        }
                        break;
                    }
                    #endregion

                    #region Item3 Collision + 3 to score
                    if (currentball.GetType().ToString() == "assignment_4.Item3")
                    {
                        items.Remove(currentball);
                        Player2score += 3;
                        Random newposition = new Random();
                        int x = newposition.Next(0, 2);
                        int y = 10;
                        if (x == 1)
                        {
                            x = newposition.Next(10, 700);
                            items.Add(new Item1(this, new Vector2(x, y)));
                        }
                        else
                        {
                            x = newposition.Next(10, 700);
                            items.Add(new Item2(this, new Vector2(x, y)));
                        }
                        break;
                    }
                    #endregion

                    #region Item4 - MysteryBox
                    if (currentball.GetType().ToString() == "assignment_4.Item4")
                    {
                        items.Remove(currentball);
                        Player1score += 3;
                        Random newposition = new Random();
                        int x = newposition.Next(0, 3);
                        if (x == 0)
                        {
                            mario.setSpeed(8);
                        }
                        if (x == 1)
                        {
                            x = newposition.Next(0, 10);
                            Player2score = Player2score + x;
                        }
                        if (x == 2)
                        {
                            x = newposition.Next(0, 10);
                            Player2score = Player2score - x;
                        }

                        x = newposition.Next(0, 4);
                        int y = 10;
                        if (x == 0)
                        {
                            x = newposition.Next(10, 700);
                            items.Add(new Item1(this, new Vector2(x, y)));
                        }
                        if (x == 1)
                        {
                            x = newposition.Next(10, 700);
                            items.Add(new Item2(this, new Vector2(x, y)));
                        }
                        if (x == 2)
                        {
                            x = newposition.Next(10, 700);
                            items.Add(new Item3(this, new Vector2(x, y)));
                        }
                        if (x == 3)
                        {
                            x = newposition.Next(10, 700);
                            items.Add(new Item2(this, new Vector2(x, y)));
                        }
                        break;
                    }
                    #endregion
                }
                #endregion

                #region if square goes off level, delete & add more items.
                if (currentball.getPosition().Y > 500)
                 {
                        items.Remove(currentball);
                        Random newposition = new Random();
                        int x = newposition.Next(0, 10);
                        int y = 10;
                        if (x == 0)
                        {
                            x = newposition.Next(10, 700);
                            items.Add(new Item1(this, new Vector2(x, y)));
                        }
                        if(x == 1 || x == 3)
                        {
                            x = newposition.Next(10, 700);
                            items.Add(new Item2(this, new Vector2(x, y)));
                        }
                        if (x == 2 || x == 6)
                        {
                                x = newposition.Next(10, 700);
                                items.Add(new Item3(this, new Vector2(x, y)));
                        }
                        if (x == 5 || x== 9)
                        {
                            x = newposition.Next(10, 700);
                            items.Add(new Item4(this, new Vector2(x, y)));
                        }
                        if (x == 8 || x == 7 || x == 4)
                        {
                            x = newposition.Next(10, 700);
                            items.Add(new Item2(this, new Vector2(x, y)));
                        }
                        break;
                 }
                #endregion
            }
            
            //if the weird glitch happens or either player escapes from the map, re-position both players.
            if (mario.Position.Y > 700 || peach.Position.Y > 700)
            {
                mario.KillMario();
                peach.KillMario();
            }

            if (Player1score >= 40)
            {
                gameOver = true;
                MediaPlayer.Stop();
            }

            if (Player2score >= 40)
            {
                gameOver = true;
                MediaPlayer.Stop();
            }
        }

        /// <summary>
        /// Draws the level contents to the Graphics Device.
        /// </summary>
        /// <param name="gameTime">Current game time.</param>
        /// <param name="spriteBatch">Sprite batch for drawing.</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //Draw characters
            mario.DrawMario(gameTime, spriteBatch);
            peach.DrawMario(gameTime, spriteBatch);

            //draws scoring.
            spriteBatch.DrawString(spriteFont3, "Luigi's Score: " + Player1score, new Vector2(10, 10), Color.White);
            spriteBatch.DrawString(spriteFont3, "Mario's Score: " + Player2score, new Vector2(500,10), Color.White);


            //draw tiles function.
            DrawTiles(spriteBatch);
            
            //draws items.
            foreach (items currentball in items)
            {
                currentball.Draw(gameTime, spriteBatch);
            }
        }

        /// <summary>
        /// function draws tiles specified in the initialise function.
        /// </summary>
        /// <param name="spriteBatch"></param>
        private void DrawTiles(SpriteBatch spriteBatch)
        {
            foreach (Tile currentTile in tiles)
            {
                currentTile.Draw(spriteBatch);
            }
        }


    }
}
