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
    public static class DataHelper
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
						list = list.Concat(masterContentTypeElement.GenericProperties.Cast<GenericPropertyElement>().AddSortOrder(masterContentType.Id).OrderByDescending(p => p.SortOrder));
						
				}
				while (masterContentType.MasterContentType > 0);
			}

			// We added the properties in reverse order (walking up the master tree) - Reverse to get the "correct" order
			list = list.Reverse();

			// Add the properties from the "actual" doctype AFTER the masters have been added
			if (contentTypeElement != null)
				list = list.Concat(contentTypeElement.GenericProperties.Cast<GenericPropertyElement>().AddSortOrder(contentTypeId).OrderBy(p => p.SortOrder));


        	return list;
        }

		/// <summary>
		/// Adds each property's Sort Order as defined in Umbraco to a list of GenericPropertyElements
		/// Does not take into account the master doctype properties
		/// </summary>
		/// <param name="properties">List of GenericPropertyElements to add the sort order to</param>
		/// <param name="contentTypeId">Content type where the properties exist</param>
		/// <returns>IEnumerable containing GenericPropertyElements with SortOrder populated</returns>
		private static IEnumerable<GenericPropertyElement> AddSortOrder(this IEnumerable<GenericPropertyElement> properties, int contentTypeId)
		{
			var umbracoProperties = new ContentType(contentTypeId).PropertyTypes.Where(p => p.ContentTypeId == contentTypeId).ToList();
			
			// By default properties have a 0 sortorder until manually sorted - check for this situation
			var arePropertiesSorted = umbracoProperties.Any(p => p.SortOrder > 0);

			var _properties = properties.ToList();
			foreach (var property in _properties)
			{
				var umbracoProperty = umbracoProperties.FirstOrDefault(p => p.Alias == property.Alias);
				if (umbracoProperty != null)
				{
					property.SortOrder = arePropertiesSorted ? umbracoProperty.SortOrder : umbracoProperties.IndexOf(umbracoProperty);
				}
			}

			return properties;
		}
    }
}