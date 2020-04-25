﻿using System.Collections.Generic;
using System.Text;
using System.Dynamic;
using System.Linq.Expressions;
using System.ComponentModel;
using System.Collections;
using System;
using System.Reflection;
using System.Buffers;
using System.Diagnostics;

namespace Azure.Data
{
    class DictionaryStore : DataStore
    {
        Dictionary<string, object> _properties = new Dictionary<string, object>(StringComparer.Ordinal);
        bool _readonly = false;

        protected internal override IEnumerable<string> PropertyNames => _properties.Keys;

        protected internal override bool IsReadOnly => false;

        internal void Freeze() => _readonly = true;

        protected internal override Data CreateCore(ReadOnlySpan<(string propertyName, object propertyValue)> properties)
        {
            var result = new DictionaryStore();
            for (int i = 0; i < properties.Length; i++)
            {
                var property = properties[i];
                result.SetPropertyCore(property.propertyName, property.propertyValue);
            }
            if (_readonly) result.Freeze();
            return new Data(result);
        }

        protected internal override void SetPropertyCore(string propertyName, object propertyValue)
        {
            if (_readonly) throw new InvalidOperationException("The data is read-only");
            _properties[propertyName] = propertyValue;
        }

        protected internal override bool TryConvertToCore(Type type, out object converted)
        {
            try
            {
                converted = Activator.CreateInstance(type);
                foreach (var property in _properties)
                {
                    PropertyInfo propertyInfo = type.GetProperty(property.Key, BindingFlags.Public | BindingFlags.Instance);
                    propertyInfo.SetValue(converted, property.Value);
                    // TDOO: this needs to deserialize complex objects
                }
                return true;
            }
            catch
            {
                converted = default;
                return false;
            }
        }

        protected internal override bool TryGetAtCore(int index, out object item)
        {
            throw new NotImplementedException();
        }

        protected internal override bool TryGetPropertyCore(string propertyName, out object propertyValue)
            => _properties.TryGetValue(propertyName, out propertyValue);

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("{");
            bool first = true;
            foreach (var propertyName in PropertyNames)
            {
                if (first) first = false;
                else sb.Append(",\n");
                sb.Append($"\t{propertyName} : {_properties[propertyName]}");
            }
            sb.Append("\n}");

            return sb.ToString();
        }
    }
}