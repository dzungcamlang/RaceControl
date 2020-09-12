using Mpv.NET.Player;
using Prism.Mvvm;
using RaceControl.Common.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace RaceControl.Mpv
{
    public class MpvMediaPlayer : BindableBase, IMediaPlayer
    {
        private readonly MpvPlayer _mpvPlayer;

        private long _time;
        private long _duration;
        private int _volume;
        private bool _isPaused;
        private bool _isMuted;
        private bool _isScanning;
        private bool _isCasting;
        private ObservableCollection<IMediaTrack> _audioTracks;
        private ObservableCollection<IMediaRenderer> _mediaRenderers;
        private bool _disposed;

        public MpvMediaPlayer(MpvPlayer mpvPlayer)
        {
            _mpvPlayer = mpvPlayer;
            _mpvPlayer.PositionChanged += MpvPlayer_PositionChanged;
            _mpvPlayer.MediaResumed += MpvPlayer_MediaResumed;
            _mpvPlayer.MediaPaused += MpvPlayer_MediaPaused;
        }

        public long Time
        {
            get => _time;
            set
            {
                if (SetProperty(ref _time, Math.Max(value, 0)))
                {
                    _mpvPlayer.Position = TimeSpan.FromMilliseconds(_time);
                }
            }
        }

        public long Duration
        {
            get => _duration;
            set => SetProperty(ref _duration, value);
        }

        public int Volume
        {
            get => _volume;
            set
            {
                if (SetProperty(ref _volume, Math.Min(Math.Max(value, 0), 100)))
                {
                    _mpvPlayer.Volume = _volume;
                }
            }
        }

        public bool IsPaused
        {
            get => _isPaused;
            set => SetProperty(ref _isPaused, value);
        }

        public bool IsMuted
        {
            get => _isMuted;
            set => SetProperty(ref _isMuted, value);
        }

        public bool IsScanning
        {
            get => _isScanning;
            set => SetProperty(ref _isScanning, value);
        }

        public bool IsCasting
        {
            get => _isCasting;
            set => SetProperty(ref _isCasting, value);
        }

        public ObservableCollection<IMediaTrack> AudioTracks
        {
            get => _audioTracks ??= new ObservableCollection<IMediaTrack>();
            set => SetProperty(ref _audioTracks, value);
        }

        public ObservableCollection<IMediaRenderer> MediaRenderers
        {
            get => _mediaRenderers ??= new ObservableCollection<IMediaRenderer>();
            set => SetProperty(ref _mediaRenderers, value);
        }

        public async Task StartPlaybackAsync(string streamUrl, IMediaRenderer mediaRenderer = null)
        {
            _mpvPlayer.Load(streamUrl);
            _mpvPlayer.Resume();
        }

        public void TogglePause()
        {
            if (!IsPaused)
            {
                _mpvPlayer.Pause();
            }
            else
            {
                _mpvPlayer.Resume();
            }
        }

        public void ToggleMute()
        {
            if (Volume > 0)
            {
                Volume = 0;
                IsMuted = true;
            }
            else
            {
                Volume = 100;
                IsMuted = false;
            }
        }

        public void SetAudioTrack(IMediaTrack audioTrack)
        {
            // todo
        }

        public async Task ScanChromecastAsync()
        {
            // todo
        }

        public async Task ChangeRendererAsync(IMediaRenderer mediaRenderer, string streamUrl)
        {
            // todo
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _mpvPlayer.Stop();
                _mpvPlayer.Dispose();
            }

            _disposed = true;
        }

        private void MpvPlayer_PositionChanged(object sender, MpvPlayerPositionChangedEventArgs args)
        {
            SetProperty(ref _time, Convert.ToInt32(args.NewPosition.TotalMilliseconds), nameof(Time));
        }

        private void MpvPlayer_MediaResumed(object sender, EventArgs args)
        {
            IsPaused = false;
        }

        private void MpvPlayer_MediaPaused(object sender, EventArgs args)
        {
            IsPaused = true;
        }
    }
}