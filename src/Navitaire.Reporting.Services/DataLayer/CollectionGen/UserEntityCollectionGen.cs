using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Navitaire.Reporting.Services.BusinessObjects;

namespace Navitaire.Reporting.Services.DataLayer.CollectionGen
{
    public class UserEntityCollectionGen : Collection<BOUserEntity>
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
        #endregion

        #region Constructors

		public UserEntityCollectionGen()
		{
            try{
			_connString = ConfigurationManager.ConnectionStrings["NAVITAIRE-REPORTING"].ConnectionString;
            }catch{}
		}

		#endregion Constructors

		#region Data Layer Methods

		protected virtual void SelectUserEntityList(  )
		{
			using ( SqlConnection conn = new SqlConnection( _connString ) )
			{
				try
				{
					conn.Open();
					using ( SqlCommand cmd = new SqlCommand( "app.SelectUserEntityList", conn ) )
					{
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.CommandTimeout = _commandTimeout;

						using ( SqlDataReader dr = cmd.ExecuteReader() )
						{
							while ( dr.Read() )
							{
								this.Add( new BOUserEntity( dr ) );
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
