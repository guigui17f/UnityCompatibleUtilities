using System;
using UnityEngine;

namespace GUIGUI17F
{
    /// <summary>
    /// used by MicrophoneRecorder
    /// </summary>
    public class MicrophoneVolumeDetector : MonoBehaviour
    {
        public event Action<float, float> OnVolumeUpdate;

        private AudioClip _clip;
        private string _deviceName;
        private int _position;
        private int _sampleCount;
        private float[] _sampleData;
        private float _sumValue;
        private float _maxValue;

        public void Initialize(AudioClip microphoneClip, string deviceName)
        {
            _clip = microphoneClip;
            _deviceName = deviceName;
            _sampleCount = Mathf.CeilToInt(Time.unscaledDeltaTime * _clip.frequency);
            _sampleData = new float[_sampleCount * _clip.channels];
        }

        private void Update()
        {
            _position = Microphone.GetPosition(_deviceName);
            if (_position <= _sampleCount)
            {
                return;
            }
            _clip.GetData(_sampleData, _position - _sampleCount - 1);
            _sumValue = 0;
            _maxValue = -1;
            for (int i = 0; i < _sampleData.Length; i++)
            {
                _sumValue += _sampleData[i];
                if (_sampleData[i] > _maxValue)
                {
                    _maxValue = _sampleData[i];
                }
            }
            OnVolumeUpdate?.Invoke(_sumValue / _sampleData.Length, _maxValue);
        }
        
        public void Dispose()
        {
            Destroy(gameObject);
        }
    }
}
