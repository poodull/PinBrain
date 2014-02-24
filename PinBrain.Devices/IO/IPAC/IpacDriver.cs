using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using log4net;

using PinBrain.Library.Switch;

namespace PinBrain.Devices.IO.IPAC
{
    public class IpacDriver : ISwitchDriver
    {
        protected static readonly new ILog _log = LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private KeySniffer _keysnif;
        Switch[] _state;

        public IpacDriver(Switch[] state)
        {
            _keysnif = new KeySniffer();
            _keysnif.KeyDown += new KeySniffer.KeyPress(_keysnif_KeyDown);
            _keysnif.KeyUp += new KeySniffer.KeyPress(_keysnif_KeyUp);
            _state = state;
        }

        void _keysnif_KeyUp(Keys key)
        {
            _log.DebugFormat("Key Up: {0}", key);
            getKeyIndex(key, SwitchState.Off);
            if (OnSwitchEvent != null)
                OnSwitchEvent(this, (int)key);
        }

        void _keysnif_KeyDown(Keys key)
        {
            _log.DebugFormat("Key Down: {0}", key);
            getKeyIndex(key, SwitchState.On);
            if (OnSwitchEvent != null)
                OnSwitchEvent(this, (int)key);
        }

        #region ISwitchDriver Members

        public event SwitchEventHandler OnSwitchEvent;

        public Switch[] SwitchArrayState
        {
            get { return _state; }
        }

        public void RotateStage(int position) { }
        public void RotateCross(bool isClosed) { }
        #endregion

