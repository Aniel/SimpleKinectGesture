using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleKinectGestures
{
    public delegate void StatusUpdateEventHandler(object o, StatusUpdateEventArgs args);

    public delegate void GestureUpdateEventHandler(object o, ProgressionEventArgs args);

    public class ProgressionEventArgs : EventArgs
    {
        public ProgressionEventArgs(double level)
        {
            Level = level;
        }

        public double Level { get; set; }
    }

    public class StatusUpdateEventArgs : EventArgs
    {
        public StatusUpdateEventArgs(string msg)
        {
            Msg = msg;
        }

        public string Msg { get; set; }
    }
}
