using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NasladdinPlace.UI.Helpers
{
    [Serializable]
    public class DictionaryTypes<TValue> : Dictionary<Type, TValue>
    {
        public DictionaryTypes()
        {
            // intentionally left empty
        }

        protected DictionaryTypes(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            // intentionally left empty
        }

        public DictionaryTypes<TValue> Add<TKey>(TValue value)
        {
            Add(typeof(TKey), value);
            return this;
        }
    }
}
