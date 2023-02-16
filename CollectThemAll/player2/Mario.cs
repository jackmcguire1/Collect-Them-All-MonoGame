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

namespace assignment_4
{
    public enum MarioDirection
    {
        Left, Right, Up, Down
    }

    public enum MarioAction
    {
        Stand, Walk, Run, JumpFall, Crouch
    }


    public class Mario
    {

        //A 2D vector that is used to represent the (x,y) position of Mario.
        Vector2 position, startingPosition;
        MarioAction currentAction;
        MarioDirection currentDirection;

        //Moving speed used for both keyboard and gamepad.
        float moveSpeed = 4.0f;

        //Some variables for the jump code.
        const float InitJumpVelocity = -6.0f;
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



        //Current Level Mario is running in.
        Level currentLevel;

        public Mario(Level currentLevel, Vector2 position, InputHandler inputHandler)
        {
            startingPosition = position;
            this.position = startingPosition;
            this.inputHandler = inputHandler;
            this.currentLevel = currentLevel;
            InitialiseMario();
            InitialiseAnimationFrames();
        }


        public void setSpeed(float speed)
        {
            moveSpeed = speed;
        }

        /// <summary>
        /// Loads content required in order to use character sorite.
        /// </summary>
        public void LoadContent(ContentManager content)
        {
            spriteSheet = content.Load<Texture2D>("Images/Mario");
        }

        /// <summary>
        /// sets values for the player accordingly.
        /// </summary>
        private void InitialiseMario()
        {
            currentDirection = MarioDirection.Right;
            currentAction = MarioAction.Stand;
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
        /// This is the frame which is used for collision detection.
        /// </summary>
        public Rectangle BoundingBox
        {
            get
            {
                return new Rectangle((int)position.X + (int)(frameSize.X / 1.25), (int)position.Y, 10 * (int)textureScale, 26 * (int)textureScale);
            }
        }

        /// <summary>
        /// Re-positions the character to the starting position, if called.
        /// </summary>
        public void KillMario()
        {
            position = startingPosition;
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
            Tuple<Vector2, float> gravityResult = Physics.ApplyGravityToVector2(updatedPosition, yVelocity, gameTime);
            updatedPosition = gravityResult.Item1;
            yVelocity = gravityResult.Item2;

            //Next up we update the position based on collision detection.
            updatedPosition = HandleCollisions(updatedPosition);

            position = new Vector2((int)updatedPosition.X, (int)updatedPosition.Y);

            if (yVelocity <= -1.5 || yVelocity >= 1.5)
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
                if (currentAction == MarioAction.JumpFall)
                {
                }
                else
                {
                    currentAction = MarioAction.Stand;
                }
               

                //Check if left is pressed on either keyboard or pad.
                if (inputHandler.IsLeftPressed())
                {
                    //Set the actions
                    if (currentAction == MarioAction.JumpFall)
                    {

                    }
                    else
                    {
                        currentDirection = MarioDirection.Left;
                        currentAction = MarioAction.Walk;
                    }

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
                    if (currentAction == MarioAction.JumpFall)
                    {

                    }
                    else
                    {
                        currentDirection = MarioDirection.Left;
                        currentAction = MarioAction.Walk;
                    }

                    if (inputHandler.GamePadActive() && inputHandler.LeftStick.X != 0)
                    {
                        updatedPosition.X += moveSpeed * inputHandler.LeftStick.X;
                    }
                    else
                    {
                        updatedPosition.X += moveSpeed;
                    }
                }

                if (inputHandler.IsUpPressed()) //We have decide Mario will jump!
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
            currentAction = MarioAction.JumpFall;
        }

        private Vector2 HandleCollisions(Vector2 newPosition)
        {

            bool collides = false;
            Tile collidingTile = null;

            foreach (Tile currentTile in currentLevel.Tiles)
            {
                if (currentTile.BoundingBox.Intersects(BoundingBox))
                {

                    collides = true;
                    collidingTile = currentTile;
                    isJumping = false;
                    if (currentAction == MarioAction.Crouch)
                    {
                    }
                    else
                    {
                        currentAction = MarioAction.Stand;
                    }
                }
            }

            //if collision is detected amongst the player, re-position accordingly.
            if (collides)
            {
                Vector2 collisionDepth = RectangleExtensions.GetIntersectionDepth(BoundingBox, collidingTile.BoundingBox);

                if (collisionDepth != Vector2.Zero)
                {
                    float depthX = Math.Abs(collisionDepth.X);
                    float depthY = Math.Abs(collisionDepth.Y);

                    if (depthY < depthX)
                    {
                        newPosition = new Vector2(Position.X, Position.Y + collisionDepth.Y);
                        yVelocity = 0.0f;        
                    }
                    else
                    {
                        newPosition = new Vector2(Position.X + collisionDepth.X, Position.Y); // Resolve the collision along the X axis.
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

            if (Game1.COLLISION_DEBUG)
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

                currentDirection = MarioDirection.Left;//sets player 2 facing left automatically.
            } 

        }
    }
}
