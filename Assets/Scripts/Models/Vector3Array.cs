using System;
using UnityEngine;

namespace GUIGUI17F
{
    [Serializable]
    public class Vector3Array
    {
        public Vector3[] Data;
        
        public Vector3 this[int index]
        {
            get => Data[index];
            set => Data[index] = value;
        }
        
        public int Length => Data.Length;
        
        public Vector3Array()
        {
        }
        
        public Vector3Array(int length)
        {
            Data = new Vector3[length];
        }

        public Vector3Array(Vector3[] data)
        {
            Data = data;
        }
    }
}