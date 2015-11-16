using System;

namespace CKinect
{
    public class GestureEventArgs : EventArgs
    {
        public string GestureName { get; private set; }

        public bool IsBodyTrackingIdValid { get; private set; }
    
        public bool IsGestureDetected { get; private set; }
    
        public float DetectionConfidence { get; private set; }

        public GestureDetector Detector{ get; private set; }
    
        public GestureEventArgs(string gestureName, bool isBodyTrackingIdValid, bool isGestureDetected, float detectionConfidence, GestureDetector detector = null)
        {
            this.GestureName = gestureName;
            this.IsBodyTrackingIdValid = isBodyTrackingIdValid;
            this.IsGestureDetected = isGestureDetected;
            this.DetectionConfidence = detectionConfidence;
            this.Detector = detector;
        }
    }
}