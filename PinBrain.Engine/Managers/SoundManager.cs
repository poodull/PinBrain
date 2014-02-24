using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using PinBrain.Devices.Sound;
using PinBrain.Engine.Constants;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;
using System.Configuration;

namespace PinBrain.Engine.Managers
{
    public class SoundManager
    {
        protected static readonly new ILog _log = LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static SoundManager _instance;

        private SoundDriverDx _soundDriver;
        private struct MusicPlayListItem
        {
            public SoundConstants.Music Music;
            public bool Repeat;
            public MusicPlayListItem(SoundConstants.Music music, bool repeat)
            { Music = music; Repeat = repeat; }
        }

        private ConcurrentQueue<MusicPlayListItem> _musicQueue;

        private SoundManager()
        {
            _log.Info("Initiating SoundManager...");
            ////this is where we load the Sound controller for effects and music

            //TODO: Load from file
            string[] CLIP = { @"Teleport.wav", @"dra_die.wav", @"elevtrbl.wav", "pickupbg.wav" };
            string[] MUSIC = { @"StageIntro.wav", @"StalkerRough1.wav", @"hod_clocktower.mid", @"CV2DayTrial6.wav", "CV1VKTest1.Looped.wav"};

            _soundDriver = new SoundDriverDx(ConfigurationSettings.AppSettings["soundRootFolder"], CLIP, MUSIC);
            _musicQueue = new ConcurrentQueue<MusicPlayListItem>();

            var t = Task.Factory.StartNew(() => monitorMusicQueue(), TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// Singleton pattern
        /// </summary>
        protected static SoundManager getInstance()
        {
            if (_instance == null)
                _instance = new SoundManager();

            return _instance;
        }

        #region # Music Queue Monitor Logic #
        bool _isMusicPlaying = false;
        private void monitorMusicQueue()
        {
            try
            {
                while (true)
                {
                    MusicPlayListItem next;
                    if (!_isMusicPlaying && _musicQueue.TryDequeue(out next))
                    {
                        _log.Debug("Sound Manager now playing : " + next.Music.ToString());
                        _isMusicPlaying = true;
                        _soundDriver.PlayMusic((int)next.Music, next.Repeat);
                    }
                    else
                    {
                        //debug
                    }
                    Thread.Sleep(1);
                }
            }
            catch (Exception ex)
            {
                _log.Error("Error in sound queue manager task.  No more music!", ex);
            }
        }

        public void SoundEndingHandler(int soundId)
        {
            //This lets the music queue monitor thread know to move to the next queued music
            _isMusicPlaying = false;
        }

        private void registerCallback()
        {
            getInstance()._soundDriver.RegisterCallback(new Action<int>(SoundEndingHandler));
        }
        #endregion

        internal static void PlaySfx(SoundConstants.Sfx sound)
        {
            getInstance().playSfx(sound);
        }

        internal static void PlayMusic(SoundConstants.Music music, bool repeat)
        {
            getInstance().playMusic(music, repeat);
        }

        /// <summary>
        /// Stops all sound and clears any from the queue.  Dead stop.
        /// </summary>
        internal static void Reset()
        {
            getInstance()._musicQueue = new ConcurrentQueue<MusicPlayListItem>();
            getInstance()._soundDriver.StopAllMusic();
        }

        internal static void Init()
        {
            getInstance().registerCallback();
        }

        private void playSfx(SoundConstants.Sfx sound)
        {
            _soundDriver.PlaySfx((int)sound);
        }

        private void playMusic(SoundConstants.Music music, bool repeat)
        {
            _log.Debug("Sound Manager Enqueueing Music: " + music.ToString());
            _musicQueue.Enqueue(new MusicPlayListItem(music, repeat));
        }

        internal static void StopAllMusic()
        {
            getInstance()._soundDriver.StopAllMusic();
            getInstance()._isMusicPlaying = false;
        }
    }

}
