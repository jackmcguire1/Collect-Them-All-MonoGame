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
    public class Item2 : items
    {

        static Rectangle frame;
        Point frameSize = new Point(40, 34);
        static Texture2D texture;
        Vector2 position;



        //Some variables for gravity and texture scaling.
        float yVelocity;
        float scale;
        Level currentLevel;

        public Item2(Level currentLevel, Vector2 position)
        {
            this.position = position;
            this.currentLevel = currentLevel;
            initialise();
        }

        public static Texture2D Texture
        {
            set
            {
                texture = value;
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (Game1.COLLISION_DEBUG)
            {
                Texture2D simpleTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                simpleTexture.SetData(new Color[] { Color.LightPink });
                spriteBatch.Draw(simpleTexture, BoundingBox(), Color.LightPink);
            }
            else
            {

                spriteBatch.Draw(texture,
                                position,
                                new Rectangle(176, 65, 28, 28),
                                Color.White,
                                0,
                                Vector2.Zero,
                                scale,
                                SpriteEffects.None,
                                0);
            }
        }

        public Rectangle BoundingBox()
        {
            return new Rectangle((int)position.X, (int)position.Y, 28, 28);
            throw new NotImplementedException();
        }

        public Vector2 getPosition()
        {
            return position;
            throw new NotImplementedException();
        }

        public void initialise()
        {
            yVelocity = 4.0f;
            scale = 1.25f;
        }

        public void Update(GameTime gameTime)
        {
            Vector2 updatedPosition = position;

            position = new Vector2((int)updatedPosition.X, (int)updatedPosition.Y);

            //applies gravity to the items
            Tuple<Vector2, float> gravityResult = Physics.ApplyGravityToVector2(updatedPosition, yVelocity, gameTime);
            updatedPosition = gravityResult.Item1;

            position = new Vector2((int)updatedPosition.X, (int)updatedPosition.Y);
        }

        public void LoadContent(ContentManager content)
        {
            frame = new Rectangle(30, 30, 30, 30);
        }
    }
}
