using System;
using System.Collections.Generic;
using UnityEditor;
using EasyMobile.Internal;

namespace EasyMobile.Editor
{
    [CustomPropertyDrawer(typeof(StringStringSerializableDictionary))]
    [CustomPropertyDrawer(typeof(StringAdIdSerializableDictionary))]
    public class AnySerializableDictionaryPropertyDrawer : SerializableDictionaryPropertyDrawer
    {

    }

    public class AnySerializableDictionaryStoragePropertyDrawer : SerializableDictionaryStoragePropertyDrawer
    {

    }
}
