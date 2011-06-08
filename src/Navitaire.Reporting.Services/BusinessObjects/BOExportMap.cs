using System;
using Navitaire.Reporting.Services.DataLayer.Message;
using Navitaire.Reporting.Services.Exceptions;

namespace Navitaire.Reporting.Services.BusinessObjects
{
    /// <summary>
    /// BOExportMap contains custom business logic associated with ExportMapMessage
    /// </summary>
    public class BOExportMap : ExportMapMessage
    {
        #region Constructors

        public BOExportMap()
            : base()
        {
        }

        /// <summary>
        /// This constructor is used by Collections, it is not intended to be publicly consumable
        /// </summary>
        /// <param name="dr">The DataReader used to fill the underlying Message type</param>
        internal BOExportMap(System.Data.SqlClient.SqlDataReader dr)
            : base(dr)
        {
        }

        #endregion Constructors

        public new string ProcName
        {
            get
            {
                return base.ProcName;
            }
            set
            {
                // Strip out any square brackets so we can find the proc in cases where the RDL 
                // doesn't follow the naming standard
                base.ProcName = value.Replace("[", null).Replace("]", null);
            }
        }

        public void Delete()
        {
            base.Delete(this.ExportMapId);
        }

        public void Save(string authenticatedUser)
        {
            #region Validate Parameters

            if (string.IsNullOrEmpty(this.ReportPath))
            {
                throw new InvalidValueException("A report must be specified in order to create a map");
            }

            if (string.IsNullOrEmpty(this.ProcName))
            {
                throw new InvalidValueException("A stored procedure must be specified in order to create a map");
            }

            if (string.IsNullOrEmpty(this.DataSourcePath))
            {
                throw new InvalidValueException("A data source must be specified in order to create a map");
            }

            #endregion Validate Parameters

            if (this.ExportMapId == Guid.Empty)
            {
                base.Insert(
                    this.ReportPath,
                    this.DataSourcePath,
                    this.ProcName,
                    this.DisplayOptions,
                    authenticatedUser);
            }
            else
            {
                base.Update(
                    this.ExportMapId,
                    this.ReportPath,
                    this.DataSourcePath,
                    this.ProcName,
                    this.DisplayOptions,
                    authenticatedUser);
            }
        }

        public void SaveTarget(string authenticatedUser, bool insert, Guid exportMapId)
        {
            #region Validate Parameters

            if (string.IsNullOrEmpty(this.ReportPath))
            {
                throw new InvalidValueException("A report must be specified in order to create a map");
            }

            if (string.IsNullOrEmpty(this.ProcName))
            {
                throw new InvalidValueException("A stored procedure must be specified in order to create a map");
            }

            if (string.IsNullOrEmpty(this.DataSourcePath))
            {
                throw new InvalidValueException("A data source must be specified in order to create a map");
            }

            #endregion Validate Parameters

            if (insert)
            {
                base.InsertTarget(
                    this.ReportPath,
                    this.DataSourcePath,
                    this.ProcName,
                    this.DisplayOptions,
                    authenticatedUser);
            }
            else
            {
                base.UpdateTarget(
                    exportMapId,
                    this.ReportPath,
                    this.DataSourcePath,
                    this.ProcName,
                    this.DisplayOptions,
                    authenticatedUser);
            }
        }
    }
}
