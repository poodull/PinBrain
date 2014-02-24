using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PinBrain.Library.Switch;
using System.Threading.Tasks;
using log4net;
using System.Threading;

namespace PinBrain.Devices.IO.Serial
{
    public partial class SolenoidArduino : ISwitchDriver
    {
        /// <summary>
        /// The log4net logger.
        /// </summary>
        protected static readonly ILog _log = LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public event SwitchEventHandler OnSwitchEvent;

        private COMClient _comClient;
        private Switch[] _state;
        private const int BAUDRATE = 115200;
        private string _port = "COM1";

        private byte[] lastState = new byte[COMClient.SWITCHCOUNTBYTEARRAYSIZE]; //fixed to number of bytes returned
        private byte[] justPressed = new byte[COMClient.SWITCHCOUNTBYTEARRAYSIZE];
        private byte[] justReleased = new byte[COMClient.SWITCHCOUNTBYTEARRAYSIZE];
        private byte[] changed = new byte[COMClient.SWITCHCOUNTBYTEARRAYSIZE];
        private Action<List<Switch>> _switchChangedHandler;

        public SolenoidArduino(string port, Switch[] state, Action<List<Switch>> switchChangedHandler)
        {
            _port = port;
            _log.InfoFormat("SolenoidArduino defined on port {0}", port);
            _state = state;
            _switchChangedHandler = switchChangedHandler;
            connect();

            //This is the callback for when a switch changes.
            var t = Task.Factory.StartNew(() => getSwitchStates(), TaskCreationOptions.LongRunning);
        }

        public Switch[] SwitchArrayState
        {
            get { return _state; }
        }

        void getSwitchStates()
        {
            byte[] newState; //the new state

            try
            {
                while (true) //not disposed
                {
                    if (_comClient == null || !_comClient.IsConnected) // and not disposing.
                    {
                        Thread.Sleep(1000); //not ready yet.
                        connect(); //is this smart to have this here?  Guess it should be hot-pluggable
                        continue;
                    }

                    while (handleResponses()) //do we want to block until we handle responses?
                    { }

                    newState = _comClient.SwitchMask;
                    if (newState.Length == 0)
                    {
                        //Console.WriteLine("not ready yet " + DateTime.Now.Millisecond);
                        Thread.Sleep(1);
                        continue;
                    }

                    if (lastState != null && lastState.Length > 0) //not our first time here. //only true once.  
                    {
                        for (int i = 0; i < COMClient.SWITCHCOUNTBYTEARRAYSIZE; i++)
                        {
                            //Console.Write(newState[i]); //debug
                            //old XOR new = changed
                            changed[i] = (byte)(newState[i] ^ lastState[i]);
                            //changed & new = just pressed
                            justPressed[i] = (byte)(changed[i] & newState[i]);
                            //changed & old = just released
                            justReleased[i] = (byte)(changed[i] & lastState[i]);
                        }
                        //Console.WriteLine();
                        List<Switch> newStates = translatePinMask(newState);

                        if (_switchChangedHandler != null && newStates.Count > 0)
                            _switchChangedHandler(newStates);
                    }
                    lastState = newState;
                }
            }
            catch (Exception ex)
            {
                _log.Error("Error in switchStateTask.  Exiting and no longer listening to COM driver.", ex);
            }
        }

        private void connect()
        {
            if (_comClient == null)
            {
                _comClient = new COMClient(_port, BAUDRATE);
            }
            //if (_comClient.IsConnected) //why would we disconnect???
            //    _comClient.Disconnect();
            //else
            _comClient.Connect();
            if (_comClient.IsConnected)
            {
                _comClient.WriteMessage(string.Format(SolenoidDriverCommands.DebugMode, 0));
                _comClient.WriteMessage(string.Format(SolenoidDriverCommands.SwitchSpew, 1));
            }
        }

        private bool handleResponses()
        {
            byte[] response = null;
            if (!_comClient.Responses.TryDequeue(out response))
                return false;
            //updateGui(response); //TODO: do something with response.  Event?
            return true;
        }

        public void RotateStage(int position)
        {
            if (_comClient == null)
                return; //what do we do if the client doesn't pass a message?  queue it?
            _comClient.WriteMessage(string.Format(SolenoidDriverCommands.RotateMotor, (int)SolenoidArduino.Motors.CenterStage, position));
        }

        public void RotateCross(bool isClosed)
        {
            if (_comClient == null)
                return; //what do we do if the client doesn't pass a message?  queue it?
            _comClient.WriteMessage(string.Format(SolenoidDriverCommands.RotateMotor, (int)SolenoidArduino.Motors.Cross, isClosed ? 1 : 0));
        }

        public void EjectBall()
        {
            if (_comClient == null)
                return; //what do we do if the client doesn't pass a message?  queue it?
            _comClient.WriteMessage(string.Format(SolenoidDriverCommands.FireSolenoid, (int)SolenoidArduino.Solenoids.TroughEject));
        }

        public void AutoPlunge()
        {
            if (_comClient == null)
                return; //what do we do if the client doesn't pass a message?  queue it?
            throw new NotImplementedException();
        }

        public void EnableFlippers()
        {
            throw new NotImplementedException();
        }

        public void FireKnocker()
        {
            throw new NotImplementedException();
        }
    }
}
