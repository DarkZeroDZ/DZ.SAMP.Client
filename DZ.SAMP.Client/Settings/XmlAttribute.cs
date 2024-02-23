using System;

namespace DZ.SAMP.Client.Settings
{
    public enum Priority
    {
        First,
        Last,
        Skip
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class XMLSerializePriority : Attribute
    {
        public Priority Priority;

        public XMLSerializePriority(Priority prio)
        {
            this.Priority = prio;
        }
    }
}
