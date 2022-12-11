using System;
using System.Collections;
using UnityEngine;

namespace GUIGUI17F
{
    public class GameTimer : MonoBehaviour, IDisposable
    {
        public event Action<float> CountEvent;
        public event Action EndEvent;

        /// <summary>
        /// timer: passed seconds
        /// count down: left seconds
        /// </summary>
        public float CurrentSeconds { get; private set; }

        /// <summary>
        /// timer: passed minutes
        /// count down: left minutes
        /// </summary>
        public float CurrentMinutes => CurrentSeconds / 60f;

        private float _startSeconds;
        private float _countDownSeconds;
        private bool _autoDispose;
        private WaitForSeconds _loopIntervalSeconds;
        private Coroutine _loopCoroutine;
        private bool _enabled;

        /// <summary>
        /// remember to call Dispose() when this instance is no longer needed
        /// </summary>
        public static GameTimer Create(bool dontDestroyOnLoad)
        {
            GameObject go = new GameObject($"GameTimer_{Time.time:F3}");
            if (dontDestroyOnLoad)
            {
                DontDestroyOnLoad(go);
            }
            return go.AddComponent<GameTimer>();
        }

        public void StartTimer(float noticeFrequency)
        {
            if (_loopCoroutine != null)
            {
                StopCoroutine(_loopCoroutine);
            }
            _startSeconds = Time.time;
            CurrentSeconds = 0;
            _enabled = true;
            _loopIntervalSeconds = new WaitForSeconds(noticeFrequency);
            _loopCoroutine = StartCoroutine(TimerCoroutine());
        }

        public void StartCountDown(float countDownSeconds, float noticeFrequency, bool autoDispose)
        {
            if (_loopCoroutine != null)
            {
                StopCoroutine(_loopCoroutine);
            }
            _startSeconds = Time.time;
            _countDownSeconds = countDownSeconds;
            _autoDispose = autoDispose;
            CurrentSeconds = countDownSeconds;
            _enabled = true;
            _loopIntervalSeconds = new WaitForSeconds(noticeFrequency);
            _loopCoroutine = StartCoroutine(CountDownCoroutine());
        }

        public void ChangeTimer(float deltaSeconds)
        {
            _startSeconds += deltaSeconds;
            _startSeconds = Mathf.Max(0, _startSeconds);
        }

        public void ClearListeners()
        {
            CountEvent = null;
            EndEvent = null;
        }

        public void Dispose()
        {
            _enabled = false;
            Destroy(gameObject);
        }

        private IEnumerator TimerCoroutine()
        {
            while (_enabled)
            {
                yield return _loopIntervalSeconds;
                CurrentSeconds = Time.time - _startSeconds;
                CountEvent?.Invoke(CurrentSeconds);
            }
        }

        private IEnumerator CountDownCoroutine()
        {
            bool finish = false;
            while (enabled && !finish)
            {
                yield return _loopIntervalSeconds;
                CurrentSeconds = _countDownSeconds - (Time.time - _startSeconds);
                finish = CurrentSeconds <= 0;
                if (finish)
                {
                    CurrentSeconds = 0;
                    CountEvent?.Invoke(CurrentSeconds);
                    EndEvent?.Invoke();
                }
                else
                {
                    CountEvent?.Invoke(CurrentSeconds);
                }
            }
            //avoid call Dispose() multiple times
            if (_autoDispose && enabled)
            {
                Dispose();
            }
        }
    }
}