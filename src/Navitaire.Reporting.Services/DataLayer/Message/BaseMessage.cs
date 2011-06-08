using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Navitaire.Reporting.Services.DataLayer.Message
{
    public abstract class BaseMessage
    {
        private string _connection;
        private int _timeout = 30;

        public string Connection
        {
            get { return _connection; }
            set { _connection = value; }
        }

        public int Timeout
        {
            get { return _timeout; }
            set { _timeout = value; }
        }
    }
}
