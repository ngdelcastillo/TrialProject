using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Navitaire.Reporting.Security
{
    public class Permission
    {
        #region Variables
        private string _securityNameSpace;
        private string _action;
        private bool _allowed;
        #endregion

        #region Constructors
        private Permission()
        { }

        public Permission(string securityNameSpace, string action, bool allowed)
        {
            _securityNameSpace = securityNameSpace;
            _action = action;
            _allowed = allowed;
        }

        public Permission(Permission template)
        {
            _securityNameSpace = template.SecurityNameSpace;
            _action = template.Action;
            _allowed = template.Allowed;
        }
        #endregion

        #region Property
        public string SecurityNameSpace
        {
            get
            {
                return _securityNameSpace;
            }
        }

        public string Action
        {
            get
            {
                return _action;
            }
        }

        public bool Allowed
        {
            get
            {
                return _allowed;
            }
        }

        public override string ToString()
        {
            return string.Format("{0} {1} - {2}", _securityNameSpace, _action, _allowed);
        }
        #endregion
    }
}
