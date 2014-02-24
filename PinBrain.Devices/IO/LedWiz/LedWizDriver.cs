using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//using AxLEDWiz_Control;

namespace PinBrain.Devices.IO.LedWiz
{
    public class LedWizDriver
    {
        //private AxLED_Wiz _wiz;
        private string _name;

        public LedWizDriver(string name)
        {
            _name = name;
            //_wiz = new AxLED_Wiz();
        }
    }
}
