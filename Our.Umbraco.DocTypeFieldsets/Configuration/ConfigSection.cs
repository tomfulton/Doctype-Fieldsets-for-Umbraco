using System.Configuration;
using Our.Umbraco.DocTypeFieldsets.Configuration.ContentTypes;

namespace Our.Umbraco.DocTypeFieldsets.Configuration
{
    public class ConfigSection : ConfigurationSection
    {
        private static readonly ConfigurationProperty contenttypes = new ConfigurationProperty("ContentTypes", typeof(ContentTypeCollection), null, ConfigurationPropertyOptions.IsDefaultCollection);
        private static ConfigurationProperty displayMode;
        private static readonly ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();

        static ConfigSection()
        {
            displayMode = new ConfigurationProperty("displayMode", typeof(int), 1, ConfigurationPropertyOptions.None);
            properties.Add(contenttypes);
            properties.Add(displayMode);
        }

        [ConfigurationProperty("ContentTypes", Options = ConfigurationPropertyOptions.IsDefaultCollection)]
        public ContentTypeCollection ContentTypes
        {
            get
            {
                return (ContentTypeCollection)base[contenttypes];
            }
        }

        [ConfigurationProperty("displayMode", IsKey = false, IsRequired = false)]
        public int DisplayMode
        {
            get
            {
                return (int)base[displayMode];
            }
            set
            {
                base[displayMode] = value;
            }
        }

 
    }
}