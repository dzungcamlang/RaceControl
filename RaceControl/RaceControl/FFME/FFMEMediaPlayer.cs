using FFmpeg.AutoGen;
using Prism.Mvvm;
using RaceControl.Common.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Unosquare.FFME;
using Unosquare.FFME.Common;

namespace RaceControl.FFME
{
    public class FFMEMediaPlayer : BindableBase, IMediaPlayer
    {
        private readonly MediaElement _mediaElement;

        private long _time;
        private long _duration;
        private int _volume;
        private bool _isPaused;
        private bool _isMuted;
        private bool _isScanning;
        private bool _isCasting;
        private ObservableCollection<IMediaTrack> _audioTracks;
        private IMediaTrack _selectedAudioTrack;
        private ObservableCollection<IMediaRenderer> _mediaRenderers;
        private bool _disposed;

        public FFMEMediaPlayer(MediaElement mediaElement)
        {
            _mediaElement = mediaElement;
            _mediaElement.MediaInitializing += mediaElement_MediaInitializing;
            _mediaElement.MediaOpening += mediaElement_MediaOpening;
            _mediaElement.MediaOpened += mediaElement_MediaOpened;
            _mediaElement.MediaChanging += mediaElement_MediaChanging;
            _mediaElement.MediaChanged += mediaElement_MediaChanged;
            _mediaElement.PositionChanged += mediaElement_PositionChanged;
            _mediaElement.MediaStateChanged += mediaElement_MediaStateChanged;
        }

        public long Time
        {
            get => _time;
            set
            {
                if (SetProperty(ref _time, Math.Max(value, 0)))
                {
                    _mediaElement.Position = TimeSpan.FromMilliseconds(_time);
                    //_mediaElement.Seek(TimeSpan.FromMilliseconds(_time)).GetAwaiter().GetResult();
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
                    _mediaElement.Volume = _volume / 100D;
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
            AudioTracks.Clear();
            await _mediaElement.Open(new Uri(streamUrl));
            await _mediaElement.Play();
        }

        public async Task StopPlaybackAsync()
        {
            await _mediaElement.Stop();
            AudioTracks.Clear();
        }

        public void TogglePause()
        {
            if (!IsPaused)
            {
                _mediaElement.Pause();
            }
            else
            {
                _mediaElement.Play();
            }
        }

        public void ToggleMute()
        {
            _mediaElement.IsMuted = !_mediaElement.IsMuted;
            IsMuted = !IsMuted;
        }

        public async Task SetAudioTrackAsync(IMediaTrack audioTrack)
        {
            _selectedAudioTrack = audioTrack;
            await _mediaElement.ChangeMedia();
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
                // do nothing
            }

            _disposed = true;
        }

        private void mediaElement_MediaInitializing(object sender, MediaInitializingEventArgs e)
        {
        }

        private void mediaElement_MediaOpening(object sender, MediaOpeningEventArgs e)
        {
            var bestVideoStream = e.Info.Streams.Select(p => p.Value).Where(s => s.CodecType == AVMediaType.AVMEDIA_TYPE_VIDEO).OrderByDescending(s => s.PixelHeight).FirstOrDefault();

            if (bestVideoStream != null)
            {
                e.Options.VideoStream = bestVideoStream;
            }
        }

        private void mediaElement_MediaOpened(object sender, MediaOpenedEventArgs e)
        {
            Duration = Convert.ToInt64(e.Info.Duration.TotalMilliseconds);
            AudioTracks.AddRange(e.Info.Streams.Select(p => p.Value).Where(s => s.CodecType == AVMediaType.AVMEDIA_TYPE_AUDIO).Select(s => new FFMEMediaTrack(s)));
        }

        private void mediaElement_MediaChanging(object sender, MediaOpeningEventArgs e)
        {
            if (_selectedAudioTrack != null)
            {
                e.Options.AudioStream = e.Info.Streams[_selectedAudioTrack.Id];
            }
        }

        private void mediaElement_MediaChanged(object sender, MediaOpenedEventArgs e)
        {
        }

        private void mediaElement_PositionChanged(object sender, PositionChangedEventArgs e)
        {
            SetProperty(ref _time, Convert.ToInt64(e.Position.TotalMilliseconds), nameof(Time));
        }

        private void mediaElement_MediaStateChanged(object sender, MediaStateChangedEventArgs e)
        {
            switch (e.MediaState)
            {
                case MediaPlaybackState.Play:
                    IsPaused = false;
                    break;

                case MediaPlaybackState.Pause:
                    IsPaused = true;
                    break;
            }
        }
    }
}