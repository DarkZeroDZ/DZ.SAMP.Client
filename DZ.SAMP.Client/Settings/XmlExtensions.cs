using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace DZ.SAMP.Client.Settings
{
    public static class XmlExtensions
    {
        public static void AddProperty(this XElement e, object sender, string propertyName)
        {
            var pi = sender.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).First(x => x.Name == propertyName);
            var value = pi.GetValue(sender);
            var propTyp = pi.PropertyType;

            e.AddProperty(propertyName, value, propTyp);
        }

        private static void AddProperty(this XElement e, string name, object value, Type piTyp)
        {
            if (value == null)
            {
                e.Add(new XElement(name, new XAttribute("Type", "NULL")));
            }
            else
            {
                var typ = value.GetType();

                if (typeof(IDictionary).IsAssignableFrom(value.GetType()))
                {
                    var _e = new XElement(name);

                    var itemType = value.GetType().GetEnumeratedType();
                    var lst = value as IDictionary;

                    foreach (DictionaryEntry x in lst)
                    {
                        var item = new XElement("Item");
                        item.AddProperty("Key", x.Key, itemType);
                        item.AddProperty("Value", x.Value, itemType);
                        _e.Add(item);
                    }

                    e.Add(_e);
                }
                else if (value is IEnumerable && typ != typeof(string))
                {
                    var _e = new XElement(name);

                    var itemType = value.GetType().GetEnumeratedType();
                    var lst = value as IEnumerable;

                    foreach (var x in lst)
                    {
                        _e.AddProperty("Item", x, itemType);
                    }

                    e.Add(_e);
                }
                else
                {
                    var x = new XElement(name);

                    if (piTyp != null && !piTyp.Equals(typ))
                        x.Add(new XAttribute("Type", typ.Name));

                    if (typ.IsSubclassOf(typeof(AbstractXMLItem)))
                        ((AbstractXMLItem)value).AddAllProperties(x);
                    else
                        x.Add(new XAttribute("Value", value));

                    e.Add(x);
                }
            }
        }

        public static Type GetEnumeratedType(this Type type)
        {
            // provided by Array
            var elType = type.GetElementType();
            if (null != elType) return elType;

            // otherwise provided by collection
            var elTypes = type.GetGenericArguments();
            if (elTypes.Length > 0) return elTypes[0];

            // otherwise is not an 'enumerated' type
            return null;
        }
    }
}
