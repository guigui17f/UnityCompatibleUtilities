using System;
using System.Threading.Tasks;
using UnityEngine;

namespace GUIGUI17F
{
    public class MicrophoneRecorder
    {
        private const string LogTag = "MicrophoneRecorder";
        private const int DefaultFrequency = 44100;
        private static AudioClip _audioClip;
        private static string _deviceName;
        private static MicrophoneVolumeDetector _detector;
        private static int _startPosition;
        private static int _endPosition;

        public static bool StartRecord(int maxSeconds)
        {
            int recordFrequency = DefaultFrequency;
            if (_deviceName == null)
            {
                string[] devices = Microphone.devices;
                int selectMinFreq = 0;
                int selectMaxFreq = 0;
                for (int i = 0; i < devices.Length; i++)
                {
                    Microphone.GetDeviceCaps(devices[i], out int minFreq, out int maxFreq);
                    if (minFreq == 0 && maxFreq == 0)
                    {
                        _deviceName = devices[i];
                        selectMinFreq = 0;
                        selectMaxFreq = int.MaxValue;
                        break;
                    }
                    else if (maxFreq > selectMaxFreq)
                    {
                        _deviceName = devices[i];
                        selectMinFreq = minFreq;
                        selectMaxFreq = maxFreq;
                    }
                }
                if (_deviceName != null)
                {
                    recordFrequency = Mathf.Clamp(DefaultFrequency, selectMinFreq, selectMaxFreq);
                    Debug.unityLogger.Log(LogTag, $"{_deviceName} support frequency is {selectMinFreq} - {selectMaxFreq}, in {devices.Length} microphones.");
                }
                else
                {
                    Debug.unityLogger.Log(LogTag, "Can't find usable microphone, try to use default device.");
                }
            }

            if (Microphone.IsRecording(_deviceName))
            {
                Microphone.End(_deviceName);
                Debug.unityLogger.LogError(LogTag, "Microphone hasn't closed since last record, close it first.");
            }

            _startPosition = 0;
            _endPosition = 0;
            _audioClip = Microphone.Start(_deviceName, true, maxSeconds, recordFrequency);
            bool success = _audioClip != null;
            if (success)
            {
                Debug.unityLogger.Log(LogTag, $"Start microphone successfully, max seconds is {maxSeconds}, device name is {_deviceName ?? "DefaultDevice"}.");
            }
            else
            {
                Debug.unityLogger.LogError(LogTag, "Start microphone failed!");
            }

            return success;
        }

        public static async void MarkStartPosition()
        {
            _startPosition = 0;
            for (int i = 0; i < 3; i++)
            {
                _startPosition = Microphone.GetPosition(_deviceName);
                if (_startPosition > 0)
                {
                    break;
                }
                await Task.Delay(20);
            }
            if (_startPosition == 0)
            {
                Debug.unityLogger.LogError(LogTag, "Get microphone position failed!");
            }
        }

        public static async void MarkEndPosition()
        {
            _endPosition = 0;
            for (int i = 0; i < 3; i++)
            {
                _endPosition = Microphone.GetPosition(_deviceName);
                if (_endPosition > 0)
                {
                    break;
                }
                await Task.Delay(20);
            }
            if (_endPosition == 0)
            {
                Debug.unityLogger.LogError(LogTag, "Get microphone position failed!");
            }
        }

        /// <summary>
        /// arg0: average value, arg1: maximum value
        /// </summary>
        public static void AddRecordDetector(Action<float, float> callback)
        {
            if (_audioClip == null)
            {
                Debug.unityLogger.LogError(LogTag, "Record hasn't started");
                return;
            }
            if (_detector == null)
            {
                GameObject go = new GameObject("MicrophoneVolumeDetector");
                UnityEngine.Object.DontDestroyOnLoad(go);
                _detector = go.AddComponent<MicrophoneVolumeDetector>();
                _detector.Initialize(_audioClip, _deviceName);
            }
            _detector.OnVolumeUpdate += callback;
        }

        public static void RemoveRecordDetector(Action<float, float> callback)
        {
            if (_detector != null)
            {
                _detector.OnVolumeUpdate -= callback;
            }
        }

        public static AudioClip EndRecord()
        {
            if (_detector != null)
            {
                _detector.Dispose();
                _detector = null;
            }
            if (!Microphone.IsRecording(_deviceName))
            {
                return null;
            }

            if (_endPosition == 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    _endPosition = Microphone.GetPosition(_deviceName);
                    if (_endPosition > 0)
                    {
                        break;
                    }
                }
                if (_endPosition == 0)
                {
                    Debug.unityLogger.LogError(LogTag, "Get microphone position failed!");
                }
            }
            Microphone.End(_deviceName);
            Debug.unityLogger.Log(LogTag, $"Stop microphone, start position is {_startPosition}, end position is {_endPosition}.");

            if (_audioClip != null)
            {
                if (_startPosition >= _endPosition)
                {
                    Debug.unityLogger.LogError(LogTag, "Start position is larger than end position, record result will be null!");
                    return null;
                }

                float[] sourceData = new float[_audioClip.samples * _audioClip.channels];
                _audioClip.GetData(sourceData, 0);
                float[] resultData = new float[(_endPosition - _startPosition + 1) * _audioClip.channels];
                Buffer.BlockCopy(sourceData, _startPosition * _audioClip.channels * 4, resultData, 0, resultData.Length * 4);
                AudioClip resultClip = AudioClip.Create(_audioClip.name, resultData.Length / _audioClip.channels, _audioClip.channels, _audioClip.frequency, false);
                resultClip.SetData(resultData, 0);
                UnityEngine.Object.Destroy(_audioClip);
                _audioClip = null;

                Debug.unityLogger.Log(LogTag, $"The result clip sample length is {resultClip.samples}, time length is {resultClip.length}.");
                return resultClip;
            }
            else
            {
                return null;
            }
        }
    }
}