        void getKeyIndex(Keys key, SwitchState value)
        {
            switch (key)
            {
                case Keys.D5: //Coin1
                    _state[0].State = value;
                    break;
                case Keys.D6://Coin2
                    _state[1].State = value;
                    break;
                case Keys.D1://Start1
                    _state[2].State = value;
                    break;
                case Keys.D2://Start2
                    _state[3].State = value;
                    break;
                case Keys.Right://Right
                    _state[4].State = value;
                    break;
                case Keys.Left://Left
                    _state[5].State = value;
                    break;
                case Keys.Up://Up
                    _state[6].State = value;
                    break;
                case Keys.Down://Down
                    _state[7].State = value;
                    break;
                case Keys.Control://LeftControl
                    _state[8].State = value;
                    break;
                case Keys.Alt://1SW2
                    _state[9].State = value;
                    break;
                case Keys.Space://Space
                    _state[10].State = value;
                    break;
                case Keys.ShiftKey://LeftShift
                    _state[11].State = value;
                    break;
                case Keys.Z://1SW5
                    _state[12].State = value;
                    break;
                case Keys.X://1SW6
                    _state[13].State = value;
                    break;
                case Keys.C://1SW7
                    _state[14].State = value;
                    break;
                case Keys.V://1SW8
                    _state[15].State = value;
                    break;
                case Keys.P://1A
                    _state[16].State = value;
                    break;
                case Keys.Enter://1B
                    _state[17].State = value;
                    break;
                case Keys.G://2Right 
                    _state[18].State = value;
                    break;
                case Keys.D://2Left
                    _state[19].State = value;
                    break;
                case Keys.R://2Up
                    _state[20].State = value;
                    break;
                case Keys.F://2Down
                    _state[21].State = value;
                    break;
                case Keys.A://2SW1
                    _state[22].State = value;
                    break;
                case Keys.S://2SW2
                    _state[23].State = value;
                    break;
                case Keys.Q://2SW3
                    _state[24].State = value;
                    break;
                case Keys.W://2SW4
                    _state[25].State = value;
                    break;
                case Keys.I://2SW5
                    _state[26].State = value;
                    break;
                case Keys.K://2SW6
                    _state[27].State = value;
                    break;
                case Keys.L://2SW8
                    _state[28].State = value;
                    break;
                case Keys.Tab://2B
                    _state[29].State = value;
                    break;
                case Keys.Escape://2B
                    _state[30].State = value;
                    break;
                case Keys.Add:
                case Keys.Apps:
                case Keys.Attn:
                case Keys.B:
                case Keys.Back:
                case Keys.BrowserBack:
                case Keys.BrowserFavorites:
                case Keys.BrowserForward:
                case Keys.BrowserHome:
                case Keys.BrowserRefresh:
                case Keys.BrowserSearch:
                case Keys.BrowserStop:
                case Keys.Cancel:
                case Keys.Capital:
                //case Keys.CapsLock:
                case Keys.Clear:
                case Keys.ControlKey:
                case Keys.Crsel:
                case Keys.D0:
                case Keys.D3:
                case Keys.D4:
                case Keys.D7:
                case Keys.D8:
                case Keys.D9:
                case Keys.Decimal:
                case Keys.Delete:
                case Keys.Divide:
                case Keys.E:
                case Keys.End:
                case Keys.EraseEof:
                case Keys.Execute:
                case Keys.Exsel:
                case Keys.F1:
                case Keys.F10:
                case Keys.F11:
                case Keys.F12:
                case Keys.F13:
                case Keys.F14:
                case Keys.F15:
                case Keys.F16:
                case Keys.F17:
                case Keys.F18:
                case Keys.F19:
                case Keys.F2:
                case Keys.F20:
                case Keys.F21:
                case Keys.F22:
                case Keys.F23:
                case Keys.F24:
                case Keys.F3:
                case Keys.F4:
                case Keys.F5:
                case Keys.F6:
                case Keys.F7:
                case Keys.F8:
                case Keys.F9:
                case Keys.FinalMode:
                case Keys.H:
                case Keys.HanguelMode:
                //case Keys.HangulMode:
                case Keys.HanjaMode:
                case Keys.Help:
                case Keys.Home:
                    break;
                case Keys.IMEAccept:
                //case Keys.IMEAceept:
                case Keys.IMEConvert:
                case Keys.IMEModeChange:
                case Keys.IMENonconvert:
                case Keys.Insert:
                case Keys.J:
                case Keys.JunjaMode:
                    break;
                //case Keys.KanaMode:
                //case Keys.KanjiMode:
                case Keys.KeyCode:
                    break;
                case Keys.LButton:
                case Keys.LControlKey:
                case Keys.LMenu:
                case Keys.LShiftKey:
                case Keys.LWin:
                case Keys.LaunchApplication1:
                case Keys.LaunchApplication2:
                case Keys.LaunchMail:
                    break;
                case Keys.LineFeed:
                case Keys.M:
                case Keys.MButton:
                case Keys.MediaNextTrack:
                case Keys.MediaPlayPause:
                case Keys.MediaPreviousTrack:
                case Keys.MediaStop:
                case Keys.Menu:
                case Keys.Modifiers:
                case Keys.Multiply:
                case Keys.N:
                case Keys.Next:
                case Keys.NoName:
                case Keys.None:
                case Keys.NumLock:
                case Keys.NumPad0:
                case Keys.NumPad1:
                case Keys.NumPad3:
                case Keys.NumPad4:
                case Keys.NumPad5:
                case Keys.NumPad6:
                case Keys.NumPad7:
                case Keys.NumPad8:
                case Keys.NumPad9:
                case Keys.O:
                case Keys.Oem1:
                case Keys.Oem102:
                case Keys.Oem2:
                case Keys.Oem3:
                case Keys.Oem4:
                case Keys.Oem5:
                case Keys.Oem6:
                case Keys.Oem7:
                case Keys.Oem8:
                //case Keys.OemBackslash:
                case Keys.OemClear:
                //case Keys.OemCloseBrackets:
                case Keys.OemMinus:
                //case Keys.OemOpenBrackets:
                case Keys.OemPeriod:
                //case Keys.OemPipe:
                //case Keys.OemQuestion:
                //case Keys.OemQuotes:
                //case Keys.OemSemicolon:
                case Keys.Oemcomma:
                case Keys.Oemplus:
                    //case Keys.Oemtilde:
                    break;
                case Keys.Pa1:
                case Keys.Packet:
                //case Keys.PageDown:
                case Keys.PageUp:
                case Keys.Pause:
                case Keys.Play:
                case Keys.Print:
                case Keys.PrintScreen:
                //case Keys.Prior:
                case Keys.ProcessKey:
                    break;
                case Keys.RButton:
                case Keys.RControlKey:
                case Keys.RMenu:
                case Keys.RShiftKey:
                case Keys.RWin:
                //case Keys.Return:
                case Keys.Scroll:
                case Keys.Select:
                case Keys.SelectMedia:
                case Keys.Separator:
                case Keys.Shift:
                case Keys.Sleep:
                //case Keys.Snapshot:
                case Keys.Subtract:
                case Keys.T:
                    break;
                case Keys.U:
                    break;
                case Keys.VolumeDown:
                case Keys.VolumeMute:
                case Keys.VolumeUp:
                    break;
                case Keys.XButton1:
                case Keys.XButton2:
                case Keys.Y:
                    break;
                case Keys.Zoom:
                default:
                    break;
            }
        }


        public void EjectBall()
        {
            throw new NotImplementedException();
        }

        public void AutoPlunge()
        {
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
