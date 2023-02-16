using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Threading;
using Microsoft.Xna.Framework.Audio;

namespace Jumping
{
    public enum MarioDirection
    {
        Left, Right, Up, Down
    }

    public enum MarioAction
    {
        Stand, Walk, Run, JumpFall, Crouch
    }

    /// <summary>
    /// The latest version of the Mario class now contains references to the sound effects
    /// required.
    /// 
    /// Tommy Thompson 03/01/13
    /// </summary>
    public class Mario
    {
        //Number of lives.
        int numberOfLives;

        //A 2D vector that is used to represent the (x,y) position of Mario.
        Vector2 position, startingPosition;
        MarioAction currentAction;
        MarioDirection currentDirection;

        //Moving speed used for both keyboard and gamepad.
        const float moveSpeed = 4.0f;

        //Some variables for the jump code.
        const float InitJumpVelocity = -10.0f;
        float yVelocity;
        bool isJumping = false;


        //Code to handle the Input for Mario
        InputHandler inputHandler;

        /* These two variables are used to track the changes to the currently visible sprite.
         * Increase or decrease the const millisecondsPerFrame in order to speed up or slow
         * down the changing of the visible sprite.
         */
        int timeSinceLastFrame;
        const int millisecondsPerFrame = 50;
        const float textureScale = 3.0f;
       
        /*THis dictionary is designed to keep separate sets for all of the different
         * movement behaviours of Mario.  Based on what direction he is moving and the 
         * action taking place, we have a set of animation frames that we can draw.
         * This is achieved by matching a tuple of (direction, action) to an animation set.
         */
        Dictionary<Tuple<MarioDirection, MarioAction>, AnimationFrames> animationSets;

        //A texture variable that stores the full Mario sprite sheet.
        Texture2D spriteSheet;
        Point frameSize = new Point(40, 34);

        //Sound effect(s) for Mario
        SoundEffect deathEffect, jumpSound;

        //Current Level Mario is running in.
        assignment_4.Level currentLevel;

        public Mario(assignment_4.Level currentLevel, Vector2 position, InputHandler inputHandler)
        {
            startingPosition = position;
            this.position = startingPosition;
            this.inputHandler = inputHandler;
            this.currentLevel = currentLevel;
            InitialiseMario();
            InitialiseAnimationFrames();
        }

        /// <summary>
        /// Provide Mario with his own content loading code.
        /// This is called during the content load in the main game.
        /// </summary>
        public void LoadContent(ContentManager content)
        {
            //We load our sprite sheet "Mario.png" into the Texture2D variable.
            spriteSheet = content.Load<Texture2D>("Images/Mario");

            //Loading in the sound effect for playing later.
            deathEffect = content.Load<SoundEffect>("Sound/Death");
            jumpSound = content.Load<SoundEffect>("Sound/Jump2");
        }

        /// <summary>
        /// Private method that allows us to prepare
        /// basic information about the current state of
        /// Mario himself.
        /// </summary>
        private void InitialiseMario()
        {
            currentDirection = MarioDirection.Right;
            currentAction = MarioAction.Stand;
            numberOfLives = 3;
            yVelocity = 0.0f;
        }

        /// <summary>
        /// Once the instance of Mario is created, we want to store references
        /// to all the useful images on the sprite sheet depending on what action
        /// Mario is conducting.  We achieve this by creating AnimationFrames instances
        /// for each different action Mario is carrying out.
        /// </summary>
        private void InitialiseAnimationFrames()
        {
            timeSinceLastFrame = 0; //Set the frame update to zero.

            animationSets = new Dictionary<Tuple<MarioDirection, MarioAction>, AnimationFrames>();

            /**
             * For each set of frames, we start by giving the first and potentially only
             * frame of the animation as a location within the sprite sheet.  Think of the
             * sheet as an x*y 2D array, and we pointing out the specific locations within
             * that array that we want to start the animation.  We then add new frames as
             * we wish. The first four represent crouching and standing and only have
             * one frame in their animation.  */
            AnimationFrames standRight = new AnimationFrames(new Point(5, 7));
            AnimationFrames standLeft = new AnimationFrames(new Point(4, 7));
            AnimationFrames crouchRight = new AnimationFrames(new Point(9, 7));
            AnimationFrames crouchLeft = new AnimationFrames(new Point(0, 7));

            //I add two new animation sets for jumping/falling either direction.
            AnimationFrames jumpFallRight = new AnimationFrames(new Point(8, 7));
            AnimationFrames jumpFallLeft = new AnimationFrames(new Point(1, 7));

            /* Meanwhile, walking carries multiple frames.
             * We give it the first frame, followed by others we want.*/
            AnimationFrames walkRight = new AnimationFrames(new Point(5, 7));
            walkRight.AddFrame(new Point(6, 7));
            walkRight.AddFrame(new Point(7, 7));
            
            AnimationFrames walkLeft = new AnimationFrames(new Point(4, 7));
            walkLeft.AddFrame(new Point(3, 7));
            walkLeft.AddFrame(new Point(2, 7));

            //Add all of these animation sets to our dictionary.  We can then call upon them at will.
            animationSets.Add(new Tuple<MarioDirection, MarioAction>(MarioDirection.Left, MarioAction.Stand), standLeft);
            animationSets.Add(new Tuple<MarioDirection, MarioAction>(MarioDirection.Right, MarioAction.Stand), standRight);
            animationSets.Add(new Tuple<MarioDirection, MarioAction>(MarioDirection.Left, MarioAction.Crouch), crouchLeft);
            animationSets.Add(new Tuple<MarioDirection, MarioAction>(MarioDirection.Right, MarioAction.Crouch), crouchRight);
            animationSets.Add(new Tuple<MarioDirection, MarioAction>(MarioDirection.Left, MarioAction.Walk), walkLeft);
            animationSets.Add(new Tuple<MarioDirection, MarioAction>(MarioDirection.Right, MarioAction.Walk), walkRight);

            //Add the jump/fall animations.
            animationSets.Add(new Tuple<MarioDirection, MarioAction>(MarioDirection.Left, MarioAction.JumpFall), jumpFallLeft);
            animationSets.Add(new Tuple<MarioDirection, MarioAction>(MarioDirection.Right, MarioAction.JumpFall), jumpFallRight);
        }

