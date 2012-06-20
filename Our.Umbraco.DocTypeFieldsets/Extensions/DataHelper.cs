using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Configuration;
using Our.Umbraco.DocTypeFieldsets.Configuration;
using Our.Umbraco.DocTypeFieldsets.Configuration.ContentTypes;
using Our.Umbraco.DocTypeFieldsets.Configuration.GenericProperties;
using umbraco.cms.businesslogic;

namespace Our.Umbraco.DocTypeFieldsets.Extensions
{
    public class DataHelper
    {
        public static ConfigSection GetConfigSection()
        {
            var config = WebConfigurationManager.OpenWebConfiguration("~/");
            var section = config.GetSection(Common.ConfigName) as ConfigSection;
            return section;
        }

        public static int GetDisplayMode()
        {
            var section = GetConfigSection();
            if (section == null)
                return 1;

            var mode = section.DisplayMode;
            if (mode == 1 || mode == 2 || mode == 3)
                return mode;
            
            return 1;
        }

        public static void AddOrUpdateProperty(string alias, string fieldset, int contentTypeId)
        {
            var section = GetConfigSection();
            var contentTypeElement = section.ContentTypes.Cast<ContentTypeElement>().FirstOrDefault(c => c.Id == contentTypeId);

            if (contentTypeElement == null)
            {
                contentTypeElement = new ContentTypeElement { Id = contentTypeId };
                section.ContentTypes.Add(contentTypeElement);
            }
                

            var genericPropertyElement =
                contentTypeElement.GenericProperties.Cast<GenericPropertyElement>().FirstOrDefault (p => p.Alias == alias);

                if (genericPropertyElement == null)
                {
                    genericPropertyElement = new GenericPropertyElement {Alias = alias, Fieldset = fieldset};
                    contentTypeElement.GenericProperties.Add(genericPropertyElement);
                }
                genericPropertyElement.Fieldset = fieldset;
                
                section.CurrentConfiguration.Save();
        }

        public static string GetFieldsetForProperty(string alias, int contentTypeId)
        {
            var section = GetConfigSection();
            var contentTypeElement = section.ContentTypes.Cast<ContentTypeElement>().FirstOrDefault(c => c.Id == contentTypeId);
            if (contentTypeElement == null)
                return "";

            var genericPropertyElement =
                contentTypeElement.GenericProperties.Cast<GenericPropertyElement>().FirstOrDefault(g => g.Alias == alias);
            return genericPropertyElement == null ? "" : genericPropertyElement.Fieldset;
        }

        public static IEnumerable<GenericPropertyElement> GetPropertiesForContentType(int contentTypeId)
        {
            var section = GetConfigSection();
            var contentTypeElement = section.ContentTypes.Cast<ContentTypeElement>().FirstOrDefault(c => c.Id == contentTypeId);

        	IEnumerable<GenericPropertyElement> list = Enumerable.Empty<GenericPropertyElement>();

			if (contentTypeElement != null)
				list = contentTypeElement.GenericProperties.Cast<GenericPropertyElement>();

        	var contentType = new ContentType(contentTypeId);

			// Check for master content types and return their configured properties as well
			// TODO: refactor this / clean up
			if (contentType.MasterContentType > 0)
			{
				var masterContentType = contentType;
				do
				{
					masterContentType = new ContentType(masterContentType.MasterContentType);

					var masterContentTypeElement =
						section.ContentTypes.Cast<ContentTypeElement>().FirstOrDefault(c => c.Id == masterContentType.Id);
	
					if (masterContentTypeElement != null)
						list = list.Concat(masterContentTypeElement.GenericProperties.Cast<GenericPropertyElement>());
						
				}
				while (masterContentType.MasterContentType > 0);
			}
        	return list;
        }
    }
}