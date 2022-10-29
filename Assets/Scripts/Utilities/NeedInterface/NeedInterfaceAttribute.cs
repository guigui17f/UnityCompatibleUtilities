using UnityEngine;
using System;

namespace GUIGUI17F
{
    /// <summary>
    /// use this attribute on a MonoBehaviour field to claim this MonoBehaviour must implement a certain interface
    /// </summary>
    public class NeedInterfaceAttribute : PropertyAttribute
    {
        public readonly Type InterfaceType;

        public NeedInterfaceAttribute(Type interfaceType)
        {
            InterfaceType = interfaceType;
        }
    }
}