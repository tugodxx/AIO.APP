using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIO.APP.Models
{
   public static class MasterPara
    {
        public static string LocalDb { get; set; }

        public static SqlClass RemoteDb{ get; set; }

       public static string OutputPicDir { get; set; }
        public static string LoggingDir { get; set; }

       
        public static string LocalDataDir  { get; set; }
    }
}
