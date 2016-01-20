using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleKinectGestures
{
        /// <summary>
        ///     The possible detection types
        /// </summary>
        public enum HandDetectionType
        {
            /// <summary>
            ///     Both hands are required
            /// </summary>
            BothHandsRequired,

            /// <summary>
            ///     Either the left or the right hand can be used
            /// </summary>
            BothHands,

            /// <summary>
            ///     The right hand is required
            /// </summary>
            RightHand,

            /// <summary>
            ///     The left hand is required
            /// </summary>
            LeftHand
        }

        public enum Directions
        {
            X,
            Y
        }
}
