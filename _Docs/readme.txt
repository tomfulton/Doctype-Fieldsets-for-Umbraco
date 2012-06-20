This package adds a layer of grouping beneath tabs, allowing you to group sets of properties into "fieldsets".  This can help provide a cleaner UI if your document type has a lot of properties.

STATUS
This package is in Beta - it was written very quickly for the CG12 Package Competition and has not had much TLC yet.  Please help out by testing and reporting any issues and I'll be glad to get them resolved!

USAGE
Simply install the package to get started.  When editing Document Type Properties, you'll see a new option for a Fieldset.  You can enter any name for the Fieldset, and any properties that have the same value will be grouped together.  Check out the screencast to see it in action.

This also works with master document types, you can even add a child property to a master's fieldset (note there is a small sorting issue at the moment)

TECHNICAL NOTES
During install, the package creates a config file (/config/DocumentTypeFieldsets.config) and adds a Config Section to your web.config.

The core of the package is two ApplicationBase classes - one that handles creating the fieldsets on the editContent page, and another that injects the editing controls onto the doctype editor.

All of the UI adjusting is done dynamically, so although it might be considered hacky, there is no change of damage to your site.  In the worst case, simply remove the Our.Umbraco.DocumentTypeFieldsets.dll file from your /bin/ to disable/remove the functionality.

COMPATIBILITY NOTES
Note, this package relies on hardcoded control IDs, therefore it's possible it may break if these change in a future Umbraco update.