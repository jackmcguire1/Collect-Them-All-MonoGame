using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace assignment_4
{
    /// <summary>
    /// A small utlity class to 
    /// </summary>
    public class Physics
    {
        const float FallMax = 1000.0f;
        const float YAcceleration = 20.0f; //Gravitational acceleration
        
        /// <summary>
        /// Physics code, that will apply gravity to a given vector based on 
        /// current velocity on the Y-axis.
        /// </summary>
        /// <param name="position">Position of object</param>
        /// <param name="yVelocity">Velocity of object on y-axis</param>
        /// <param name="gameTime">Current game time.</param>
        /// <returns></returns>
        public static Tuple<Vector2, float> ApplyGravityToVector2(Vector2 position, float yVelocity, GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector2 updatedPosition = position;
            
            // Base velocity is a combination of horizontal movement control and
            // acceleration downward due to gravity.
            float newVelocity = MathHelper.Clamp(yVelocity + YAcceleration * (elapsed/2), -FallMax, FallMax);
            updatedPosition.Y = updatedPosition.Y + newVelocity;
            newVelocity = MathHelper.Clamp(newVelocity + YAcceleration * (elapsed/2), -FallMax, FallMax);
            
            return new Tuple<Vector2,float>(updatedPosition, newVelocity);
        }
    }
}
