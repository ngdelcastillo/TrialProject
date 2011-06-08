using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Navitaire.Reporting.Services.DataLayer.CollectionGen;

namespace Navitaire.Reporting.Services.BusinessObjects
{
    /// <summary>
    /// BOSubscriptionReportParameterCollection contains custom business logic associated with BOSubscriptionReportParameterCollectionGen
    /// </summary>
    public class BOSubscriptionReportParameterCollection : BOSubscriptionReportParameterCollectionGen
    {
        public BOSubscriptionReportParameter this[Guid subscriptionReportParameterId]
        {
            get
            {
                foreach (BOSubscriptionReportParameter reportParameter in this)
                {
                    if (reportParameter.SubscriptionReportParameterId.Equals(subscriptionReportParameterId))
                    {
                        return reportParameter;
                    }
                }

                return null;
            }
        }

        public BOSubscriptionReportParameter this[string parameterName]
        {
            get
            {
                foreach (BOSubscriptionReportParameter reportParameter in this)
                {
                    if (reportParameter.ParameterName.Equals(parameterName, StringComparison.OrdinalIgnoreCase))
                    {
                        return reportParameter;
                    }
                }

                return null;
            }
        }

        public void Fill()
        {
            this.Clear();

            this.SelectSubscriptionReportParameterList();
        }

        public void FillTarget()
        {
            this.Clear();
            base.SelectTargetSubscriptionReportParameterList();
        }

        public void Fill(Guid subscriptionId)
        {

            this.Clear();

            this.SelectSubscriptionReportParameterListBySubscriptionId(subscriptionId);
        }

        public void Fill(SqlConnection conn, SqlTransaction trans, Guid subscriptionId)
        {

            this.Clear();

            this.SelectSubscriptionReportParameterListBySubscriptionId(conn, trans, subscriptionId);
        }

        public bool Remove(string parameterName)
        {
            BOSubscriptionReportParameter theParameter = this[parameterName];
            if (theParameter != null)
            {
                return this.Remove(theParameter);
            }

            return true;
        }

        public void Save(SqlConnection conn, SqlTransaction trans, Guid subscriptionId, string authenticatedUser)
        {
            // Delete any items that are present on the server, but are not present locally
            BOSubscriptionReportParameterCollection parametersInDb = new BOSubscriptionReportParameterCollection();
            parametersInDb.Fill(conn, trans, subscriptionId);

            for (int ndx = 0; ndx < parametersInDb.Count; ndx++)
            {
                BOSubscriptionReportParameter currentParameter = parametersInDb[ndx];
                if (this[currentParameter.SubscriptionReportParameterId] == null)
                {
                    currentParameter.Delete(conn, trans);
                }
            }

            // Save any items that haven't been saved to the database yet.
            for (int fileNdx = 0; fileNdx < this.Count; fileNdx++)
            {
                BOSubscriptionReportParameter currentParameter = this[fileNdx];
                currentParameter.SubscriptionId = subscriptionId;

                currentParameter.Save(conn, trans, authenticatedUser);
            }
        }
    }
}
