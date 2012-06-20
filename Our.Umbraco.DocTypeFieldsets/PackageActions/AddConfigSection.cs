using System;
using System.Configuration;
using System.IO;
using System.Web.Configuration;
using System.Xml;
using Our.Umbraco.DocTypeFieldsets.Configuration;
using Our.Umbraco.DocTypeFieldsets.Extensions;
using umbraco.BusinessLogic;
using umbraco.IO;
using umbraco.cms.businesslogic.packager.standardPackageActions;
using umbraco.interfaces;

namespace Our.Umbraco.DocTypeFieldsets.PackageActions
{
    /// <summary>
    /// This package action will Add a new HTTP Module to the web.config file.
    /// </summary>
    /// <remarks>
    /// This package action has been customized from the PackageActionsContrib Project.
    /// http://packageactioncontrib.codeplex.com
    /// </remarks>
    public class AddConfigSection : IPackageAction
    {
        /// <summary>
        /// This Alias must be unique and is used as an identifier that must match the alias in the package action XML.
        /// </summary>
        /// <returns>The Alias of the package action.</returns>
        public string Alias()
        {
            return string.Concat(Common.ConfigName, "_AddConfigSection");
        }

        /// <summary>
        /// Executes the specified package name.
        /// </summary>
        /// <param name="packageName">Name of the package.</param>
        /// <param name="xmlData">The XML data.</param>
        /// <returns></returns>
        public bool Execute(string packageName, XmlNode xmlData)
        {
            //try
            //{
                var webConfig = WebConfigurationManager.OpenWebConfiguration("~/");
                if (webConfig.Sections[Common.ConfigName] == null)
                {
					//webConfig.SaveAs("web.config.bak");

                    webConfig.Sections.Add(Common.ConfigName, new ConfigSection());

                    string configPath = string.Concat("config", Path.DirectorySeparatorChar, Common.ConfigName, ".config");
                    string xmlPath = IOHelper.MapPath(string.Concat("~/", configPath));
                    string xml;

                    using (var reader = new StreamReader(xmlPath))
                    {
                        xml = reader.ReadToEnd();
                    }

                    webConfig.Sections[Common.ConfigName].SectionInformation.SetRawXml(xml);
                    webConfig.Sections[Common.ConfigName].SectionInformation.ConfigSource = configPath;
                    webConfig.Sections[Common.ConfigName].SectionInformation.RestartOnExternalChanges = false;

                    webConfig.Save(ConfigurationSaveMode.Modified);
                }

                return true;
//            }
//            catch (Exception ex)
//            {
//                string message = string.Concat("Error at install ", this.Alias(), " package action: ", ex);
//                Log.Add(LogTypes.Error, -1, message);
//            }

            //return false;
        }

        /// <summary>
        /// Returns a Sample XML Node
        /// </summary>
        /// <returns>The sample xml as node</returns>
        public XmlNode SampleXml()
        {
            string xml = string.Concat("<Action runat=\"install\" undo=\"true\" alias=\"", this.Alias(), "\" />");
            return helper.parseStringToXmlNode(xml);
        }

        /// <summary>
        /// Undoes the specified package name.
        /// </summary>
        /// <param name="packageName">Name of the package.</param>
        /// <param name="xmlData">The XML data.</param>
        /// <returns></returns>
        public bool Undo(string packageName, XmlNode xmlData)
        {
            try
            {
                var webConfig = WebConfigurationManager.OpenWebConfiguration("~/");
                if (webConfig.Sections[Common.ConfigName] != null)
                {
                    webConfig.Sections.Remove(Common.ConfigName);

                    webConfig.Save(ConfigurationSaveMode.Modified);
                }

                return true;
            }
            catch (Exception ex)
            {
                string message = string.Concat("Error at undo ", this.Alias(), " package action: ", ex);
                Log.Add(LogTypes.Error, -1, message);
            }

            return false;
        }
    }
}