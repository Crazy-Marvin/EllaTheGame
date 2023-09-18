using UnityEngine;
using System;

namespace EasyMobile.Internal
{
    public class RenameAttribute : PropertyAttribute
    {
        public string NewName { get ; private set; }

        public RenameAttribute(string name)
        {
            NewName = name;
        }
    }
}