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
    public class Item4 : items
    {

        static Rectangle frame;
        Point frameSize = new Point(40, 34);
        static Texture2D texture;
        //Door position.
        Vector2 position;

        //Moving speed used for both keyboard and gamepad.
        const float moveSpeed = 3.0f;

        //Some variables for the jump code.
        const float InitJumpVelocity = 0.0f;
        float yVelocity;
        Level currentLevel;

        public Item4(Level currentLevel, Vector2 position)
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
                simpleTexture.SetData(new Color[] { Color.Sienna });
                spriteBatch.Draw(simpleTexture, BoundingBox(), Color.Sienna);
            }
            else
            {

                spriteBatch.Draw(texture,
                                position,
                                new Rectangle(155, 135, 28, 28),
                                Color.White,
                                0,
                                Vector2.Zero,
                                1,
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
        }

        public void Update(GameTime gameTime)
        {
            //We store the new position in a temporary variable, in case we need to edit it.
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
