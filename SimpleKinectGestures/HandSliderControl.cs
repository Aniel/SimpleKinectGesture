using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace SimpleKinectGestures
{
    public class HandSliderControl : HandGestureBase
    {
        private bool _isGestureInProgress;
        private CoordinateMapper _mapper;
        private double _path;
        private double _startPos;
        public double Value;
        public double Max = 100;
        public double Min = 0;

        public Directions Direction { get; set; }

        public double SmoothLevel { get; set; }

        public event GestureUpdateEventHandler GestureUpdate;
        public event StatusUpdateEventHandler StatusUpdate;


        public HandSliderControl(HandDetectionType handDetectionType, HandState requirredHandState,
            Directions direction, CoordinateMapper mapper)
            : base(handDetectionType, requirredHandState)
        {
            SmoothLevel = 0;
            Direction = direction;
            _mapper = mapper;
        }

        public double UpdateData(Body[] bodies)
        {
            foreach (Body body in bodies)
            {
                if (body.IsTracked)
                {
                    if (IsStateCorrect(body))
                    {
                        if (_isGestureInProgress)
                        {
                            if (!IsStateFailing)
                                Value = GetLevel(body);
                            StatusUpdateEventSender("Start pos: " + _startPos + "\nPath: " + _path + "\nRealLvl: " + Value);
                            if (Value < 0)
                            {
                                if (GestureUpdate != null) GestureUpdate(this, new ProgressionEventArgs(0));
                            }
                            else
                            {
                                if (Value > 100)
                                {
                                    if (GestureUpdate != null) GestureUpdate(this, new ProgressionEventArgs(100));
                                }
                                else
                                {
                                    if (GestureUpdate != null)
                                        GestureUpdate(this, new ProgressionEventArgs(Math.Round(Value)));
                                }
                            }
                        }
                        else
                        {
                            _isGestureInProgress = true;
                            SetStartPos(body);
                            SetPath(body);
                        }
                    }
                    else
                    {
                        StatusUpdateEventSender("Is Not trackable " + Direction + " " + RequirredHandDetectionType + " " + RequirredHandState);
                        _isGestureInProgress = false;

                    }
                }
            }
            return Value;
        }

        private double GetValue(double PositionProzent)
        {

            return 10;
        }

        private double GetLevel(Body body)
        {
            double retVal = 0;
            switch (RequirredHandDetectionType)
            {
                case SimpleKinectGestures.HandDetectionType.BothHandsRequired:
                    retVal = (GetPositionProzent(body, JointType.HandRight) +
                              GetPositionProzent(body, JointType.HandLeft)) / 2;
                    break;
                case SimpleKinectGestures.HandDetectionType.BothHands:
                    if (body.HandLeftState.Equals(RequirredHandState) && body.HandRightState.Equals(RequirredHandState))
                    {
                        retVal = (GetPositionProzent(body, JointType.HandRight) +
                                  GetPositionProzent(body, JointType.HandLeft)) / 2;
                    }
                    else
                    {
                        if (body.HandLeftState.Equals(RequirredHandState))
                        {
                            retVal = GetPositionProzent(body, JointType.HandLeft);
                        }
                        if (body.HandRightState.Equals(RequirredHandState))
                        {
                            retVal = GetPositionProzent(body, JointType.HandRight);
                        }
                    }
                    break;
                case SimpleKinectGestures.HandDetectionType.RightHand:
                    retVal = GetPositionProzent(body, JointType.HandRight);
                    break;
                case SimpleKinectGestures.HandDetectionType.LeftHand:
                    retVal = GetPositionProzent(body, JointType.HandLeft);
                    break;
            }
            return retVal;
        }

        private double GetPositionProzent(Body body, JointType type)
        {
            double retVal = 0;
            switch (Direction)
            {
                case Directions.X:
                    retVal = (((_startPos + (_path / 2)) -
                               (_mapper.MapCameraPointToDepthSpace(body.Joints[type].Position).X)) / _path) * 100;
                    break;
                case Directions.Y:
                    retVal = (((_startPos + (_path / 2)) -
                               (_mapper.MapCameraPointToDepthSpace(body.Joints[type].Position).Y)) / _path) * 100;
                    break;
            }

            return retVal;
        }

        private void SetPath(Body body)
        {
            switch (Direction)
            {
                case Directions.X:
                    _path = _mapper.MapCameraPointToDepthSpace(body.Joints[JointType.AnkleLeft].Position).X -
                            _mapper.MapCameraPointToDepthSpace(body.Joints[JointType.AnkleRight].Position).X;
                    break;
                case Directions.Y:
                    _path = _mapper.MapCameraPointToDepthSpace(body.Joints[JointType.SpineMid].Position).Y -
                            _mapper.MapCameraPointToDepthSpace(body.Joints[JointType.SpineShoulder].Position).Y;
                    break;
            }
        }

        private void SetStartPos(Body body)
        {
            switch (Direction)
            {
                case Directions.X:
                    switch (RequirredHandDetectionType)
                    {
                        case SimpleKinectGestures.HandDetectionType.BothHandsRequired:
                            _startPos =
                                (_mapper.MapCameraPointToDepthSpace(body.Joints[JointType.HandLeft].Position).X +
                                 _mapper.MapCameraPointToDepthSpace(body.Joints[JointType.HandRight].Position).X) / 2;
                            break;
                        case SimpleKinectGestures.HandDetectionType.BothHands:
                            if (body.HandLeftState.Equals(RequirredHandState) &&
                                body.HandRightState.Equals(RequirredHandState))
                            {
                                _startPos =
                                    (_mapper.MapCameraPointToDepthSpace(body.Joints[JointType.HandLeft].Position).X +
                                     _mapper.MapCameraPointToDepthSpace(body.Joints[JointType.HandRight].Position).X) / 2;
                            }
                            else
                            {
                                if (body.HandLeftState.Equals(RequirredHandState))
                                {
                                    _startPos =
                                        _mapper.MapCameraPointToDepthSpace(body.Joints[JointType.HandLeft].Position).X;
                                }
                                if (body.HandRightState.Equals(RequirredHandState))
                                {
                                    _startPos =
                                        _mapper.MapCameraPointToDepthSpace(body.Joints[JointType.HandRight].Position).X;
                                    ;
                                }
                            }
                            break;
                        case SimpleKinectGestures.HandDetectionType.RightHand:
                            _startPos = _mapper.MapCameraPointToDepthSpace(body.Joints[JointType.HandRight].Position).X;
                            break;
                        case SimpleKinectGestures.HandDetectionType.LeftHand:
                            _startPos = _mapper.MapCameraPointToDepthSpace(body.Joints[JointType.HandLeft].Position).X;
                            break;
                    }
                    break;
                case Directions.Y:
                    switch (RequirredHandDetectionType)
                    {
                        case SimpleKinectGestures.HandDetectionType.BothHandsRequired:
                            _startPos =
                                (_mapper.MapCameraPointToDepthSpace(body.Joints[JointType.HandLeft].Position).Y +
                                 _mapper.MapCameraPointToDepthSpace(body.Joints[JointType.HandRight].Position).Y) / 2;
                            break;
                        case SimpleKinectGestures.HandDetectionType.BothHands:
                            if (body.HandLeftState.Equals(RequirredHandState) &&
                                body.HandRightState.Equals(RequirredHandState))
                            {
                                _startPos =
                                    (_mapper.MapCameraPointToDepthSpace(body.Joints[JointType.HandLeft].Position).Y +
                                     _mapper.MapCameraPointToDepthSpace(body.Joints[JointType.HandRight].Position).Y) / 2;
                            }
                            else
                            {
                                if (body.HandLeftState.Equals(RequirredHandState))
                                {
                                    _startPos =
                                        _mapper.MapCameraPointToDepthSpace(body.Joints[JointType.HandLeft].Position).Y;
                                }
                                if (body.HandRightState.Equals(RequirredHandState))
                                {
                                    _startPos =
                                        _mapper.MapCameraPointToDepthSpace(body.Joints[JointType.HandRight].Position).Y;
                                }
                            }
                            break;
                        case SimpleKinectGestures.HandDetectionType.RightHand:
                            _startPos = _mapper.MapCameraPointToDepthSpace(body.Joints[JointType.HandRight].Position).Y;
                            break;
                        case SimpleKinectGestures.HandDetectionType.LeftHand:
                            _startPos = _mapper.MapCameraPointToDepthSpace(body.Joints[JointType.HandLeft].Position).Y;
                            break;
                    }
                    break;
            }
        }

        private void StatusUpdateEventSender(string msg)
        {
            if (StatusUpdate != null) StatusUpdate(this, new StatusUpdateEventArgs(msg));
        }
    }
}
