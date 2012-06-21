using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Our.Umbraco.DocTypeFieldsets.Extensions;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic;
using umbraco.cms.businesslogic.propertytype;
using umbraco.controls;
using umbraco.presentation.LiveEditing;
using umbraco.uicontrols;

namespace Our.Umbraco.DocTypeFieldsets.Events
{
    public class CreateFieldsets : ApplicationBase
    {
        private List<Control> _controls;

        public CreateFieldsets()
        {
            ContentControl.AfterContentControlLoad += new ContentControl.AfterContentControlLoadEventHandler(ContentControl_AfterContentControlLoad);
        }

        void ContentControl_AfterContentControlLoad(ContentControl contentControl, ContentControlLoadEventArgs e)
        {
            var contentType = contentControl.ContentObject.ContentType;

            var configuredProperties = DataHelper.GetPropertiesForContentType(contentType.Id);

            if (configuredProperties == null || !configuredProperties.Any() || configuredProperties.Count(x => !string.IsNullOrEmpty(x.Fieldset)) == 0)
                return;

            // Loop through configured fieldsets
            foreach (var fieldset in configuredProperties.Select(x => x.Fieldset).Distinct(StringComparer.CurrentCultureIgnoreCase))
            {
                _controls = new List<Control>();

                var currentFieldsetConfiguredProperties = configuredProperties.Where(g => string.Equals(g.Fieldset, fieldset, StringComparison.CurrentCultureIgnoreCase));

				foreach (var property in currentFieldsetConfiguredProperties)
                {
                    string propertyAlias = property.Alias;

                    // Find the associated control (ends with prop_propAlias)
                    var propertyControl = Utility.FindControl<Control>(
                            (Control c) => c.ClientID.EndsWith("_prop_" + propertyAlias), contentControl.Page);
                    
                    if (propertyControl != null)
                    {
                        // Get the control's container (Pane)
                        var containingPane = propertyControl.Parent.Parent.Parent;
                        _controls.Add(containingPane);
                    }
                }

                if (_controls.Any()) // TODO: shouldnt need this - seems like loop abve gets called many times??
                {
                    var insertAt = _controls.First().Parent;

                    // Create a panel to hold the new controls 
                    var ph = new Panel() { CssClass = "fieldset" };

                    foreach (var control in _controls)
                    {
                        var newControl = control;

                        // Remove the control from existing place
                        control.Parent.Controls.Remove(control);

                        // Add to new parent
                        ph.Controls.Add(newControl);
                    }

                    var fieldsetControl = GetFieldset(ph, fieldset);
                    insertAt.Controls.Add(fieldsetControl);
                }
            }
        }

        private Control GetFieldset(Panel controls, string fieldset)
        {
            var outerPanel = new Panel();
            var fieldsetPane = new Pane { ID = fieldset };

            // Remove the Pane from each control
            Panel controlsStripped = new Panel();
            foreach (Pane controlPane in controls.Controls)
            {
                var innerControl = controlPane.Controls[0];
                controlsStripped.Controls.Add(innerControl);
            }

            fieldsetPane.Controls.Add(controlsStripped);
            outerPanel.Controls.Add(fieldsetPane);
            outerPanel.Controls.AddAt(0, new LiteralControl("<h2 class='propertypaneTitel' style='text-align:left;margin-top:14px;'>" + fieldset + "</h2>"));

            return outerPanel;
        }
    }
}