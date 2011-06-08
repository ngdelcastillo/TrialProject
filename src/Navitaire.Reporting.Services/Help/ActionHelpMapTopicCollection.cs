using System;
using System.Collections.ObjectModel;
using System.Xml;

namespace Navitaire.Reporting.Services.Help
{
    public class ActionHelpMapTopicCollection : Collection<ActionHelpMapTopic>
    {
        #region Constructor
        public ActionHelpMapTopic this[string reportPath]
        {
            get
            {
                foreach (ActionHelpMapTopic actionHelpMapTopic in this)
                {
                    if (actionHelpMapTopic.ReportPath.Equals(reportPath, StringComparison.OrdinalIgnoreCase))
                    {
                        return actionHelpMapTopic;
                    }
                }
                return null;
            }
        }
        #endregion

        #region Method
        public void AddTopicsFromConfig(XmlDocument xDoc)
        {
            #region Validate Parameters
            if (xDoc == null)
            {
                throw new ArgumentNullException("xDoc");
            }
            #endregion Validate Parameters

            string helpTableOfContents = xDoc.DocumentElement.GetAttribute("tableOfContents");
            string helpProductLine = xDoc.DocumentElement.GetAttribute("productLine");

            XmlNodeList topicNodes = xDoc.SelectNodes("/ActionHelpMap/Topic");
            foreach (XmlNode topicNode in topicNodes)
            {
                XmlElement topicElement = topicNode as XmlElement;
                if (topicElement != null)
                {
                    string helpReportPath = topicElement.GetAttribute("reportPath");
                    string helpTopicPath = topicElement.GetAttribute("helpTopicPath");

                    if (this[helpReportPath] == null)
                    {
                        ActionHelpMapTopic helpTopic = new ActionHelpMapTopic(
                            helpTableOfContents,
                            helpProductLine,
                            helpReportPath,
                            helpTopicPath);

                        this.Add(helpTopic);
                    }
                }
            }
        }
        #endregion
    }
}
