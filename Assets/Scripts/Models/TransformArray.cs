using System;

namespace GUIGUI17F
{
    [Serializable]
    public class TransformArray
    {
        public TransformData[] Data;
        
        public TransformData this[int index]
        {
            get => Data[index];
            set => Data[index] = value;
        }
        
        public int Length => Data.Length;
        
        public TransformArray()
        {
        }
        
        public TransformArray(int length)
        {
            Data = new TransformData[length];
        }
        
        public TransformArray(TransformData[] data)
        {
            Data = data;
        }
    }
}