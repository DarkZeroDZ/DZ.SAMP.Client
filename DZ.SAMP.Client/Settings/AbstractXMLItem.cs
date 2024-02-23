using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Windows;
using System.Xml.Linq;

namespace DZ.SAMP.Client.Settings
{
    public abstract class AbstractXMLItem : INotifyPropertyChanged
    {
        [XMLSerializePriority(Priority.Skip)]
        protected virtual List<PropertyInfo> IgnoredProperites { get; set; } = new List<PropertyInfo>();

        #region Events
        public event EventHandler Applied;
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        public AbstractXMLItem() { }
        public AbstractXMLItem(XElement e)
        {
            e.Elements().ToList().ForEach(x => this.TrySet(x));
        }


        #region From XElment
        public void ApplyValues(AbstractXMLItem item)
        {
            var e = item.ToXElement();
            e.Elements().ToList().ForEach(x => this.TrySet(x));

            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
            this.Applied?.Invoke(this, EventArgs.Empty);
        }

        // Load
        protected bool TrySet(XElement x)
        {
            PropertyInfo pi = null;
            try
            {
                var properties = this.GetType().GetProperties();

                pi = properties.FirstOrDefault(p => p.Name == x.Name.LocalName);

                if (pi != null)
                {
                    // Liste von Typen                       
                    if (typeof(IEnumerable).IsAssignableFrom(pi.PropertyType) && pi.PropertyType != typeof(string))
                    {
                        var itemType = pi.PropertyType.GetEnumeratedType();
                        var count = x.Elements().Count();

                        if (pi.PropertyType.IsArray)
                        {
                            // Array erstellen wenn möglich
                            if (pi.GetSetMethod() != null)
                                pi.SetValue(this, Array.CreateInstance(itemType, count));

                            var array = (Array)pi.GetValue(this);

                            // Default Werte schreiben
                            if (pi.GetSetMethod() == null)
                            {
                                var empty = Array.CreateInstance(itemType, count);

                                for (int j = 0; j < count; j++)
                                {
                                    array.SetValue(empty.GetValue(j), j);
                                }
                            }

                            int i = 0;
                            foreach (var e in x.Elements())
                            {
                                array.SetValue(this.GetValue(e, itemType), i++);
                            }
                        }
                        else if (typeof(IList).IsAssignableFrom(pi.PropertyType))
                        {
                            // Liste erstellen wenn möglich
                            if (pi.GetSetMethod() != null)
                                pi.SetValue(this, Activator.CreateInstance(pi.PropertyType));

                            var lst = pi.GetValue(this) as IList;

                            lst.Clear();

                            // Datentyp des Inhalts
                            foreach (var e in x.Elements())
                            {
                                lst.Add(this.GetValue(e, itemType));
                            }
                        }
                        else if (typeof(IDictionary).IsAssignableFrom(pi.PropertyType))
                        {
                            // Liste erstellen wenn möglich
                            if (pi.GetSetMethod() != null)
                                pi.SetValue(this, Activator.CreateInstance(pi.PropertyType));

                            var lst = pi.GetValue(this) as IDictionary;

                            lst.Clear();

                            // Datentyp des Inhalts
                            foreach (var e in x.Elements())
                            {
                                var key = e.Element("Key").Attribute("Value").Value;
                                var value = e.Element("Value").Attribute("Value").Value;

                                lst.Add(key, value);
                            }
                        }
                    }

                    // Einfach Typen
                    else
                    {
                        pi.SetValue(this, this.GetValue(x, pi.PropertyType));
                    }


                    return true;
                }
                else
                {
                    Debug.WriteLine("Property not found: " + x.Name.LocalName);

                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(pi?.ToString() + Environment.NewLine + ex.ToString());
                return false;
            }
        }

        private object GetValue(XElement e, Type valueTyp)
        {
            var eType = e.Attribute("Type");
            if (eType?.Value == "NULL")
                return null;

            if (eType != null)
            {
                var typ = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                           from assemblyType in domainAssembly.GetTypes()
                           select assemblyType).FirstOrDefault(t => t.Name == eType.Value);

                if (typ.IsSubclassOf(typeof(AbstractXMLItem)))
                {
                    return Activator.CreateInstance(typ, e);
                }
                else
                {
                    return Convert.ChangeType(e.Attribute("Value").Value, typ);
                }
            }
            else
            {
                if (valueTyp.IsSubclassOf(typeof(AbstractXMLItem)))
                {
                    return Activator.CreateInstance(valueTyp, e);
                }
                else
                {
                    var value = e.Attribute("Value").Value;

                    if (valueTyp.IsEnum)
                        return Enum.Parse(valueTyp, value);
                    else if (valueTyp.Equals(typeof(IPAddress)))
                        return IPAddress.Parse(value);
                    else
                        return Convert.ChangeType(value, valueTyp);
                }
            }
        }
        #endregion

