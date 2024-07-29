using System;

namespace GUIGUI17F
{
    [Serializable]
    public class AnchorArray
    {
        public AnchorData[] Data;

        public AnchorData this[int index]
        {
            get => Data[index];
            set => Data[index] = value;
        }

        public int Length => Data.Length;

        public AnchorArray()
        {
        }

        public AnchorArray(int length)
        {
            Data = new AnchorData[length];
        }

        public AnchorArray(AnchorData[] data)
        {
            Data = data;
        }
    }
}