        /// <summary>
        /// Straightforward accessor for the position vector.
        /// </summary>
        public Vector2 Position
        {
            get
            {
                return position;
            }
        }

        /// <summary>
        /// Represents the number of lives Mario currently has.
        /// </summary>
        public int Lives
        {
            get
            {
                return numberOfLives;
            }
        }

        /// <summary>
        /// This is an accessor for the bounding box.
        /// The bounding box is what we use to represent the 
        /// bounds of Mario himself.  What is the size of Mario and 
        /// when will he collide with objects?
        /// 
        /// Bounding boxes are very important in games to recognise when
        /// characters or objects should collide with one another.  It is
        /// often the case that one character will have a variety of different
        /// bounding boxes based on their current state.  In 2D fighting games for 
        /// example, each character will have unique bounding boxes for each
        /// move, dictating where you can hit that character.
        /// </summary>
        public Rectangle BoundingBox
        {
            get
            {
                //Note I have spent trial and error getting this just right.
                return new Rectangle((int)position.X + (int)(frameSize.X / 1.25), (int)position.Y, 10 * (int)textureScale, 26 * (int)textureScale);
            }
        }

        /// <summary>
        /// Updates Mario having killed him.
        /// </summary>
        public void KillMario()
        {
            numberOfLives -= 1;
            position = startingPosition;
            
            //Play the sound effect.
            deathEffect.Play();

            Thread.Sleep(deathEffect.Duration);
        }

        public void Update(GameTime gameTime)
        {
            
            //We store the new position in a temporary variable, in case we need to edit it.
            Vector2 updatedPosition = Position;

            //First we update the position based on the user input
            updatedPosition = ApplyUserInput(updatedPosition);

            //Next up we update the position based on collision detection.
            updatedPosition = HandleCollisions(updatedPosition);

            //Next we apply gravity.
            Tuple<Vector2, float> gravityResult = assignment_4.Physics.ApplyGravityToVector2(updatedPosition, yVelocity, gameTime);
            updatedPosition = gravityResult.Item1;
            yVelocity = gravityResult.Item2;

            //Next up we update the position based on collision detection.
            updatedPosition = HandleCollisions(updatedPosition);

            position = new Vector2((int)updatedPosition.X, (int)updatedPosition.Y);

            if (yVelocity <= -1.5 || yVelocity >= 1.5 || isJumping)
            {
                currentAction = MarioAction.JumpFall;
            }

            //Now handle the animations.
            ManageAnimations(gameTime);
        }

        private Vector2 ApplyUserInput(Vector2 updatedPosition)
        {
            //First update the input handler to the current state.
            inputHandler.UpdateInput();

            //If pressing down, crouch.
            if (inputHandler.IsDownPressed())
            {
                currentAction = MarioAction.Crouch;
            }
            else //For the simple reason that we can't walk and crouch!
            {
                //If we know he is not crouching, make him stand at least.
                currentAction = MarioAction.Stand;

                //Check if left is pressed on either keyboard or pad.
                if (inputHandler.IsLeftPressed())
                {
                    //Set the actions
                    currentDirection = MarioDirection.Left;
                    currentAction = MarioAction.Walk;

                    //Move according to the input.
                    if (inputHandler.GamePadActive() && inputHandler.LeftStick.X != 0)
                    {
                        updatedPosition.X += (moveSpeed * inputHandler.LeftStick.X);
                    }
                    else
                    {
                        updatedPosition.X -= moveSpeed;
                    }
                }
                else if (inputHandler.IsRightPressed()) //Similar process for pressing right.
                {
                    currentDirection = MarioDirection.Right;
                    currentAction = MarioAction.Walk;

                    if (inputHandler.GamePadActive() && inputHandler.LeftStick.X != 0)
                    {
                        updatedPosition.X += moveSpeed * inputHandler.LeftStick.X;
                    }
                    else
                    {
                        updatedPosition.X += moveSpeed;
                    }
                }
                
                if (inputHandler.IsKeyPressed(Keys.Up) || inputHandler.IsButtonPressed(Buttons.A)) //We have decide Mario will jump!
                {
                    if (!isJumping)
                    {
                        JumpMario();
                    }
                }

            }
            return updatedPosition;
        }

