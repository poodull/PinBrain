using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using log4net;

using PinBrain.Library.Lighting;
using PinBrain.Devices.IO.LedWiz;
using PinBrain.Engine.Constants;

namespace PinBrain.Engine.Managers
{
    public class LightManager //: ILightSequenceManager
    {
        protected static readonly new ILog _log = LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        Dictionary<string, ILightSequence> _sequences;

        private static LightManager _instance;

        private LightManager()
        {
            _log.Info("Initializing LightManager...");
            _sequences = new Dictionary<string, ILightSequence>();
        }

        /// <summary>
        /// Singleton pattern
        /// </summary>
        /// <returns></returns>
        protected static LightManager getInstance()
        {
            if (_instance == null)
                _instance = new LightManager();

            return _instance;
        }

        #region ILightSequenceManager Members

        /// <summary>
        /// Add a sequence and manage it's on/off status.
        /// </summary>
        /// <param name="sequence"></param>
        public static void AddSequence(ILightSequence sequence)
        {
            if (!getInstance()._sequences.ContainsKey(sequence.SequenceName))
                getInstance()._sequences.Add(sequence.SequenceName, sequence);
        }

        /// <summary>
        /// Play a sequence immediately.
        /// </summary>
        /// <param name="sequenceName"></param>
        /// <param name="repeat"></param>
        public static void PlaySequence(LightingConstants.Lfx sequenceName)
        {
            getInstance().playSequence(sequenceName);
        }

        public static void StopSequence(string sequenceName)
        {
            //what does this mean?
            ////_log.DebugFormat("Trying to Stop Light Sequence {0}.", sequenceName);
        }

        public static void StopAll()
        {
            //what does this mean?
            ////_log.Debug("Trying to Stop ALL Light Sequences.");
        }

        private void playSequence(LightingConstants.Lfx sequenceName)
        {
            //if (!getInstance()._sequences.ContainsKey(sequence.SequenceName))
            _log.DebugFormat("Playing Light Sequence {0}", sequenceName);
        }

        #endregion
    }
}
