using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX.AudioVideoPlayback;
using System.IO;
using log4net;
using System.Windows.Forms;

namespace PinBrain.Devices.Sound
{
    /// <summary>
    /// This is an individual sound clip
    /// </summary>
    public class SoundClip
    {
        protected static readonly new ILog _log = LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static Action<int> SoundEndingCallback;

        private Audio[] _clips;
        private int _clipIndex = 0;
        private int _id;
        private bool _repeat;
        /// <summary>
        /// Fire the callback when the sound is completed
        /// </summary>
        private bool _doCallback;

        public SoundClip(int id, string filenameArray, bool doCallback)
        {
            _id = id;
            string[] clipsNames = filenameArray.Split(',');
            _clips = new Audio[clipsNames.Length];
            for (int i = 0; i < _clips.Length; i++)
            {
                if (File.Exists(clipsNames[i]))
                {
                    _clips[i] = Audio.FromFile(clipsNames[i]);
                    _clips[i].Ending += new EventHandler(SoundClip_Ending); //doesn't work on background thread
                    //_clips[i].Stopping += new EventHandler(SoundClip_Stopping);
                    _log.InfoFormat("Sound {0} is {1}s long.", clipsNames[i], _clips[i].Duration);
                }
                else
                {
                    _log.ErrorFormat("Sound Clip file {0} does not exist!");
                    Console.Beep(); //NOT AVAILABLE
                }
            }
            _doCallback = doCallback;
        }

        void SoundClip_Stopping(object sender, EventArgs e)//doesn't work on background thread
        {
            _log.WarnFormat("Sound Clip file {0} Stopping!", sender);
        }

        public void SoundClip_Ending(object sender, EventArgs e)//doesn't work on background thread
        {
            _log.WarnFormat("Sound Clip file {0} Ending!", _id);
            if (_repeat)
                Play(_repeat);
            else
                if (_doCallback && SoundEndingCallback != null)
                    SoundEndingCallback(_id);
        }

        //public SoundClip(int id, string filename, int count, Action<int> _soundEndingCallback)
        //{
        //    //File = filename;
        //    _clips = new Audio[count];
        //    for (int i = 0; i < _clips.Length; i++)
        //    {
        //        _clips[i] = Audio.FromFile(filename);
        //    }
        //}

        //public SoundClip(int id, params string[] filename, Action<int> _soundEndingCallback)
        //{
        //    _clips = new Audio[filename.Length];
        //    for (int i = 0; i < _clips.Length; i++)
        //    {
        //        _clips[i] = Audio.FromFile(filename[i]);
        //    }
        //}

        public void Play(bool repeat = false)
        {
            _repeat = repeat;
            if (_clipIndex > _clips.Length - 1) _clipIndex = 0;
            int i = _clipIndex++;
            if (_clips[i] == null)
            {
                Console.Beep(); //NOT AVAILABLE
                return;
            }
            _clips[i].Stop();
            _clips[i].Play();
        }

        public void Stop()
        {
            _repeat = false;
            for (int i = 0; i < _clips.Length; i++)
            {
                if (_clips[i] != null)
                {
                    _clips[i].Stop();
                }
            }
        }
    }
}
