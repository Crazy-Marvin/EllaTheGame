using UnityEngine;
using System;

namespace EasyMobile.Internal
{
    public class EnumFlagsAttribute : PropertyAttribute
    {
        public string enumName;

        public EnumFlagsAttribute()
        {
        }

        public EnumFlagsAttribute(string name)
        {
            enumName = name;
        }
    }
}