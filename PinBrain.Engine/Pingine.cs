using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using log4net;

//using PinBrain.Library.BookKeeping;
using PinBrain.Library.Lighting;
//using PinBrain.Library.Test;

using PinBrain.Engine.Lighting;
using PinBrain.Engine.Managers;
using PinBrain.Engine.Constants;

namespace PinBrain.Engine
{
    public class Pingine
    {
        protected static readonly new ILog _log = LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //private ITestManager;
        //private ICreditManager;
        //MotorManager
        //DisplayManager
        //GameManager

        private bool _isRunning = true;

        /// <summary>
        /// Determines if the engine is running.  False will drop everything as is.
        /// </summary>
        public bool IsRunning
        {
            get { return _isRunning; }
            set { _isRunning = value; }
        }

        public Pingine()
        {
            //todo load configuration
            //initialize hardware
            initLights();
            initDisplay();
            initGameManager();
            initSwitchManager();
            initSoundManager();

            //DEBUG: turn the GI lights on
            LightManager.PlaySequence(LightingConstants.Lfx.AllOff); //DEBUG
            LightManager.PlaySequence(LightingConstants.Lfx.AllOn); //DEBUG
            //DisplayManager.PlaySequence(DisplayConstants.ATTRACT); //DisplayConstants.TEST
            GameManager.ModeStart(GameManagerConstants.GameModes.ATTRACT);
            LightManager.PlaySequence(LightingConstants.Lfx.Attract); //DEBUG
            //GameManager.ModeStart(GameManagerConstants.TEST);
            _log.Debug("Done initialization.");
        }

        private void initDisplay()
        {
            DisplayManager.Reset();
        }

        private void initGameManager()
        {
            // reset everything.
            GameManager.Reset();
        }

        private void initSwitchManager()
        {
            SwitchManager.Reset();
        }

        private void initSoundManager()
        {
            SoundManager.Init();
        }

        private void initLights()
        {
            //add the GI Lights
            GIString1AllWhite giLights =
                new GIString1AllWhite(LightingConstants.GILIGHTSTRING);
            _log.DebugFormat("Adding Light Sequence {0}:{1}",
                LightingConstants.GILIGHTSTRING, giLights);
            LightManager.AddSequence(giLights);
        }

        public void SystemCalibrate()
        {
        }

        public void PlayfieldCalibrate() { }

        /// <summary>
        /// This method is called to initialize the playfield for play.
        /// Scores, credits, etc are reset.  Occurs after SystemCalibrate
        /// and PlayfieldCalibrate
        /// </summary>
        public void InitializePlayfield() { }
        /// <summary>
        /// A music channel has finished playing.
        /// </summary>
        public void MusicFinished(int channel) { }

        /// <summary>
        /// Load a new ball on the playfield.
        /// </summary>
        public void CreateNewBall() { }

        /// <summary>
        /// A switch has been triggered.
        /// </summary>
        /// <param name="switchId"></param>
        public void SwitchHit(int switchId) { }
    }
}
