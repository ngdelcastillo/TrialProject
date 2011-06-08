using System;

namespace Navitaire.Reporting.Services
{
    public enum AccessStateType : int
    {
        Allowed = 0,
        Denied = 1
    }

    public enum TraceLevel : int
    {
        None = 0,
        Standard = 1,
        Verbose = 2
    }

    public enum ReportParameterType : int
    {
        None = 0,
        Boolean = 1,
        DateTime = 2,
        Integer = 3,
        Float = 4,
        String = 5,
        DateMapping = 6 // This is a masking for the date used with the subscription        
    }
}
