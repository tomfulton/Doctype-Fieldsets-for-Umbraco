using System.Configuration;
using Our.Umbraco.DocTypeFieldsets.Configuration.GenericProperties;

namespace Our.Umbraco.DocTypeFieldsets.Configuration.ContentTypes
{
    public class ContentTypeElement : ConfigurationElement
    {
        #region Private Properties
        
        private static ConfigurationPropertyCollection properties;

        private static ConfigurationProperty id;

        private static readonly ConfigurationProperty genericproperties = new ConfigurationProperty("GenericProperties", typeof(GenericPropertyCollection), null, ConfigurationPropertyOptions.IsDefaultCollection);

        #endregion

        static ContentTypeElement()
        {
            id = new ConfigurationProperty("id", typeof(int), null, ConfigurationPropertyOptions.IsRequired);

            properties = new ConfigurationPropertyCollection { id, genericproperties };
        }

        #region Public Properties

        [ConfigurationProperty("id", IsKey = true, IsRequired = true)]
        public int Id
        {
            get
            {
                return (int)base[id];
            }
            set
            {
                base[id] = value;
            }
        }

        [ConfigurationProperty("GenericProperties", IsDefaultCollection = true)]
        public GenericPropertyCollection GenericProperties
        {
            get
            {
                return (GenericPropertyCollection)base[genericproperties];
            }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                return properties;
            }
        }

        #endregion
    }
}