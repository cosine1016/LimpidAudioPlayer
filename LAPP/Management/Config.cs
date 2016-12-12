using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;

namespace LAPP.Management
{
    public sealed class ConfigDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
    {
        public ConfigDictionary() { }
        public ConfigDictionary(Func<TKey, TValue> KeyConverter) { this.KeyConverter = KeyConverter; }

        public Func<TKey, TValue> KeyConverter { get; set; } = null;
        public Func<TValue, TValue> SettingValueFunction { get; set; } = null;
        public Func<TValue, TValue> GettingValueFunction { get; set; } = null;

        public new TValue this[TKey Key]
        {
            get
            {
                bool constant = false;
                TValue constant_val = GetConstantValue(Key, out constant);

                if (constant)
                    return constant_val;

                if (ContainsKey(Key))
                {
                    TValue ret = base[Key];
                    if (GettingValueFunction != null)
                        ret = GettingValueFunction(ret);

                    return ret;
                }
                else
                    return GetDefaultValue(Key);
            }
            set
            {
                TValue val = value;
                if (SettingValueFunction != null)
                    val = SettingValueFunction(val);

                base[Key] = val;
            }
        }

        public TValue GetConstantValue(TKey Key, out bool IsConstant)
        {
            ConfigAttribute attribute = Attribute.GetCustomAttribute(typeof(TKey).GetField(Key.ToString()),
                typeof(ConfigAttribute)) as ConfigAttribute;

            if (attribute != null)
            {
                IsConstant = attribute.IsConstant;

                if (attribute.IsConstant)
                    return (TValue)attribute.Default;
                else
                    return default(TValue);
            }
            else
            {
                IsConstant = false;
                return default(TValue);
            }
        }

        public TValue GetDefaultValue(TKey Key)
        {
            ConfigAttribute attribute = Attribute.GetCustomAttribute(typeof(TKey).GetField(Key.ToString()),
                typeof(ConfigAttribute)) as ConfigAttribute;

            if (attribute != null)
            {
                if (GettingValueFunction != null)
                    return GettingValueFunction((TValue)attribute.Default);
                else
                    return (TValue)attribute.Default;
            }
            else
            {
                if (KeyConverter != null)
                    return KeyConverter(Key);
                else
                    return default(TValue);
            }
        }

        public void Reset(TKey Key)
        {
            TValue val = GetDefaultValue(Key);
            this[Key] = val;
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            if (reader.IsEmptyElement)
                return;

            XmlSerializer serializer = new XmlSerializer(typeof(KeyValue));

            reader.Read();

            while (reader.NodeType != XmlNodeType.EndElement)
            {
                KeyValue kv = serializer.Deserialize(reader) as KeyValue;
                if (kv != null)
                    Add(kv.Key, kv.Value);
            }
            reader.Read();
        }

        public void WriteXml(XmlWriter writer)
        {
            if (Keys.Count > 0)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(KeyValue));
                foreach (var key in Keys)
                {
                    serializer.Serialize(writer, new KeyValue(key, this[key]));
                }
            }
        }

        public class KeyValue
        {
            public KeyValue() { }
            public KeyValue(TKey key, TValue value)
            {
                Key = key;
                Value = value;
            }

            public TKey Key { get; set; }
            public TValue Value { get; set; }
        }
    }

    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
    public sealed class ConfigAttribute : Attribute
    {
        public ConfigAttribute(object DefaultValue)
        {
            Default = DefaultValue;
        }

        public ConfigAttribute(object DefaultValue, bool IsConstant) : this(DefaultValue)
        {
            this.IsConstant = IsConstant;
        }

        public object Default { get; set; }
        public bool IsConstant { get; set; } = false;
    }
}
