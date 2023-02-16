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
    public interface items
    {
        //Interface of items, designed to hold a collection of items to test collision for each item.
       void Draw(GameTime gameTime, SpriteBatch spriteBatch);
       Rectangle BoundingBox();
       Vector2 getPosition();
       void initialise();
       void Update(GameTime gameTime);
       void LoadContent(ContentManager content);


    }
}
