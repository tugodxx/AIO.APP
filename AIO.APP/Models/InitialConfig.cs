using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AIO.APP.Common.Utility;
using PeanutButter.INIFile;
namespace AIO.APP.Models
{
    public static class InitialConfig
    {
        
        public static void InitialFile()
        {
            if (!DirFile.IsExistFile("config.ini"))
            { 
                var inifile = new INIFile();

                inifile.AddSection("LocalDataBase");
                inifile.AddSection("RemoteDataBase");
                inifile.AddSection("OutputPicture");
                inifile.AddSection("Logging");
                inifile.AddSection("LocalData");

                inifile.SetValue("LocalDataBase","HostDir","DataBase");
                inifile.SetValue("RemoteDataBase","HostName","127.0.0.1");
                inifile.SetValue("RemoteDataBase","Port","1433");
                inifile.SetValue("RemoteDataBase","User","admin");
                inifile.SetValue("RemoteDataBase","Password","123456");
                inifile.SetValue("RemoteDataBase","Table","master");
                inifile.SetValue("OutputPicture","Dir",@"C:\Output");
                inifile.SetValue("Logging","Dir","Logging");
                inifile.SetValue("LocalData","Dir","LocalData");
                inifile.Persist("config.ini");
            }

            if (!DirFile.IsExistDirectory("./LocalData"))
            {
                DirFile.CreateDirectory("./LocalData");
            }

            
            if (!DirFile.IsExistDirectory("./Logging"))
            {
                DirFile.CreateDirectory("./Logging");
            }

            
            if (!DirFile.IsExistDirectory("./LocalData/DataBase"))
            {
                DirFile.CreateDirectory("./LocalData/DataBase");
            }

            if (!DirFile.IsExistDirectory("./LocalData/Picture"))
            {
                DirFile.CreateDirectory("./LocalData/Picture");
            }
        }


        public static void GetConfig()
        {
            var inifile = new INIFile("config.ini");
            var basepath = System.AppDomain.CurrentDomain.BaseDirectory;

            MasterPara.LocalDataDir = basepath + inifile.GetValue("LocalDataBase","HostDir");
            MasterPara.RemoteDb  = new SqlClass()
            {
                HostName = inifile.GetValue("RemoteDataBase","HostName","127.0.0.1"),
                Port = inifile.GetValue("RemoteDataBase","Port","1433"),
                User = inifile.GetValue("RemoteDataBase","User","admin"),
                Password = inifile.GetValue("RemoteDataBase","Password","123456"),
              Table = inifile.GetValue("RemoteDataBase","Table","master")
            };
            MasterPara.OutputPicDir=  inifile.GetValue("OutputPicture","Dir");
            MasterPara.LoggingDir = basepath + inifile.GetValue("Logging","Dir");
            MasterPara.LocalDataDir = basepath + inifile.GetValue("LocalData","Dir");
        }
    }
}
