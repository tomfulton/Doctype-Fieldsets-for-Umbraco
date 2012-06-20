using System.Configuration;

namespace Our.Umbraco.DocTypeFieldsets.Configuration.GenericProperties
{
    [ConfigurationCollection(
        typeof(GenericPropertyElement),
        CollectionType = ConfigurationElementCollectionType.BasicMap,
        AddItemName = "genericProperty")]
    public class GenericPropertyCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new GenericPropertyElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as GenericPropertyElement).Alias;
        }

        public void Add(GenericPropertyElement element)
        {
            this.BaseAdd(element);
        }

        public void Clear()
        {
            this.BaseClear();
        }

        public int IndexOf(GenericPropertyElement element)
        {
            return this.BaseIndexOf(element);
        }

        public void Remove(GenericPropertyElement element)
        {
            if (this.BaseIndexOf(element) >= 0)
            {
                this.BaseRemove(element.Alias);
            }
        }

        public void RemoveAt(int index)
        {
            this.BaseRemoveAt(index);
        }

        public GenericPropertyElement this[int index]
        {
            get
            {
                return (GenericPropertyElement)this.BaseGet(index);
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