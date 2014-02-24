using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.IO.Ports;
using PinBrain.Library.Switch;

namespace PinBrain.Devices.IO.Serial
{
    /// <summary>
    /// DO NOT USE THIS FOR REAL TIME ANYTHING. 2/12
    /// </summary>
    public abstract class Arduino
    {
        protected static readonly new ILog _log = LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected bool _isDisposed = false;
        protected readonly SerialPort _serial;
        protected string _lastData = string.Empty;
        protected bool _stopped = false;

        public Arduino(string port)
        {
            _serial = new SerialPort(port) { BaudRate = 115200 };
            //  hook up the event for receiving the data
            _serial.DataReceived += _port_DataReceived;
            _serial.Open();
        }

        #region * Dispose *
        //Call Dispose to free resources explicitly
        public void Dispose()
        {
            Dispose(true);
            //If dispose is called already then say GC to skip finalize on this instance.
            GC.SuppressFinalize(this);
        }

        ~Arduino()
        {
            Dispose(false);
        }

        //Implement dispose to free resources
        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                _isDisposed = true;
                // Released unmanaged Resources
                if (disposing)
                {
                    // Released managed Resources
                    if (_serial != null)
                        _serial.Dispose();
                }
            }
        }
        #endregion

        public void SendCommand(byte[] b)
        {
            //  Signal the Arduino Board to start sending data
            _serial.Write(b, 0, b.Length);
        }

        private void _port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (_stopped) return;
            // Send data received from the Arduino to the Listbox on the UI thread

            string data = _serial.ReadExisting();
            _lastData += data;
            string[] allData = _lastData.Split('\n');
            _lastData = allData[allData.Length - 1];
            foreach (var word in allData.Take(allData.Length - 1))
            {
                _log.DebugFormat("Data from Serial{0}: {1}", _serial.PortName, word);
            }
        }
    }
}
