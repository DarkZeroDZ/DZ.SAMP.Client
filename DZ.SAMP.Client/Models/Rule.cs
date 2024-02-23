namespace DZ.SAMP.Client.Models
{
    public class Rule
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public Rule(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }

        public Rule()
        {

        }
    }
}