using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace EasyMobile.Internal
{
    /// <typeparam name="T">Key type.</typeparam>
    /// <typeparam name="U">Value type.</typeparam>
    /// <typeparam name="V">Value storage type.</typeparam>
    public abstract class SerializableDictionaryBase<T, U, V> : 
        Dictionary<T, U>, ISerializationCallbackReceiver
    {
        [SerializeField]
        protected T[] keys;

        [SerializeField]
        protected V[] values;

        public SerializableDictionaryBase(IDictionary<T, U> dict)
            : base(dict.Count)
        {
            foreach (var kvp in dict)
            {
                this[kvp.Key] = kvp.Value;
            }
        }

        public SerializableDictionaryBase()
        {
        }

        protected SerializableDictionaryBase(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        protected abstract void SetValue(V[] storage, int i, U value);

        protected abstract U GetValue(V[] storage, int i);

        public void CopyFrom(IDictionary<T, U> dict)
        {
            this.Clear();
            foreach (var pair in dict)
            {
                this[pair.Key] = pair.Value;
            }
        }

        public void OnAfterDeserialize()
        {
            if (keys != null && values != null && keys.Length == values.Length)
            {
                this.Clear();
                for (int i = 0; i < keys.Length; ++i)
                {
                    this[keys[i]] = GetValue(values, i);
                }

                keys = null;
                values = null;
            }
        }

        public void OnBeforeSerialize()
        {
            keys = new T[this.Count];
            values = new V[this.Count];

            int i = 0;
            foreach (var pair in this)
            {
                keys[i] = pair.Key;
                SetValue(values, i, pair.Value);
                ++i;
            }
        }
    }
}
