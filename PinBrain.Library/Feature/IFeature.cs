using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PinBrain.Library.Feature
{
    public class DisplayEventArgs : EventArgs
    {
        public string SceneName;
        public string e;
        public DisplayEventArgs(object scene, string arg)
        {
            if (scene != null)
                SceneName = scene.ToString();
            e = arg;
        }
    }

    public delegate void DisplayEventHandler(DisplayEventArgs args);

    public interface IFeature
    {
        string Name
        { get; }
    }
}
