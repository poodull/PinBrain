//#define DOTIMING

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO.Ports;
using System.Threading;
using log4net;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Concurrent;


namespace PinBrain.Devices.IO.Serial
{
    public class COMClient
    {
        /// <summary>
        /// The log4net logger.
        /// </summary>
        protected static readonly ILog _log = LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// Serial port that is used to communicate.
        /// </summary>
        private SerialPort _serialPort;
        /// <summary>
        /// Whether or not we are in the process of trying to reconnect or not.
        /// </summary>
        private bool _isReconnecting = false;

        protected ConcurrentQueue<byte[]> _responses = new ConcurrentQueue<byte[]>();

        /// <summary>
        /// The amount of time to wait for data from an open COM port before logging the timeout.
        /// </summary>
        public const int TIMEOUTWARNINGMS = 5000;

        /// <summary>
        /// The size of the switchmask array.  This is the total number of switches / 7 + remainder.
        /// NOTE:  This is not /8 since a byte = 0 is not an indication of any switch state
        /// </summary>
        public const int SWITCHCOUNTBYTEARRAYSIZE = 8;
        private byte[] _switchMask = new byte[SWITCHCOUNTBYTEARRAYSIZE];
        private volatile bool _isDirty;

        private enum TransmitHeader
        {
            Unknown = 0,
            SwitchState = 1,
            Response = 2
        }

        public COMClient(string port, int baud)
        {
            // Set up the serial port
            _serialPort = new SerialPort(port, baud, Parity.None, 8, StopBits.One);
        }

        #region * Public properties *
        /// <summary>
        /// Whether or not the COM port is connected to our Fez or not.
        /// </summary>
        public bool IsConnected
        {
            get { return _serialPort != null && _serialPort.IsOpen; }
        }

        /// <summary>
        /// Gets the state of the byte array switch mask.  Returns byte[0] if no update since last fetch.
        /// </summary>
        public byte[] SwitchMask
        {
            get
            {
                if (!_isDirty)
                {
                    return new byte[0]; //no data available
                }
                byte[] ret = new byte[SWITCHCOUNTBYTEARRAYSIZE];
                //_switchMask.CopyTo(ret, 0);
                Buffer.BlockCopy(_switchMask, 0, ret, 0, SWITCHCOUNTBYTEARRAYSIZE);
                _isDirty = false; //flip the dirty flag so we only get this array once
                return ret;
            }
        }

        public ConcurrentQueue<byte[]> Responses
        {
            get { return _responses; }
        }
        #endregion

        #region * Connect/Disconnect Methods *
        /// <summary>
        /// Connect the COM Port.
        /// </summary>
        public void Connect()
        {
            if (_serialPort == null)
                return;
            try
            {
                if (!_serialPort.IsOpen)
                {
                    _log.Debug("Attempting to open Serial Port.");
                    _serialPort.Open();
                    _log.Debug("Successfully opened Serial Port.");
                    var t = Task.Factory.StartNew(() => readSocketInBytes());
                }
                else
                {
                    reconnect();
                }
                _serialPort.ErrorReceived += new SerialErrorReceivedEventHandler(_serialPort_ErrorReceived);
                _log.Info("Found COM Port " + _serialPort.PortName);
            }
            catch (System.IO.IOException ioEx)
            {
                _log.Error("IO Exception while trying to connect to COM Port. Port: " + _serialPort.PortName + " Type: " + ioEx.GetType().Name);
                reconnect();
            }
            catch (Exception ex)
            {
                _log.Error("Error connecting to COM Port attempting reconnect. Port: " + _serialPort.PortName, ex);
                reconnect();
            }
        }

        private void reconnect()
        {
            if (_serialPort == null)
                return;
            try
            {
                Disconnect();
                if (!_serialPort.IsOpen)
                    _serialPort.Open();
            }
            catch (System.IO.IOException ioEx)
            {
                _log.Error("IO Exception while trying to reconnect to COM Port. Port: " + _serialPort.PortName + " Type: " + ioEx.GetType().Name, ioEx);
            }
            catch (Exception ex)
            {
                _log.Error("Error reconnecting to COM Port " + _serialPort.PortName, ex);
            }

        }

        private void reconnectAsync()
        {
            if (!_isReconnecting)
            {
                _isReconnecting = true;
                if (!System.Threading.ThreadPool.QueueUserWorkItem(new WaitCallback(connectCallback)))
                    _log.Warn("Unable to queue Threadpool for method 'connectCallback()");
            }
        }

        private void connectCallback(object stateinfo)
        {
            _log.Debug("Attempting ReconnectAsync to USB COM Device");
            reconnect();
            _isReconnecting = false;
        }

        /// <summary>
        /// Disconnect the COM Port.
        /// </summary>
        public void Disconnect()
        {
            try
            {
                if (_serialPort.IsOpen)
                    _serialPort.Close();
                _log.Debug("Disconnected from COM Port " + _serialPort.PortName);
            }
            catch (System.IO.IOException ioEx)
            {
                _log.Error("IO Exception while trying to disconnect to COM Port. Port: " + _serialPort.PortName + " Type: " + ioEx.GetType().Name, ioEx);
            }
            catch (Exception ex)
            {
                _log.Error("Error disconnecting from COM Port " + _serialPort.PortName, ex);
            }
        }
        #endregion

