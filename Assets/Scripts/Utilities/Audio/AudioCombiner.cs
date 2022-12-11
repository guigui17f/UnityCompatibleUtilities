using System;
using System.Collections.Generic;
using UnityEngine;

namespace GUIGUI17F
{
    public class AudioCombiner
    {
        public static AudioClip GetCombinedAudio(List<AudioClip> audioList, bool deleteSource)
        {
            int sampleLength = 0;
            foreach (AudioClip clip in audioList)
            {
                sampleLength += clip.samples;
            }
            float[] resultData = new float[sampleLength * audioList[0].channels];

            int copyOffset = 0;
            for (int i = 0; i < audioList.Count; i++)
            {
                float[] sourceData = new float[audioList[i].samples * audioList[i].channels];
                audioList[i].GetData(sourceData, 0);
                //trim empty audio data
                int copyStart = i == 0 ? 0 : Array.FindIndex(sourceData, item => Mathf.Abs(item) > Mathf.Epsilon);
                while (copyStart % audioList[i].channels != 0)
                {
                    copyStart++;
                }
                int copyEnd = i == audioList.Count - 1 ? sourceData.Length - 1 : Array.FindLastIndex(sourceData, item => Mathf.Abs(item) > Mathf.Epsilon);
                while ((copyEnd + 1) % audioList[i].channels != 0)
                {
                    copyEnd--;
                }
                int copyLength = copyEnd - copyStart + 1;
                Buffer.BlockCopy(sourceData, copyStart * 4, resultData, copyOffset * 4, copyLength * 4);
                copyOffset += copyLength;
            }
            Array.Resize(ref resultData, copyOffset);

            int channelCount = audioList[0].channels;
            int frequency = audioList[0].frequency;
            AudioClip resultAudio = AudioClip.Create(audioList[0].name, resultData.Length / channelCount, channelCount, frequency, false);
            resultAudio.SetData(resultData, 0);
            if (deleteSource)
            {
                foreach (AudioClip clip in audioList)
                {
                    UnityEngine.Object.Destroy(clip);
                }
                audioList.Clear();
            }
            return resultAudio;
        }
    }
}