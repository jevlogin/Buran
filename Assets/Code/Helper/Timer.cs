using System;
using UnityEngine;


namespace WORLDGAMEDEVELOPMENT
{
    internal sealed class Timer : IExecute
    {
        private int _hours;
        private int _minutes;
        private int _seconds;
        private int _secondsPrev;
        private float elapsedTime = 0f;
        private int _secondsToCountDown;
        private bool _isStartedTimer = false;

        public event Action<string> OnChangeFullTime;
        public event Action<string> OnChangeTimeMinutes;
        public event Action OnTimerLeftCountDown;

        internal void StartTimer(int sec)
        {
            _secondsToCountDown = sec;
            _isStartedTimer = true;
        }

        public void Execute(float deltatime)
        {
            elapsedTime += Time.deltaTime;

            _hours = Mathf.FloorToInt(elapsedTime / 3600);
            _minutes = Mathf.FloorToInt((elapsedTime % 3600) / 60);
            _seconds = Mathf.FloorToInt(elapsedTime % 60);

            if (MathF.Abs(_seconds - _secondsPrev) >= 1)
            {
                _secondsPrev = _seconds;
                var fullTime = string.Format("{0} ����� {1} ����� {2:00} ������", _hours, _minutes, _seconds);
                var minutes = string.Format("{0} ����� {1:00} ������", _minutes, _seconds);
                OnChangeFullTime?.Invoke(fullTime);
                OnChangeTimeMinutes?.Invoke(minutes);

                if(_seconds >= _secondsToCountDown && _isStartedTimer)
                {
                    _isStartedTimer = false;
                    OnTimerLeftCountDown?.Invoke();
                }
            }
        }
    }
}