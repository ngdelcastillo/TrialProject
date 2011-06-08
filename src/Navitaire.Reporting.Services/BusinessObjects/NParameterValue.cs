using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.SqlServer.ReportingServices.ReportService2005;
using Navitaire.Reporting.Services;

namespace Navitaire.Reporting.Services.BusinessObjects
{
    public class NParameterValue
    {
        private string _label;
        private string _name;
        private string _value;
        private bool _useDefault;

        public bool UseDefault
        {
            get { return _useDefault; }
            set { _useDefault = value; }
        }
        private ReportParameterType _parameterType;

        public NParameterValue(string label, string name, string value, ReportParameterType parameterType, bool useDefault)
        {
            _label = label;
            _name = name;
            _value = value;
            _parameterType = parameterType;
            _useDefault = useDefault;
        }

        //public NParameterValue( ParameterValue parameterValue )
        //{
        //    _label = parameterValue.Label;
        //    _name = parameterValue.Name;
        //    _value = parameterValue.Value;
        //}

        public string Label
        {
            get
            {
                return _label;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public ReportParameterType ParameterType
        {
            get
            {
                return _parameterType;
            }
        }

        public string Value
        {
            get
            {
                return _value;
            }
        }
    }
}