        private void JumpMario()
        {

            yVelocity = InitJumpVelocity;
            isJumping = true;
            jumpSound.Play();
            currentAction = MarioAction.JumpFall;
        }

        private Vector2 HandleCollisions(Vector2 newPosition)
        {
            /**
             * Managing collisions in a 2D platformer is frankly not a straightforward process.
             * If it is to be done properly, we must gain an understanding not only of what
             * two objects collided, but also how they collided.  Since if they do collide, we must then
             * place Mario into a 'safe' position, i.e. he is not stuck within a part of the
             * obstacles.  This is a rather complicated process which we will now go into below.
             * 
             * We begin by checking whether Mario collides with any of the tiles in the environment.
             * If we know he has not collided with anything, we can simply update his position without
             * any problems.
             **/

            //Keep track of collisions with these variables.
            bool collides = false;
            assignment_4.Tile collidingTile = null;

            foreach (assignment_4.Tile currentTile in currentLevel.Tiles)
            {
                if (currentTile.BoundingBox.Intersects(BoundingBox))
                {
                    collides = true;
                    collidingTile = currentTile;
                    isJumping = false;
                }
            }

            /**
             * If he has collided, we need to then determine how to set his position based
             * upon the tile he collided with.
             */
            if (collides)
            {
                /* We start by calculating the depth of the intersection, i.e. the level of
                 * overlap between the two bounding boxes of Mario and the tile.  The
                 * code for this is found in the RectangleExtensions class.  This class
                 * is taken from Microsofts platformer example.
                 */
                Vector2 collisionDepth = assignment_4.RectangleExtensions.GetIntersectionDepth(BoundingBox, collidingTile.BoundingBox);

                //If there is a depth to the intersection, we need to fix Marios new position.
                if (collisionDepth != Vector2.Zero)
                {
                    //Get the absolute depth, given that the depth can be negative
                    float depthX = Math.Abs(collisionDepth.X);
                    float depthY = Math.Abs(collisionDepth.Y);

                    /* Resolve the collision along the shallowest axis, 
                     * simply push him out to beyond the position of the tile.
                     **/
                    if (depthY < depthX)
                    {
                        // Resolve the collision along the Y axis.
                        newPosition = new Vector2(Position.X, Position.Y + collisionDepth.Y);
                        yVelocity = 0.0f;
                    }
                    else
                    {
                        // Resolve the collision along the X axis.
                        newPosition = new Vector2(Position.X + collisionDepth.X, Position.Y);
                    }

                }
            }

            return newPosition;
        }

        private void ManageAnimations(GameTime gameTime)
        {
            timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;

            //Here we update the currently active sprite sheet.
            if (timeSinceLastFrame > millisecondsPerFrame)
            {
                timeSinceLastFrame -= millisecondsPerFrame;
                //Update the current animations frames.
                animationSets[new Tuple<MarioDirection, MarioAction>(currentDirection, currentAction)].UpdateAnimationFrame();
            }
        }

        /// <summary>
        /// The Draw method for Mario is a little more complicated, since we actually focus
        /// on where on a particular image we want to draw from.  Here we use the AnimationFrame
        /// instances to grab the particular set we want and then grab the current frame from
        /// that set to draw to the screen.
        /// </summary>
        /// <param name="gameTime">Current game time.</param>
        /// <param name="spriteBatch">The sprite batch being used by the GDI.</param>
        public void DrawMario(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //Grab the current frame of the sprite sheet that our animation is viewing.
            Point currentFrame = animationSets[new Tuple<MarioDirection, MarioAction>(currentDirection, currentAction)].CurrentFrame;

            if (assignment_4.Game1.COLLISION_DEBUG)
            {
                Texture2D simpleTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                simpleTexture.SetData(new Color[] { Color.Tomato });
                spriteBatch.Draw(simpleTexture, BoundingBox, Color.Tomato);
            }
            else
            {
                spriteBatch.Draw(spriteSheet,        //The texture we want to draw from.
                                    new Vector2((int)position.X - (frameSize.X / 2), (int)position.Y - (frameSize.Y / 2)),           //The position of the sprite.
                                    new Rectangle(currentFrame.X * frameSize.X, currentFrame.Y * frameSize.Y, frameSize.X, frameSize.Y), //Point on the texture we are drawing from.
                                    Color.White,        //The color tint (i.e. white = none)
                                    0,                  //The rotation of the sprite.
                                    Vector2.Zero,       //The origin of the sprite.
                                    textureScale,                  //Scale of the texture
                                    SpriteEffects.None, //Any effects being conducted.
                                    0);                 //Layer depth.
            } 

        }
    }
}
