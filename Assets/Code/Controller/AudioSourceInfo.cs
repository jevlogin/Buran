﻿using UnityEngine;


namespace WORLDGAMEDEVELOPMENT
{
    public sealed class AudioSourceInfo
    {
        public AudioSource Source;
        public float Delay;

        public AudioSourceInfo(AudioSource source, float length)
        {
            Source = source;
            Delay = length;
        }

        public bool IsPaused { get; internal set; }
        public float CurrentPlaybackPosition { get; internal set; }
    }
}