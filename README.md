# SimpleKinectGesture
A simple programmatic gesture libary for Kinect developers

## Features
- Simple to use
- Two basic gestures
  - `HandOverHead`
  - `Slider`

## Usage
Add the following code to create a gesture
```c#
	var _handOverHeadDetector = new HandOverHeadDetector(HandDetectionType.BothHands, HandState.Open);
	//Subscribe to completed event
	_handOverHeadDetector.GestureCompleteEvent += HandOverHeadDetectorOnGestureCompleteEvent;
```
In your `Reader_FrameArrived` methode update the detector with the body data
```c#
_handOverHeadDetector.UpdateData(_bodies);
```

I have created a [demo](https://github.com/Aniel/SimpleKinectGestureExample) application showing the use of this libary

## Want to help? or found a Bug?
- Please log an issue or submit a pull request

