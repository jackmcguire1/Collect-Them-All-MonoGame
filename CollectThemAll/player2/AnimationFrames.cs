using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace assignment_4
{
    /// <summary>
    /// A simple class to keep track of which animation frame I am currently working with.
    /// This is useful when dealing with large sprite sheets where we need to move around
    /// the sheet continuously to grab different sprites.
    /// 
    /// Tommy Thompson 21/12/12
    /// </summary>
    public class AnimationFrames
    {
        //Main a list of all frames and the current frame we are using.
        List<Point> frames;
        int currentFrame;

        //Indicates whether we are moving forwards or backwards through the animation.
        bool forwards;

        /// <summary>
        /// With the constructor we store the starting frame of the animaiton.
        /// </summary>
        /// <param name="startFrame">The very first (and potentially only) frame.</param>
        public AnimationFrames(Point startFrame)
        {
            currentFrame = 0;
            frames = new List<Point>();
            frames.Add(startFrame);
            forwards = true;
        }

        /// <summary>
        /// Add a new frame to the animation.
        /// </summary>
        /// <param name="newFrame">The next frame of the animation.</param>
        public void AddFrame(Point newFrame)
        {
            frames.Add(newFrame);
        }

        /// <summary>
        /// The current frame of this animation.
        /// </summary>
        public Point CurrentFrame
        {
            get
            {
                return frames[currentFrame];
            }
        }

        /// <summary>
        /// Updates the current frame being animated.
        /// </summary>
        public void UpdateAnimationFrame()
        {
            if (forwards)
            {
                currentFrame++;

                if (currentFrame >= frames.Count)
                {
                    currentFrame = frames.Count-1;
                    forwards = false;
                }
            }
            else //Go backwards through the animation.
            {
                currentFrame--;

                if (currentFrame < 0)
                {
                    forwards = true;
                    currentFrame = 0;
                }
            }
            
        }
    }
}
