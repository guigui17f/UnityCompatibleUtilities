using System;
using UnityEngine;

namespace GUIGUI17F
{
    [Serializable]
    public class QuaternionArray
    {
        public Quaternion[] Data;
        
        public Quaternion this[int index]
        {
            get => Data[index];
            set => Data[index] = value;
        }
        
        public int Length => Data.Length;
        
        public QuaternionArray()
        {
        }
        
        public QuaternionArray(int length)
        {
            Data = new Quaternion[length];
        }
        
        public QuaternionArray(Quaternion[] data)
        {
            Data = data;
        }
    }
}