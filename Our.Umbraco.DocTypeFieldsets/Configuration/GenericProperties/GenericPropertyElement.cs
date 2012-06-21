using System.Configuration;

namespace Our.Umbraco.DocTypeFieldsets.Configuration.GenericProperties
{
    public class GenericPropertyElement : ConfigurationElement
    {
        #region Private Properties
        
        private static ConfigurationPropertyCollection properties;

        private static ConfigurationProperty alias;

        private static ConfigurationProperty fieldset;

		#endregion

        static GenericPropertyElement()
        {
            alias = new ConfigurationProperty("alias", typeof(string), null, ConfigurationPropertyOptions.IsRequired);
            fieldset = new ConfigurationProperty("fieldset", typeof(string), null, ConfigurationPropertyOptions.None);
			
            properties = new ConfigurationPropertyCollection { alias, fieldset };
        }

        #region Public Properties

        [ConfigurationProperty("alias", IsKey = true, IsRequired = true)]
        public string Alias
        {
            get
            {
                return (string)base[alias];
            }
            set
            {
                base[alias] = value;
            }
        }

		[ConfigurationProperty("fieldset", DefaultValue = "")]
		public string Fieldset
		{
			get
			{
				return (string)base[fieldset];
			}
			set
			{
				base[fieldset] = value;
			}
		}

    	public int SortOrder { get; set; }

        #endregion
    }
}