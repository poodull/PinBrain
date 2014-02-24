using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX.AudioVideoPlayback;
using log4net;

namespace PinBrain.Devices.Sound
{
    public class SoundDriverDx
    {
        protected static readonly new ILog _log = LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private string _soundRootFolder;

        SoundClip[] _sfxClips;
        SoundClip[] _musicClips;

        public SoundDriverDx(string soundRootFolder, string[] sfxFiles, string[] musicFiles)
        {
            _soundRootFolder = soundRootFolder;
            loadSoundFiles(sfxFiles, musicFiles);
        }

        public void RegisterCallback(Action<int> action)
        {
            SoundClip.SoundEndingCallback = action;
        }

        private void loadSoundFiles(string[] sfxFiles, string[] musicFiles)
        {
            try
            {
                _sfxClips = new SoundClip[sfxFiles.Length];
                for (int i = 0; i < sfxFiles.Length; i++)
                {
                    _sfxClips[i] = new SoundClip(i, _soundRootFolder + @"SFX\" + sfxFiles[i], false);
                }
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error intializing sound effects.  Check that sound files in {0}\\SFX are there.  {1}", _soundRootFolder, ex);
            }
            try
            {
                _musicClips = new SoundClip[musicFiles.Length];
                for (int i = 0; i < musicFiles.Length; i++)
                {
                    _musicClips[i] = new SoundClip(i, _soundRootFolder + @"MUSIC\" + musicFiles[i], true);
                }
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error intializing music effects.  Check that sound files in {0}\\Music are there.  {1}", _soundRootFolder, ex);
            }
        }

        public void PlaySfx(int index)
        {
            if (index > _sfxClips.Length)
            {
                _log.Warn("Asked to play sound clip outside of effects array length.  " + index);
                return;
            }
            _sfxClips[index].Play();
        }

        public void PlayMusic(int index, bool repeat)
        {
            if (index > _musicClips.Length)
            {
                _log.Warn("Asked to play sound clip outside of music array length.  " + index);
                return;
            }
            _musicClips[index].Play(repeat);
        }

        public void StopMusic(int index)
        {
            if (index > _sfxClips.Length)
            {
                _log.Warn("Asked to stop sound clip outside of music array length.  " + index);
                return;
            }
            _musicClips[index].Stop();
        }

        public void StopAllMusic()
        {
            for (int i = 0; i < _musicClips.Length; i++)
            {
                _musicClips[i].Stop();
            }
        }
    }
}
