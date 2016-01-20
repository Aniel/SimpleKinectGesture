using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace SimpleKinectGestures
{
    public abstract class HandGestureBase
    {
        private int _failsAllowed = 10;
        private HandState _requirredHandState;
        protected bool IsStateFailing;

        protected HandGestureBase(HandDetectionType handDetectionType, HandState handState)
        {
            StateFailCount = 0;
            RequirredHandDetectionType = handDetectionType;
            RequirredHandState = handState;
        }

        public HandState RequirredHandState
        {
            get { return _requirredHandState; }
            set
            {
                _requirredHandState = value;
            }
        }

        public HandDetectionType RequirredHandDetectionType { get; set; }

        public int JointStateFailsAllowedMax
        {
            get { return _failsAllowed; }
            set { _failsAllowed = value; }
        }

        protected int StateFailCount { get; set; }
        public event StatusUpdateEventHandler BaseStatusUpdateEvent;

        protected bool IsStateCorrect(Body body)
        {
            var retVal = false;
            switch (RequirredHandDetectionType)
            {
                case SimpleKinectGestures.HandDetectionType.BothHandsRequired:
                    if (body.HandLeftState.Equals(RequirredHandState) && body.HandRightState.Equals(RequirredHandState))
                        retVal = true;
                    break;
                case SimpleKinectGestures.HandDetectionType.BothHands:
                    if (body.HandLeftState.Equals(RequirredHandState) || body.HandRightState.Equals(RequirredHandState))
                        retVal = true;
                    break;
                case SimpleKinectGestures.HandDetectionType.RightHand:
                    if (body.HandRightState.Equals(RequirredHandState))
                        retVal = true;
                    break;
                case SimpleKinectGestures.HandDetectionType.LeftHand:
                    if (body.HandLeftState.Equals(RequirredHandState))
                        retVal = true;
                    break;
            }

            if (!retVal && StateFailCount < _failsAllowed)
            {
                StateFailCount++;
                retVal = true;
                IsStateFailing = true;
            }
            else
            {
                if (retVal)
                {
                    IsStateFailing = false;
                    StateFailCount = 0;
                }
            }
            if (BaseStatusUpdateEvent != null)
                BaseStatusUpdateEvent(this,
                    new StatusUpdateEventArgs("IsStateCorrect: " + retVal + " FailCounter: " + StateFailCount));
            return retVal;
        }
    }
}
