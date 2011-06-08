using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace Navitaire.Reporting.Services.DataLayer.Message
{
    public class ConfigurationMessage : BaseMessage
    {
        #region Variables
        private Guid _configurationId = Guid.Empty;
        private String _propertyName;
        private String _propertyDescription;
        private String _propertyValue;
        private Byte _behavior;
        private DateTime _created = SqlDateTime.MinValue.Value;
        private String _createdBy;
        private DateTime _modified = SqlDateTime.MinValue.Value;
        private String _modifiedBy;
        #endregion

        #region Properties
        public Guid ConfigurationId
		{
			get 
			{
				return _configurationId;
			}
		}

		public String PropertyName
		{
			get 
			{
				return _propertyName;
			}
			set 
			{
				_propertyName = value;
			}
		}

		public String PropertyDescription
		{
			get 
			{
				return _propertyDescription;
			}
			set 
			{
				_propertyDescription = value;
			}
		}

		public String PropertyValue
		{
			get 
			{
				return _propertyValue;
			}
			set 
			{
				_propertyValue = value;
			}
		}

		public Byte Behavior
		{
			get 
			{
				return _behavior;
			}
			set 
			{
				_behavior = value;
			}
		}

		public DateTime Created
		{
			get 
			{
				return _created;
			}
			set 
			{
				_created = value;
			}
		}

		public String CreatedBy
		{
			get 
			{
				return _createdBy;
			}
			set 
			{
				_createdBy = value;
			}
		}

		public DateTime Modified
		{
			get 
			{
				return _modified;
			}
			set 
			{
				_modified = value;
			}
		}

		public String ModifiedBy
		{
			get 
			{
				return _modifiedBy;
			}
			set 
			{
				_modifiedBy = value;
			}
		}

		#endregion

        #region Constructors

        public ConfigurationMessage()
        {
            try
            {
                Connection = ConfigurationManager.ConnectionStrings["NAVITAIRE-REPORTING"].ConnectionString;
            }
            catch { }
        }

        public ConfigurationMessage(string connectionString)
        {
            Connection = connectionString;
        }
        public ConfigurationMessage(SqlDataReader dr)
            : this()
        {
            LoadFromReader(dr);
        }

        #endregion Constructors
        #region Private Helper Methods

        protected virtual void LoadFromReader(SqlDataReader dr)
        {
            if (!dr.IsDBNull(0))
            {
                _configurationId = (Guid)dr[0];
            }

            if (!dr.IsDBNull(1))
            {
                _propertyName = (String)dr[1];
            }

            if (!dr.IsDBNull(2))
            {
                _propertyDescription = (String)dr[2];
            }

            if (!dr.IsDBNull(3))
            {
                _propertyValue = (String)dr[3];
            }

            if (!dr.IsDBNull(4))
            {
                _behavior = (Byte)dr[4];
            }

            if (!dr.IsDBNull(5))
            {
                _created = (DateTime)dr[5];
            }

            if (!dr.IsDBNull(6))
            {
                _createdBy = (String)dr[6];
            }

            if (!dr.IsDBNull(7))
            {
                _modified = (DateTime)dr[7];
            }

            if (!dr.IsDBNull(8))
            {
                _modifiedBy = (String)dr[8];
            }

        }

        #endregion Private Helper Methods

        #region Data Layer Methods

        protected virtual void Delete(Guid configurationId)
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
                            Delete(conn, trans, configurationId);
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
        protected virtual void Delete(SqlConnection conn, SqlTransaction trans, Guid configurationId)
        {
            using (SqlCommand cmd = new SqlCommand("app.DeleteConfiguration", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = Timeout;
                cmd.Transaction = trans;

                cmd.Parameters.AddWithValue("@configurationId", configurationId);

                cmd.ExecuteScalar();
            }
        }

        protected virtual void Insert(string propertyName, string propertyDescription, string propertyValue, short behavior, string createdBy)
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
                            Insert(conn, trans, propertyName, propertyDescription, propertyValue, behavior, createdBy);
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
        protected virtual void Insert(SqlConnection conn, SqlTransaction trans, string propertyName, string propertyDescription, string propertyValue, short behavior, string createdBy)
        {
            using (SqlCommand cmd = new SqlCommand("app.InsertConfiguration", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = Timeout;
                cmd.Transaction = trans;

                cmd.Parameters.AddWithValue("@propertyName", propertyName);
                cmd.Parameters.AddWithValue("@propertyDescription", propertyDescription);
                cmd.Parameters.AddWithValue("@propertyValue", propertyValue);
                cmd.Parameters.AddWithValue("@behavior", behavior);
                cmd.Parameters.AddWithValue("@createdBy", createdBy);

                _configurationId = (System.Guid)cmd.ExecuteScalar();
            }
        }

        protected virtual void Select(Guid configurationId)
        {
            using (SqlConnection conn = new SqlConnection(Connection))
            {
                try
                {
                    conn.Open();
                    Select(conn, null, configurationId);
                }
                finally
                {
                    conn.Close();
                }
            }
        }
        protected virtual void Select(SqlConnection conn, SqlTransaction trans, Guid configurationId)
        {
            using (SqlCommand cmd = new SqlCommand("app.SelectConfiguration", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = Timeout;
                if (trans != null)
                {
                    cmd.Transaction = trans;
                }

                cmd.Parameters.AddWithValue("@configurationId", configurationId);

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

        protected virtual void Update(Guid configurationId, string propertyName, string propertyDescription, string propertyValue, short behavior, string modifiedBy)
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
                            Update(conn, trans, configurationId, propertyName, propertyDescription, propertyValue, behavior, modifiedBy);
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
        protected virtual void Update(SqlConnection conn, SqlTransaction trans, Guid configurationId, string propertyName, string propertyDescription, string propertyValue, short behavior, string modifiedBy)
        {
            using (SqlCommand cmd = new SqlCommand("app.UpdateConfiguration", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = Timeout;
                cmd.Transaction = trans;

                cmd.Parameters.AddWithValue("@configurationId", configurationId);
                cmd.Parameters.AddWithValue("@propertyName", propertyName);
                cmd.Parameters.AddWithValue("@propertyDescription", propertyDescription);
                cmd.Parameters.AddWithValue("@propertyValue", propertyValue);
                cmd.Parameters.AddWithValue("@behavior", behavior);
                cmd.Parameters.AddWithValue("@modifiedBy", modifiedBy);

                cmd.ExecuteScalar();
            }
        }

        #endregion Data Layer Methods
    }
}
