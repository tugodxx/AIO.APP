using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIO.APP.Models
{

    public class SqlClass
    {
        private string _hostName;

        public string HostName
        {
            get { return  _hostName; }
            set { _hostName = value; }
        }

        private string _port;

        public string Port
        {
            get { return _port; }
            set { _port = value; }
        }


        private string _user;

        public string User
        {
            get { return _user; }
            set { _user = value; }
        }


        private string _password;

        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        private string _table;

        public string Table
        {
            get { return _table; }
            set { _table = value; }
        }
    }
}
