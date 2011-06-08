using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

using Microsoft.SqlServer.ReportingServices.ReportService2005;


namespace Navitaire.Reporting.Services.Catalog
{
    public class NReportParameter
    {
        #region Private Variables
        private ReportParameterType _parameterType;
        private Collection<NValidValue> _validValues = new Collection<NValidValue>();
        private StringCollection _defaultValues = new StringCollection();
        private StringCollection _dependencies = new StringCollection();

        #endregion

        #region Properties
        public string Prompt { get; set; }
        public string Name { get; set; }
        public string SelectedValue { get; set; }
        public bool MultiValue { get; set; }
        public bool MultiValueSpecified { get; set; }
        public bool AllowBlank { get; set; }

        public StringCollection DefaultValues
        {
            get
            {
                return _defaultValues;
            }
        }

        public StringCollection Dependencies
        {
            get
            {
                return _dependencies;
            }
        }

        public bool Hidden
        {
            get
            {
                return string.IsNullOrEmpty(Prompt);
            }
        }

        public ReportParameterType ParameterType
        {
            get
            {
                return _parameterType;
            }
        }

        public Collection<NValidValue> ValidValues
        {
            get
            {
                return _validValues;
            }
        }
        #endregion
               
        #region Constructors
        public NReportParameter(ReportParameter reportParameter)
        {
            Prompt = reportParameter.Prompt;
            Name = reportParameter.Name;
            MultiValue = reportParameter.MultiValue;
            MultiValueSpecified = reportParameter.MultiValueSpecified;
            AllowBlank = reportParameter.AllowBlank;

            switch (reportParameter.Type)
            {
                case ParameterTypeEnum.Boolean:
                    _parameterType = ReportParameterType.Boolean;
                    break;
                case ParameterTypeEnum.DateTime:
                    _parameterType = ReportParameterType.DateTime;
                    break;
                case ParameterTypeEnum.Float:
                    _parameterType = ReportParameterType.Float;
                    break;
                case ParameterTypeEnum.Integer:
                    _parameterType = ReportParameterType.Integer;
                    break;
                case ParameterTypeEnum.String:
                    _parameterType = ReportParameterType.String;
                    break;
            }

            if (reportParameter.ValidValues != null)
            {
                foreach (ValidValue vv in reportParameter.ValidValues)
                {
                    _validValues.Add(new NValidValue(vv));
                }
            }

            if (reportParameter.Dependencies != null)
            {
                foreach (string dep in reportParameter.Dependencies)
                {
                    _dependencies.Add(dep);
                }
            }

            if (reportParameter.DefaultValues != null)
            {
                foreach (string def in reportParameter.DefaultValues)
                {
                    _defaultValues.Add(def);
                }
            }
        }

        #endregion
    }
}