        #region To XElment
        public XElement ToXElement()
        {
            try
            {
                var e = new XElement(this.GetType().Name);

                this.AddAllProperties(e);

                return e;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                return null;
            }
        }

        internal void AddAllProperties(XElement e)
        {
            var properties = this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => !this.IgnoredProperites.Contains(p)).ToList();

            var unset = properties.Where(p => p.GetCustomAttribute<XMLSerializePriority>() == null).ToList();
            var first = properties.Where(p => p.GetCustomAttribute<XMLSerializePriority>()?.Priority == Priority.First).ToList();
            var last = properties.Where(p => p.GetCustomAttribute<XMLSerializePriority>()?.Priority == Priority.Last).ToList();

            first.ForEach(pi => this.AddProperty(e, pi));
            unset.ForEach(pi => this.AddProperty(e, pi));
            last.ForEach(pi => this.AddProperty(e, pi));
        }

        protected void AddProperty(XElement e, PropertyInfo pi)
        {
            if (pi.GetGetMethod() == null) return; 

            if (pi.GetSetMethod() != null || (typeof(IEnumerable).IsAssignableFrom(pi.PropertyType) && pi.PropertyType != typeof(string)))
            {
                e.AddProperty(this, pi.Name);
            }
        }
        #endregion
        
        public bool Equals(AbstractXMLItem other)
        {
            try
            {
                var _t = this.GetAllProperties();
                var _o = other.GetAllProperties();

                if (!_t.Count.Equals(_o.Count))
                    return false;

                for (int i = 0; i < _t.Count; i++)
                {
                    var t = _t[i];
                    var o = _o[i];

                    if (!this.RefEqualProperty(t, this, o, other))
                        return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return false;
            }
        }

        private bool RefEqualProperty(PropertyInfo p1, object s1, PropertyInfo p2, object s2)
        {
            try
            {
                if (p1.Name != p2.Name)
                    return false;
                if (p1.PropertyType != p2.PropertyType)
                    return false;
                if (!p1.GetType().Equals(p2.GetType()))
                    return false;

                var v1 = p1.GetValue(s1);
                var v2 = p2.GetValue(s2);

                if (v1 == null && v2 == null) return true;

                if ((v1 == null && v2 != null) || (v1 != null && v2 == null))
                    return false;

                var t1 = v1.GetType();
                var t2 = v2.GetType();

                if (!t1.Equals(t2))
                    return false;

                if (v1 is IEnumerable && t1 != typeof(string))
                {
                    var itemType = t1.GetEnumeratedType();

                    var l1 = (v1 as IEnumerable<object>).ToArray();
                    var l2 = (v2 as IEnumerable<object>).ToArray();

                    if (l1.Length != l2.Length)
                        return false;

                    for (int i = 0; i < l1.Length; i++)
                    {
                        if (!l1[i].Equals(l2[i]))
                            return false;
                    }

                    return true;
                }

                return v1.Equals(v2);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("RefEquel Exeption" + Environment.NewLine +
                    "P1: " + p1?.Name + " - " + p1?.PropertyType + Environment.NewLine +
                    "S1: " + s1 + Environment.NewLine +
                    "P2: " + p2?.Name + " - " + p2?.PropertyType + Environment.NewLine +
                    "S2: " + s2 + Environment.NewLine +
                       ex.Message);

                return false;
            }
        }

        private List<PropertyInfo> GetAllProperties()
        {
            var properties = this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

            var unset = properties.Where(p => p.GetCustomAttribute<XMLSerializePriority>() == null);
            var set = properties.Where(p => p.GetCustomAttribute<XMLSerializePriority>()?.Priority != Priority.Skip);

            return unset.Union(set).ToList();
        }

        public override bool Equals(object obj)
        {
            var other = obj as AbstractXMLItem;

            if (other == null) return false;

            return this.Equals(other);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
