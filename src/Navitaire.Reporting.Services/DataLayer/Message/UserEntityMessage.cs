using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace Navitaire.Reporting.Services.DataLayer.Message
{
    public class UserEntityMessage : BaseMessage
    {
        #region Variables

        private Guid _userId = Guid.Empty;
        private string _domain;
        private string _username;
        private string _password;
        private Int64 _ncaUserId;
        private DateTime _ncaExpiration = SqlDateTime.MinValue.Value;
        private DateTime _created = SqlDateTime.MinValue.Value;
        private string _createdBy;
        private DateTime _modified = SqlDateTime.MinValue.Value;
        private string _modifiedBy;
        #endregion

        #region Properties
        public Guid UserId
        {
            get { return _userId;}
        }

        public String Domain
        {
            get { return _domain;}
            set { _domain = value;}
        }

        public String UserName
        {
            get { return _username; }
            set { _username = value; }
        }

        public String Password
        {
            get { return _password; }
            set { _password = value; }
        }

        public Int64 NCAUserId
        {
            get { return _ncaUserId; }
            set { _ncaUserId = value; }
        }

        public DateTime NCAExpiration
        {
            get { return _ncaExpiration; }
            set { _ncaExpiration = value; }
        }

        public DateTime Created
        {
            get { return _created; }
            set { _created = value; }
        }

        public String CreatedBy
        {
            get { return _createdBy; }
            set { _createdBy = value; }
        }

        public DateTime Modified
        {
            get { return _modified; }
            set { _modified = value; }
        }

        public System.String ModifiedBy
        {
            get { return _modifiedBy; }
            set { _modifiedBy = value;}
        }
        #endregion

        #region Constructors
        public UserEntityMessage()
        {
            try
            {
                base.Connection = ConfigurationManager.ConnectionStrings["NAVITAIRE-REPORTING"].ConnectionString;
            }
            catch { }
        }

        public UserEntityMessage(string connectionString)
        {
            base.Connection = connectionString;
        }
        public UserEntityMessage(SqlDataReader dr)
            : this()
        {
            LoadFromReader(dr);
        }
        #endregion

        #region Methods
        protected virtual void LoadFromReader( SqlDataReader dr )
		{
			if ( !dr.IsDBNull( 0 ) )
			{
				_userId = ( Guid ) dr[ 0 ];
			}

			if ( !dr.IsDBNull( 1 ) )
			{
				_domain = ( String ) dr[ 1 ];
			}

			if ( !dr.IsDBNull( 2 ) )
			{
				_username = ( String ) dr[ 2 ];
			}

			if ( !dr.IsDBNull( 3 ) )
			{
				_password = ( String ) dr[ 3 ];
			}

			if ( !dr.IsDBNull( 4 ) )
			{
				_ncaUserId = ( Int64 ) dr[ 4 ];
			}

			if ( !dr.IsDBNull( 5 ) )
			{
				_ncaExpiration = ( DateTime ) dr[ 5 ];
			}

			if ( !dr.IsDBNull( 6 ) )
			{
				_created = ( DateTime ) dr[ 6 ];
			}

			if ( !dr.IsDBNull( 7 ) )
			{
				_createdBy = ( String ) dr[ 7 ];
			}

			if ( !dr.IsDBNull( 8 ) )
			{
				_modified = ( DateTime ) dr[ 8 ];
			}

			if ( !dr.IsDBNull( 9 ) )
			{
				_modifiedBy = ( String ) dr[ 9 ];
			}

		}

        protected virtual void Delete(Guid userEntityID)
        {
            using (SqlConnection conn = new SqlConnection(Connection))
            {
                try
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            Delete(conn, trans, userEntityID);
                            trans.Commit();
                        }
                        catch
                        {
                            trans.Rollback();
                            throw;
                        }
                    }
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        protected virtual void Delete(SqlConnection conn, SqlTransaction trans, Guid userEntityID)
        {
            using (SqlCommand cmd = new SqlCommand("app.DeleteUserEntity", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = Timeout;
                cmd.Transaction = trans;

                cmd.Parameters.AddWithValue("@userEntityID", userEntityID);

                cmd.ExecuteScalar();
            }
        }

        protected virtual void Insert(string domain, string userName, string password, long NCAUserId, DateTime NCAExpiration, string createdBy)
        {
            using (SqlConnection conn = new SqlConnection(Connection))
            {
                try
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            Insert(conn, trans, domain, userName, password, NCAUserId, NCAExpiration, createdBy);
                            trans.Commit();
                        }
                        catch
                        {
                            trans.Rollback();
                            throw;
                        }
                    }
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        protected virtual void Insert(SqlConnection conn, SqlTransaction trans, string domain, string userName, string password, long NCAUserId, DateTime NCAExpiration, string createdBy)
        {
            using (SqlCommand cmd = new SqlCommand("app.InsertUserEntity", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = Timeout;
                cmd.Transaction = trans;

                cmd.Parameters.AddWithValue("@domain", domain);
                cmd.Parameters.AddWithValue("@userName", userName);
                cmd.Parameters.AddWithValue("@password", password);
                cmd.Parameters.AddWithValue("@NCAUserId", NCAUserId);
                cmd.Parameters.AddWithValue("@NCAExpiration", NCAExpiration);
                cmd.Parameters.AddWithValue("@createdBy", createdBy);

                _userId = (System.Guid)cmd.ExecuteScalar();
            }
        }

        protected virtual void Select(Guid userEntityID)
        {
            using (SqlConnection conn = new SqlConnection(Connection))
            {
                try
                {
                    conn.Open();
                    Select(conn, null, userEntityID);
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        protected virtual void Select(SqlConnection conn, SqlTransaction trans, Guid userEntityID)
        {
            using (SqlCommand cmd = new SqlCommand("app.SelectUserEntity", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = Timeout;
                if (trans != null)
                {
                    cmd.Transaction = trans;
                }

                cmd.Parameters.AddWithValue("@userEntityID", userEntityID);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    try
                    {
                        while (dr.Read())
                        {
                            LoadFromReader(dr);
                        }
                    }
                    finally
                    {
                        dr.Close();
                    }
                }
            }
        }

        protected virtual void Update(Guid userEntityID, string domain, string userName, string password, long NCAUserId, DateTime NCAExpiration, string modifiedBy)
        {
            using (SqlConnection conn = new SqlConnection(Connection))
            {
                try
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            Update(conn, trans, userEntityID, domain, userName, password, NCAUserId, NCAExpiration, modifiedBy);
                            trans.Commit();
                        }
                        catch
                        {
                            trans.Rollback();
                            throw;
                        }
                    }
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        protected virtual void Update(SqlConnection conn, SqlTransaction trans, Guid userEntityID, string domain, string userName, string password, long NCAUserId, DateTime NCAExpiration, string modifiedBy)
        {
            using (SqlCommand cmd = new SqlCommand("app.UpdateUserEntity", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = Timeout;
                cmd.Transaction = trans;

                cmd.Parameters.AddWithValue("@userEntityID", userEntityID);
                cmd.Parameters.AddWithValue("@domain", domain);
                cmd.Parameters.AddWithValue("@userName", userName);
                cmd.Parameters.AddWithValue("@password", password);
                cmd.Parameters.AddWithValue("@NCAUserId", NCAUserId);
                cmd.Parameters.AddWithValue("@NCAExpiration", NCAExpiration);
                cmd.Parameters.AddWithValue("@modifiedBy", modifiedBy);

                cmd.ExecuteScalar();
            }
        }
		#endregion

    }
}
