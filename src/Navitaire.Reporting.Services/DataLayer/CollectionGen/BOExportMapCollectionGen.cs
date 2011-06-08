using System;
using Navitaire.Reporting.Services.BusinessObjects;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace Navitaire.Reporting.Services.DataLayer.CollectionGen
{
    /// <summary>
    /// BOExportMapCollectionGen is an auto-generated collection of BOExportMap.
    /// This class has methods to fill the collection based on information
    /// in the Database layer.
    /// </summary>
    /// <remarks>
    /// NOTICE: The code in this file is auto-generated.  Any changes made to this file will be
    ///         lost the next time it is generated.  To add custom code onsider a Partial Class,
    ///         or add the custom code to a class that is derived from this base message type.
    /// </remarks>
    public class BOExportMapCollectionGen : Collection<BOExportMap>
    {
        #region Declarations

        private string _connString;
        private string _targetConnString;
        private int _commandTimeout = 30;

        #endregion Declarations

        #region Properties

        public string ConnectionString
        {
            get { return _connString; }
            set { _connString = value; }
        }
        public int CommandTimeout
        {
            get { return _commandTimeout; }
            set { _commandTimeout = value; }
        }

        #endregion Properties

        #region Constructors

        public BOExportMapCollectionGen()
        {
            try
            {
                _connString = ConfigurationManager.ConnectionStrings["NAVITAIRE-REPORTING"].ConnectionString;
                _targetConnString = ConfigurationManager.ConnectionStrings["NAVITAIRE-TARGETAPP"].ConnectionString;
            }
            catch { }
        }

        #endregion Constructors

        #region Data Layer Methods

        protected virtual void SelectExportMapList()
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("app.SelectExportMapList", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = _commandTimeout;

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                this.Add(new BOExportMap(dr));
                            }
                            dr.Close();
                        }
                    }
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        protected virtual void SelectTargetExportMapList()
        {
            using (SqlConnection conn = new SqlConnection(_targetConnString))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("app.SelectExportMapList", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = _commandTimeout;

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                this.Add(new BOExportMap(dr));
                            }
                            dr.Close();
                        }
                    }
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        #endregion Data Layer Methods
    }
}