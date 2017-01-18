﻿using System;

namespace InsireBot
{
    public interface IMediaPlayer: IDisposable
    {
        event CompletedMediaItemEventHandler CompletedMediaItem;

        bool Disposed { get; }
        bool IsPlaying { get; }
        bool CanPlay { get; }

        bool CanPause { get; }
        bool CanStop { get; }

        void Play();
        void Next();
        void Previous();

        void Pause();
        void Stop();

        Playlist Playlist { get; }
        int Volume { get; set; }
        int VolumeMax { get; }
        int VolumeMin { get; }
        AudioDevice AudioDevice { get; set; }
    }
}
