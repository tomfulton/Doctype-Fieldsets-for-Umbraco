using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Our.Umbraco.DocTypeFieldsets.Extensions;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic;
using umbraco.cms.businesslogic.propertytype;
using umbraco.presentation.LiveEditing;
using umbraco.presentation.masterpages;
using umbraco.uicontrols;

namespace Our.Umbraco.DocTypeFieldsets.Events
{
    public class InjectEditorControls : ApplicationBase
    {
        private int _contentTypeId;

        public InjectEditorControls()
        {
            umbracoPage.Init += umbracoPage_Init;
        }

        private void umbracoPage_Init(object sender, EventArgs e)
        {
            var umbPage = sender as umbracoPage;

			if (umbPage == null)
				return;

			var path = umbPage.Page.Request.Path.ToLower();

			if (!path.Contains("editnodetypenew.aspx") && !path.Contains("editmediatype.aspx")) // TODO: variables?
				return;

			int.TryParse(HttpContext.Current.Request.QueryString["id"], out _contentTypeId);

			this.InjectExtensionControls(umbPage);
        }

        private void InjectExtensionControls(umbracoPage umbPage)
        {
            var propertiesPanel = Utility.FindControl<Control>((Control c) => c.ClientID.EndsWith("pnlProperties"), umbPage.Page);

            umbPage.Page.LoadComplete += rootProperties_Load;   // Needed to keep ... after postback
			//this.rootProperties_Load(umbPage, new EventArgs());

            //AddFieldsetTextbox(propertiesPanel, "new", true, "");
            AddFieldsetTextboxForNew(propertiesPanel);
            // AddFieldsetTextboxForAll(rootProperties);
        }

        void rootProperties_Load(object sender, EventArgs e)
        {
            if (((Control)sender).Page.IsPostBack)
            {
                SaveAll(((Control)sender));
            }

            AddFieldsetTextboxForAll((Control)sender);
        }

        private void AddFieldsetTextboxForNew(Control rootProperties)
        {
            var rootControl = Utility.FindControl<Control>((Control c) => c.ClientID.EndsWith("GenericPropertyNew_control"),
                                                    rootProperties);

            AddFieldsetTextbox(rootControl, "new_fieldset", true, "");
        }

        private void AddFieldsetTextboxForAll(Control rootProperties)
        {
            var allProperties = new ContentType(_contentTypeId).PropertyTypes.Where(c => c.ContentTypeId == _contentTypeId); // Skip masters -- NOTE err multiple controls same ID without this

            foreach (var property in allProperties)
            {
                PropertyType property1 = property;
                var propertyRootControl =
                    Utility.FindControl<Control>(
                        (Control c) =>
                        c.ClientID.EndsWith(string.Format("gpw_{0}_control", property1.Id)),
                        rootProperties);

                // If it's an inherited property there won't be a control for it
                if (propertyRootControl != null)
                    AddFieldsetTextbox(propertyRootControl, property1.Id + "_fieldset", false, DataHelper.GetFieldsetForProperty(property.Alias, _contentTypeId));
            }
        }

        private void AddFieldsetTextbox(Control rootContainer, string uid, bool isNew, string defaultValue)
        {
            var pane = Utility.FindControl<Pane>((Pane c) => c.GetType() == typeof(Pane), rootContainer);
            var container = new PropertyPanel { ID = "pane" + uid, CssClass = "propertyItem", Text = "Fieldset" };

            var nameBox = Utility.FindControl<TextBox>((TextBox c) => c.ClientID.EndsWith("tbAlias"), rootContainer);


            var textbox = new TextBox { ID = uid, CssClass = "propertyFormInput" };
            if (string.IsNullOrEmpty(textbox.Text))
                textbox.Text = defaultValue;
            if (isNew)
                textbox.Attributes["relatedTextbox"] = "ctl00$" + nameBox.ClientID.Replace("_", "$").Replace("GenericPropertyNew$control", "GenericPropertyNew_control"); // HACK
            else
                textbox.Attributes["relatedTextbox"] = "ctl00$" + nameBox.ClientID.Replace("_", "$").Replace("$gpw$", "$gpw_").Replace("$control$", "_control$"); // HACK
            // I don't think we're using this anymore :: textbox.TextChanged += textbox_TextChanged;
            container.Controls.Add(textbox);
            pane.Controls.Add(container);
            
        }

        // This isn't working after changing to Page.LoadComplete
        //void textbox_TextChanged(object sender, EventArgs e)
        //{
        //    var thisTextBox = (TextBox) sender;
        //    var related = thisTextBox.Attributes["relatedTextBox"];
        //    var alias = HttpContext.Current.Request.Form[related];

        //    // Don't update if save was pushed but the form wasn't filled out completely (no alias)
        //    if (!string.IsNullOrEmpty(alias))
        //        DataHelper.AddOrUpdateProperty(alias, ((TextBox)sender).Text, _contentTypeId);  
        //}

        void SaveAll(Control rootProperties)
        {
            var allProperties = new ContentType(_contentTypeId).PropertyTypes.Where(p=>p.ContentTypeId == _contentTypeId); // Skip master properties
            
            var allKeys = HttpContext.Current.Request.Form.AllKeys;
            // Check for "New Property" - if Name, Alias, & Fieldset not empty, save
            var newPropertyName = HttpContext.Current.Request.Form[allKeys.Where(x => x.EndsWith("GenericPropertyNew_control$tbName")).FirstOrDefault()];
            var newPropertyAlias = HttpContext.Current.Request.Form[allKeys.Where(x => x.EndsWith("GenericPropertyNew_control$tbAlias")).FirstOrDefault()];
            var newPropertyFieldset = HttpContext.Current.Request.Form[allKeys.Where(x => x.EndsWith("GenericPropertyNew_control$new_fieldset")).FirstOrDefault()];
            if (!string.IsNullOrEmpty(newPropertyName) && !string.IsNullOrEmpty(newPropertyAlias) && !string.IsNullOrEmpty(newPropertyFieldset))
            {
                DataHelper.AddOrUpdateProperty(newPropertyAlias, newPropertyFieldset, _contentTypeId);
            }
           
            foreach (var property in allProperties.Where(p => p.Alias != newPropertyAlias)) // Skip the newly added property else we'll overwrite it
            {
			
                PropertyType property1 = property;
                var rootControl =
                    Utility.FindControl<Control>(
                        (Control c) =>
                        c.ClientID.EndsWith(string.Format("gpw_{0}_control", property1.Id)),
                        rootProperties);

                var aliasTextbox = Utility.FindControl<TextBox>((TextBox c) => c.ClientID.EndsWith("tbAlias"), rootControl);
                var alias = aliasTextbox.Text;

                if (string.IsNullOrEmpty(alias))
                    return;

                var requestKey =
                    HttpContext.Current.Request.Form.AllKeys.Where(k => k.EndsWith(property.Id + "_fieldset")).FirstOrDefault();
                var newValue = HttpContext.Current.Request.Form[requestKey];
                var currentValue = DataHelper.GetFieldsetForProperty(aliasTextbox.Text, _contentTypeId);
                if (currentValue != newValue)
                    DataHelper.AddOrUpdateProperty(aliasTextbox.Text, newValue, _contentTypeId);  
            }           
        }

    }
}