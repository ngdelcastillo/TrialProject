﻿using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Navitaire.Reporting.Services.BusinessObjects;

namespace Navitaire.Reporting.Services.DataLayer.CollectionGen
{
    public class ConfiguredRoleCollectionGen : Collection<BOConfiguredRole>
    {
        #region Declarations

        private string _connString;
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

		public ConfiguredRoleCollectionGen()
		{
            try{
			_connString = ConfigurationManager.ConnectionStrings["NAVITAIRE-REPORTING"].ConnectionString;
            }catch{}
        }
        #endregion Constructors

        #region Data Layer Methods

        protected virtual void SelectConfiguredRoleList()
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("app.SelectConfiguredRoleList", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = _commandTimeout;

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                this.Add(new BOConfiguredRole(dr));
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
