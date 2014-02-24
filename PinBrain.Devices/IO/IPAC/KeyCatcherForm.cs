using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using log4net;

using PinBrain.Library.Switch;

namespace PinBrain.Devices.IO.IPAC
{
    public partial class KeyCatcherForm : Form, ISwitchDriver
    {
        protected static readonly new ILog _log = LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        Switch[] _state;

        public KeyCatcherForm(int size)
        {
            InitializeComponent();
            _state = new Switch[size];
        }

        private void KeyCatcherForm_KeyDown(object sender, KeyEventArgs e)
        {
            _log.DebugFormat("key {0} Down.", e.KeyValue);
        }

        private void KeyCatcherForm_KeyUp(object sender, KeyEventArgs e)
        {
            _log.DebugFormat("key {0} Up.", e.KeyValue);
        }

        private void KeyCatcherForm_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        #region ISwitchDriver Members

        public event SwitchEventHandler OnSwitchEvent;

        public Switch[] SwitchArrayState
        {
            get { return _state; }
        }

        public void RotateStage(int position) { }
        public void RotateCross(bool isClosed) { }
        public void EjectBall() { }

        public void AutoPlunge() { }
        public void EnableFlippers() { }
        public void FireKnocker() { }

        #endregion
    }
}
