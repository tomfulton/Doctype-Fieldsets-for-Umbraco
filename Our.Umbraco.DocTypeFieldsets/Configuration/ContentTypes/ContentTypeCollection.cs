using System.Configuration;

namespace Our.Umbraco.DocTypeFieldsets.Configuration.ContentTypes
{
    [ConfigurationCollection(
        typeof(ContentTypeElement), 
        CollectionType = ConfigurationElementCollectionType.BasicMap,
        AddItemName = "contentType")]
    public class ContentTypeCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ContentTypeElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as ContentTypeElement).Id;
        }

        public void Add(ContentTypeElement element)
        {
            this.BaseAdd(element);
        }

        public void Clear()
        {
            this.BaseClear();
        }

        public int IndexOf(ContentTypeElement element)
        {
            return this.BaseIndexOf(element);
        }

        public void Remove(ContentTypeElement element)
        {
            if (this.BaseIndexOf(element) >= 0)
            {
                this.BaseRemove(element.Id);
            }
        }

        public void RemoveAt(int index)
        {
            this.BaseRemoveAt(index);
        }

        public ContentTypeElement this[int index]
        {
            get
            {
                return (ContentTypeElement)this.BaseGet(index);
            }
            set
            {
                if (this.BaseGet(index) != null)
                {
                    this.BaseRemoveAt(index);
                }
                this.BaseAdd(index, value);
            }
        }

        [ConfigurationProperty("id", IsRequired = false)]
        public int Id
        {
            get { return (int)this["id"]; }
            set
            {
                this["id"] = value;
            }
        }

    }
}