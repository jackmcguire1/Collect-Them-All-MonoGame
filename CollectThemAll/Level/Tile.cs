using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace assignment_4
{
    /// <summary>
    /// The tile class is used to represent any of the blocks that
    /// Mario encounters in this simple game.  In this example
    /// the blocks are simple and have no interactive features.
    /// Naturally, this could be extended in future implementations.
    /// 
    /// NOTE: An argument can be raised at this juncture as to how these
    /// tiles should be implemented.  As we have seen throughout 
    /// the Programming 1 and Programming 2 modules, classes provide
    /// a nice way to encapsulate information that is useful to us.
    /// In saying that, a class is typically used for complex 
    /// management of information where we expect information within
    /// to be changed or processed.  These tiles will not suffer drastic
    /// change throughout the game.  In fact a simple tile will not
    /// change at all.  As such, it may be prudent to store tiles
    /// in a struct rather than a class.  However, we provide the class
    /// implementation since 1) classes tie more to our taught material
    /// and 2) if we were to create interactive tiles then the class
    /// declaration would be more extensible.
    /// 
    /// For more information, check out the MSDN articles on classes and structs:
    /// http://msdn.microsoft.com/en-us/library/ms173109.aspx
    /// 
    /// Tommy Thompson 04/01/13
    /// </summary>
    public class Tile
    {
        //Static to help with level and tile construction.
        public static int   TILE_WIDTH = 10,
                            TILE_HEIGHT = 10;
        
        //The texture sheet used for all of the tiles.
        static Texture2D texture;

        //Position of the tile.
        Vector2 position;
        
        //Necessary should the texture be within a sprite sheet.
        Vector2 textureOrigin; 

        public Tile(Vector2 position)
        {
            this.position = position;
            textureOrigin = Vector2.Zero;
        }

        public Tile(Vector2 textureOrigin, Vector2 positionIn)
            : this(positionIn)
        {
            this.textureOrigin = textureOrigin;
        }

        public static Texture2D Texture
        {
            set
            {
                texture = value;
            }
        }

        public Vector2 Position
        {
            get
            {
                return position;
            }
        }

        public Vector2 Size
        {
            get
            {
                return new Vector2(TILE_WIDTH, TILE_HEIGHT);
            }
        }

        public Rectangle BoundingBox
        {
            get
            {
                //Note we cast the position to int since the X and Y values are floats.
                return new Rectangle((int)position.X, (int)position.Y, TILE_WIDTH, TILE_HEIGHT);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Game1.COLLISION_DEBUG)
            {
                Texture2D simpleTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                simpleTexture.SetData(new Color[] { Color.Turquoise });
                spriteBatch.Draw(simpleTexture, BoundingBox, Color.Turquoise);
            }
            else
            {
                spriteBatch.Draw(texture,
                                    position,
                                    new Rectangle((int)textureOrigin.X, (int)textureOrigin.Y, TILE_WIDTH, TILE_HEIGHT),
                                    Color.White,
                                    0,
                                    Vector2.Zero,
                                    1,
                                    SpriteEffects.None,
                                    0);
            }
        }

    }
}
