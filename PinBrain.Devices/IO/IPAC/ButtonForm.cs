using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PinBrain.Library.Switch;
using log4net;

namespace PinBrain.Devices.IO.IPAC
{
    public partial class ButtonForm : Form, ISwitchDriver
    {
        protected static readonly new ILog _log = LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        volatile private Switch[] _state;

        private Action<List<Switch>> _switchChangedHandler;

        public ButtonForm(Switch[] state, Action<List<Switch>> switchChangedHandler)
        {
            InitializeComponent();
            _state = state;
            _switchChangedHandler = switchChangedHandler;

            loadSwitches(_state);

            listView1.ItemCheck += new ItemCheckEventHandler(listView1_ItemCheck);
            listView2.ItemCheck += new ItemCheckEventHandler(listView1_ItemCheck);
            listView1.MouseDoubleClick += new MouseEventHandler(listView1_MouseDoubleClick);
            listView2.MouseDoubleClick += new MouseEventHandler(listView1_MouseDoubleClick);

            this.Show();
        }

        void listView1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            try
            {
                _state[(int)((ListView)sender).Items[e.Index].Tag].State = e.NewValue == CheckState.Checked ? SwitchState.On : SwitchState.Off;
                if (_switchChangedHandler != null)
                {
                    List<Switch> changed = new List<Switch>();
                    changed.Add(_state[(int)((ListView)sender).Items[e.Index].Tag]);
                    _switchChangedHandler(changed);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }

            //if (_state[e.Index].State != _state[e.Index].LastState && OnSwitchEvent != null)
            //    OnSwitchEvent(this, e.Index);
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo info = ((ListView)sender).HitTest(e.X, e.Y);
            if (info.Item != null)
            {
                info.Item.Checked = !info.Item.Checked;
            }
        }

        private void loadSwitches(Switch[] _state)
        {
            int half = _state.Length / 2;
            for (int i = 0; i < half; i++)
            {
                Switch sw = _state[i];
                ListViewItem itm = listView1.Items.Add(sw.Id.ToString());
                itm.SubItems.Add(sw.Name);
                itm.SubItems.Add(sw.State.ToString());
                itm.Tag = i;
            }

            for (int i = half; i < _state.Length; i++)
            {
                Switch sw = _state[i];
                ListViewItem itm = listView2.Items.Add(sw.Id.ToString());
                itm.SubItems.Add(sw.Name);
                itm.SubItems.Add(sw.State.ToString());
                itm.Tag = i;
            }
        }

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
    }
}