        #region * Serial Port Data *
        private void _serialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            _log.Error("Error received from COM Port " + _serialPort.PortName + " Ex:" + e.EventType.ToString());
        }

        /// <summary>
        /// this takes about .55ms per read
        /// </summary>
        private void readSocketInBytes()
        {
#if DOTIMING
            double totalRunCount = 1; //DEBUG
            double avgRun = 0; //DEBUG
            double totalRun = 0; //DEBUG
#endif

            Stopwatch sw = new Stopwatch();
            int count = 0; //used to place bytes in array
            byte[] responseBuffer = new byte[254];

            try
            {
                bool discard = false;
                TransmitHeader headerType = TransmitHeader.Unknown;
                int discardCount = 0;
                while (_serialPort.IsOpen)
                {
                    int b;

                    while (_serialPort.BytesToRead == 0)
                    {
                        if (sw.ElapsedMilliseconds > TIMEOUTWARNINGMS)
                        {
                            _log.WarnFormat("Waiting for data from COM PORT {0} for {1}ms.", _serialPort.PortName, sw.ElapsedMilliseconds);
                            sw.Restart();
                        }
                        //todo: log if there's been a long timeout
                        Thread.SpinWait(1);
                    }
                    sw.Restart();

                    b = _serialPort.ReadByte();
                    //Console.WriteLine(b); //DEBUG
                    switch (b)
                    {
                        case -1:
                            //connection is closed.
                            return;
                        case 36: //"$" switch status start
                            if (headerType == TransmitHeader.Unknown)
                            {
                                headerType = TransmitHeader.SwitchState;
#if DOTIMING
                                sw.Start();
#endif
                                count = 0;
                                discard = false; //header hit, now we can collect data
                            }
                            else
                            {
                                goto default;
                            }
                            break;
                        case 35: //"#" response start
                            if (headerType == TransmitHeader.Unknown)
                            {
                                headerType = TransmitHeader.Response;
#if DOTIMING
                                sw.Start();
#endif
                                count = 0;
                                discard = false; //header hit, now we can collect data
                            }
                            else
                            {
                                goto default;
                            }
                            break;
                        case 46: //"." period
                            if (discard)
                            {
                                discard = false;
                                _log.Warn("Only read " + count + " ");
                            }
                            else
                            {
#if DOTIMING
                                totalRun += sw.ElapsedMilliseconds; //DEBUG
                                avgRun = totalRun / totalRunCount; //DEBUG
                                totalRunCount++; //DEBUG
                                Console.WriteLine("{0:0.000}", avgRun); //DEBUG
#endif
                                if (headerType == TransmitHeader.Response)
                                {
                                    byte[] response = new byte[count];
                                    //Array.Copy(responseBuffer, response, count);
                                    Buffer.BlockCopy(responseBuffer, 0, response, 0, count);
                                    _responses.Enqueue(response);
                                }
                                else if (headerType == TransmitHeader.SwitchState)
                                {
                                    //_switchMask[count] = b; This is not a switch state, so we don't need to populate the array
                                    _isDirty = true;
                                }
                            }
                            headerType = TransmitHeader.Unknown;
#if DOTIMING
                            sw.Reset();
#endif
                            discard = true; //need to hit a header next
                            break;
                        default:
                            if (headerType == TransmitHeader.Response)
                            {
                                responseBuffer[count] = (byte)b;
                            }
                            else if (headerType == TransmitHeader.SwitchState)
                            {
                                if (count < 0 || count > 7)
                                {
                                    discard = true; //TOO MUCH DATA! trash this until we get a new header
                                    discardCount++;
                                    _log.Warn("WTF? " + count + " was " + b);
                                }
                                else
                                    _switchMask[count] = (byte)b;
                            }
                            count++;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error("Error in readSocketInBytes: ", ex);
            }
        }

        /// <summary>
        /// Takes a byte and returns an array[8] of bits (bools)
        ///_switchStates = switchBytes.SelectMany(getBits).ToArray();
        /// </summary>
        IEnumerable<bool> getBits(byte b)
        {
            for (int i = 0; i < 8; i++)
            {
                yield return (b & 0x80) != 0;
                b *= 2;
            }
        }

        /// <summary>
        /// Write a message across the Serial port.
        /// </summary>
        public bool WriteMessage(string message)
        {
            if (_serialPort.IsOpen)
            {
                try
                {
                    _log.Debug("Writing message to the serial port. Message: " + message);
                    // Write the message
                    _serialPort.Write(message);
                    _log.Debug("Successfully wrote message " + message + " to serial port");
                    return true;
                }
                catch (System.IO.IOException ioEx)
                {
                    _log.ErrorFormat("IO Exception while trying to  write message to COM Port. Port: {0} Message {1} Type {2}.  {3}",
                         _serialPort.PortName, message, ioEx.GetType().Name, ioEx);
                    reconnect();
                    return false;
                }
                catch (Exception ex)
                {
                    _log.Error("Error writing to COM Port", ex);
                    reconnect();
                    return false;
                }
            }
            return false;
        }
        #endregion
    }
}
