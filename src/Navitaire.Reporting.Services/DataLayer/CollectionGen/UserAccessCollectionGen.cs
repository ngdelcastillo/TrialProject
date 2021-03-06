﻿using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Navitaire.Reporting.Services.BusinessObjects;

namespace Navitaire.Reporting.Services.DataLayer.CollectionGen
{
    public class UserAccessCollectionGen : Collection<BOUserAccess>
    {
        #region Declarations
        private string _connString;
        private int _commandTimeout = 30;

        #endregion

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
        #endregion

        #region Constructor
        public UserAccessCollectionGen()
        {
            try
            {
                _connString = ConfigurationManager.ConnectionStrings["NAVITAIRE-REPORTING"].ConnectionString;
            }
            catch { }
        }
        #endregion

        #region Methods
        protected virtual void SelectUserAccessList()
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("app.SelectUserAccessList", conn))
                    {
                        cmd.CommandTimeout = _commandTimeout;
                        cmd.CommandType = CommandType.StoredProcedure;

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                this.Add(new BOUserAccess(dr));
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
        #endregion
    }
}
