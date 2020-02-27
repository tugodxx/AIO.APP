using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using AIO.APP.Models;

namespace AIO.APP
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {

        void app_Startup(object sender, StartupEventArgs e)
        {
            InitialConfig.InitialFile();
            InitialConfig.GetConfig();
        }

    }
}
