using DZ.SAMP.Client.Settings;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace DZ.SAMP.Client.Models
{
    public class Language : AbstractXMLItem
    {
        public string Name { get; set; }
        public string Culture { get; set; }

        public Language(string name, string culture)
        {
            this.Name = name;
            this.Culture = culture;
        }

        public Language (XElement e) : base(e)
        {

        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
