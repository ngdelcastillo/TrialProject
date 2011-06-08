using System;

namespace Navitaire.Reporting.Services.Help
{
    public class ActionHelpMapTopic
    {
        #region Variables
        private string _helpTableOfContentsPath;
        private string _helpTopicPath;
        private string _productLine;
        private string _reportPath;
        #endregion

        #region Constructor
        public ActionHelpMapTopic(string helpTableOfContentsPath, string productLine, string reportPath, string helpTopicPath)
        {
            _helpTableOfContentsPath = helpTableOfContentsPath;
            _helpTopicPath = helpTopicPath;
            _productLine = productLine;
            _reportPath = reportPath;
        }
        #endregion

        #region Properties
        public string HelpTableOfContentsPath
        {
            get
            {
                return _helpTableOfContentsPath;
            }
        }

        public string HelpTopicPath
        {
            get
            {
                return _helpTopicPath;
            }
        }

        public string ProductLine
        {
            get
            {
                return _productLine;
            }
        }

        public string ReportPath
        {
            get
            {
                return _reportPath;
            }
        }
        #endregion
    }
}
