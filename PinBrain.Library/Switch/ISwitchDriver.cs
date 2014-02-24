using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PinBrain.Library.Switch
{
    public delegate void SwitchEventHandler(object sender, int id);

    public interface ISwitchDriver
    {
        event SwitchEventHandler OnSwitchEvent;

        Switch[] SwitchArrayState { get; }

        void EjectBall();

        void AutoPlunge();

        void EnableFlippers();

        void FireKnocker();

        // Motor control.  Should this be elsewhere?
        void RotateStage(int position);

        void RotateCross(bool isClosed);
    }

    public enum SwitchState
    {
        Off = 0,
        On = 1
    }

    public struct Switch
    {
        private SwitchState _state;
        private int _id;
        private string _name;
        private SwitchState _lastState;
        private SwitchState _defaultState;

        public SwitchState State
        {
            get { return _state; }
            set
            {
                _lastState = _state;
                _state = value;
            }
        }
        public int Id { get { return _id; } }
        public string Name { get { return _name; } }
        public SwitchState LastState
        { get { return _lastState; } }
        public SwitchState DefaultValue
        {
            get { return _defaultState; }
            set { _defaultState = value; }
        }

        public Switch(int id, string name)
            : this(id, name, SwitchState.Off)
        {
        }

        public Switch(int id, string name, SwitchState defaultState)
        {
            _id = id;
            _name = name;
            _state = defaultState;
            _lastState = defaultState;
            _defaultState = defaultState;
        }
    }
}
