using System;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Navitaire.Reporting.Services.BusinessObjects
{
    public class NParameterValueCollection : Collection<NParameterValue>
    {
        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public NParameterValueCollection()
            : base()
        {
        }

        /// <summary>
        /// Copy Constructor to work with subscription parameters
        /// </summary>
        /// <param name="subscriptionSavedParameters"></param>
        public NParameterValueCollection(BOSubscriptionReportParameterCollection subscriptionSavedParameters)
            : base()
        {
            foreach (BOSubscriptionReportParameter rp in subscriptionSavedParameters)
            {
                this.Add(new NParameterValue(rp.ParameterName, rp.ParameterName, rp.ParameterValue, rp.ParameterType, rp.ParameterUseDefault));
            }
        }

        #endregion Constructors

        public NParameterValue this[string parameterName]
        {
            get
            {
                foreach (NParameterValue np in this)
                {
                    if (np.Name.Equals(parameterName, StringComparison.OrdinalIgnoreCase))
                    {
                        return np;
                    }
                }

                return null;
            }
        }
    }
}
