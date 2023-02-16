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
    public enum PeachDirection
    {
        Left, Right, Up, Down
    }

    public enum PeachAction
    {
        Stand, Walk, Run, JumpFall, Crouch
    }

    /// <summary>
    /// this class is very similar to the Mario class,
    /// however the Inputhandler functions are different.
    /// </summary>
    public class Peach
    {

        //A 2D vector that is used to represent the (x,y) position of the character.
        Vector2 position, startingPosition;
        PeachAction currentAction;
        PeachDirection currentDirection;

        //Moving speed used for both keyboard and gamepad.
         float moveSpeed = 4.0f;

        //Some variables for the jump code.
        const float InitJumpVelocity = -6.0f;
        float yVelocity;
        bool isJumping = false;


        //Code to handle the Input for Player1
        InputHandler inputHandler;

        int timeSinceLastFrame;
        const int millisecondsPerFrame = 50;
        const float textureScale = 3.0f;

        Dictionary<Tuple<PeachDirection, PeachAction>, AnimationFrames> animationSets;

        //A texture variable that stores the full Mario sprite sheet.
        Texture2D spriteSheet;
        Point frameSize = new Point(40, 34);

        //Current Level Player1 is running in.
        Level currentLevel;

        public Peach(Level currentLevel, Vector2 position, InputHandler inputHandler)
        {
            startingPosition = position;
            this.position = startingPosition;
            this.inputHandler = inputHandler;
            this.currentLevel = currentLevel;
            InitialiseMario();
            InitialiseAnimationFrames();
        }

        /// <summary>
        /// This function is used by the mysterybox item, if the random number
        /// generated, executed the code to increase users speed.
        /// </summary>
        /// <param name="speed"></param>
        public void setSpeed(float speed)
        {
            moveSpeed = speed;
        }

        /// <summary>
        /// Provide Mario with his own content loading code.
        /// This is called during the content load in the main game.
        /// </summary>
        public void LoadContent(ContentManager content)
        {
            //We load our sprite sheet "Mario.png" into the Texture2D variable.
            spriteSheet = content.Load<Texture2D>("Images/Mario");
        }

        /// <summary>
        /// Private method that allows us to prepare
        /// basic information about the current state of
        /// Mario himself.
        /// </summary>
        private void InitialiseMario()
        {
            currentDirection = PeachDirection.Right;
            currentAction = PeachAction.Stand;
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

            animationSets = new Dictionary<Tuple<PeachDirection, PeachAction>, AnimationFrames>();

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

            AnimationFrames walkLeft = new AnimationFrames(new Point(0, 1));
            walkLeft.AddFrame(new Point(1, 1));
            walkLeft.AddFrame(new Point(1, 1));

            //Add all of these animation sets to our dictionary.  We can then call upon them at will.
            animationSets.Add(new Tuple<PeachDirection, PeachAction>(PeachDirection.Left, PeachAction.Stand), standLeft);
            animationSets.Add(new Tuple<PeachDirection, PeachAction>(PeachDirection.Right, PeachAction.Stand), standRight);
            animationSets.Add(new Tuple<PeachDirection, PeachAction>(PeachDirection.Left, PeachAction.Crouch), crouchLeft);
            animationSets.Add(new Tuple<PeachDirection, PeachAction>(PeachDirection.Right, PeachAction.Crouch), crouchRight);
            animationSets.Add(new Tuple<PeachDirection, PeachAction>(PeachDirection.Left, PeachAction.Walk), walkLeft);
            animationSets.Add(new Tuple<PeachDirection, PeachAction>(PeachDirection.Right, PeachAction.Walk), walkRight);

            //Add the jump/fall animations.
            animationSets.Add(new Tuple<PeachDirection, PeachAction>(PeachDirection.Left, PeachAction.JumpFall), jumpFallLeft);
            animationSets.Add(new Tuple<PeachDirection, PeachAction>(PeachDirection.Right, PeachAction.JumpFall), jumpFallRight);
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

        public Rectangle BoundingBox
        {
            get
            {
                //Note I have spent trial and error getting this just right.
                return new Rectangle((int)position.X + (int)(frameSize.X / 1.25), (int)position.Y, 10 * (int)textureScale, 26 * (int)textureScale);
            }
        }

        /// <summary>
        /// re-poosition player1 when called.
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
                currentAction = PeachAction.JumpFall;
            }

            //Now handle the animations.
            ManageAnimations(gameTime);
        }

        private Vector2 ApplyUserInput(Vector2 updatedPosition)
        {
            //First update the input handler to the current state.
            inputHandler.UpdateInput();

            //If pressing down, crouch.
            if (inputHandler.IsPlayer2DownPressed())
            {
                currentAction = PeachAction.Crouch;
            }
            else 
            {
                if (currentAction == PeachAction.JumpFall)
                {
                }
                else
                {
                    currentAction = PeachAction.Stand;
                }


                //Check if left is pressed on either keyboard or pad.
                if (inputHandler.IsPlayer2LeftPressed())
                {
                    if (currentAction == PeachAction.JumpFall)
                    {
                    }
                    else
                    {
                        currentDirection = PeachDirection.Right;
                        currentAction = PeachAction.Walk;
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
                else if (inputHandler.IsPlayer2RightPressed()) //Similar process for pressing right.
                {
                    if (currentAction == PeachAction.JumpFall)
                    {
                    }
                    else
                    {
                        currentDirection = PeachDirection.Right;
                        currentAction = PeachAction.Walk;
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

                if (inputHandler.IsPlayer2UpPressed()) //player 2 jumps
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
            isJumping = true;;
            currentAction = PeachAction.JumpFall;
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
                    if (currentAction == PeachAction.Crouch)
                    {
                    }
                    else
                    {
                        currentAction = PeachAction.Stand;
                    }
                }
            }

            if (collides)
            {
                Vector2 collisionDepth = RectangleExtensions.GetIntersectionDepth(BoundingBox, collidingTile.BoundingBox);

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
                animationSets[new Tuple<PeachDirection, PeachAction>(currentDirection, currentAction)].UpdateAnimationFrame();
            }
        }

        public void DrawMario(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Point currentFrame = animationSets[new Tuple<PeachDirection, PeachAction>(currentDirection, currentAction)].CurrentFrame;

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
                                    Color.SpringGreen,        //The color tint (i.e. white = none)
                                    0,                  //The rotation of the sprite.
                                    Vector2.Zero,       //The origin of the sprite.
                                    textureScale,                  //Scale of the texture
                                    SpriteEffects.None, //Any effects being conducted.
                                    0);                 //Layer depth.
                currentDirection = PeachDirection.Right;
            }
        }
    }
}

