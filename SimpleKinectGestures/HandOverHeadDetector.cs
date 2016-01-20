using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace SimpleKinectGestures
{
    public class HandOverHeadDetector : HandGestureBase
    {
        private bool _currentGestureStatus;
        private bool _isGestureInProgress;
        private int _registerCounter;

        public HandOverHeadDetector(HandDetectionType handDetectionType, HandState state, int framesToRegister = 10,
            double headOffset = 0, bool headOffsetAutomatic = true)
            : base(handDetectionType, state)
        {
            FramesToRegister = framesToRegister;
            HeadOffset = headOffset;
            HeadOffsetAutomatic = headOffsetAutomatic;
        }

        public double HeadOffset { get; set; }

        /// <summary>
        ///     The time in Frames the Hand/Hands needs to be above the Head
        /// </summary>
        public int FramesToRegister { get; set; }

        public bool HeadOffsetAutomatic { get; set; }

        /// <summary>
        ///     Gets called if the event was succesfull
        /// </summary>
        public event EventHandler GestureCompleteEvent;

        public event StatusUpdateEventHandler StatusUpdateEvent;

        public bool UpdateData(Body[] bodies)
        {
            var returnVal = false;
            foreach (Body body in bodies)
            {
                if (body.IsTracked)
                {
                    if (IsStateCorrect(body))
                    {
                        if (IsHandOverHead(body))
                        {
                            if (StatusUpdateEvent != null)
                                StatusUpdateEvent(this,
                                    new StatusUpdateEventArgs("In Progress Overhead! " + _registerCounter));
                            if (_isGestureInProgress)
                            {
                                if (StatusUpdateEvent != null)
                                    StatusUpdateEvent(this,
                                        new StatusUpdateEventArgs("In Progress Frames: " + _registerCounter));
                                _registerCounter++;
                                _currentGestureStatus = _registerCounter >= FramesToRegister;
                                if (_currentGestureStatus)
                                    StateFailCount = JointStateFailsAllowedMax;
                            }
                            else
                            {
                                if (StatusUpdateEvent != null)
                                    StatusUpdateEvent(this, new StatusUpdateEventArgs("In Progress"));
                                _isGestureInProgress = true;
                                _registerCounter = 0;
                            }
                        }
                        else
                        {
                            _registerCounter = 0;
                        }
                    }
                    else
                    {
                        _registerCounter = 0;
                        if (_currentGestureStatus)
                        {
                            Suscess();
                            returnVal = true;
                        }
                    }
                }
            }
            return returnVal;
        }

        private void Suscess()
        {
            if (StatusUpdateEvent != null)
                StatusUpdateEvent(this,
                    new StatusUpdateEventArgs("Complete Frames: " + _registerCounter));
            if (GestureCompleteEvent != null) GestureCompleteEvent(this, new EventArgs());
            _isGestureInProgress = false;
            _currentGestureStatus = false;
        }

        private double GetHeadOffset(Body body)
        {
            var returnVal = HeadOffset;
            if (HeadOffsetAutomatic)
                returnVal = body.Joints[JointType.Head].Position.Y - body.Joints[JointType.Neck].Position.Y;
            return returnVal;
        }

        private bool IsHandOverHead(Body body)
        {
            var returnVal = false;
            var headY = body.Joints[JointType.Head].Position.Y + GetHeadOffset(body);
            switch (RequirredHandDetectionType)
            {
                case SimpleKinectGestures.HandDetectionType.BothHands:
                    if (body.Joints[JointType.HandLeft].Position.Y > headY ||
                        body.Joints[JointType.HandRight].Position.Y > headY)
                        returnVal = true;
                    break;

                case SimpleKinectGestures.HandDetectionType.BothHandsRequired:
                    if (body.Joints[JointType.HandLeft].Position.Y > headY &&
                        body.Joints[JointType.HandRight].Position.Y > headY)
                        returnVal = true;
                    break;
                case SimpleKinectGestures.HandDetectionType.LeftHand:
                    if (body.Joints[JointType.HandLeft].Position.Y > headY)
                        returnVal = true;
                    break;
                case HandDetectionType.RightHand:
                    if (body.Joints[JointType.HandRight].Position.Y > headY)
                        returnVal = true;
                    break;
            }
            return returnVal;
        }
    }
}
