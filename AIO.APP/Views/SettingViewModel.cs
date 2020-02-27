using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MaterialDesignExtensions.Controls;
using MVVMC;
using AIO.APP.Common.Utility;
using AIO.APP.Models;
using PeanutButter.INIFile;

namespace AIO.APP.Views
{
    public class SettingViewModel: MVVMCViewModel
    {
        #region parameter
        private string _outputPicDirStr=" Pictrue Folder Path";
        public string OutputPicDirStr
        {
            get { return _outputPicDirStr; }
            set
            {
                _outputPicDirStr = value;
                OnPropertyChanged();
            }
        }

       private string _LogDirStr="Logging Folder Path";
        public string LogDirStr
        {
            get { return _LogDirStr; }
            set
            {
                _LogDirStr = value;
                OnPropertyChanged();
            }
        }

       private string _loDaDirStr="Local Data Folder Path";
        public string LoDaDirStr
        {
            get { return _loDaDirStr; }
            set
            {
                _loDaDirStr = value;
                OnPropertyChanged();
            }
        }

        public string _remoteDbHostName;
        public string RemoteDbHostName
        {
            get { return _remoteDbHostName; }
            set
            {
                _remoteDbHostName = value;
                OnPropertyChanged();
            }
        }

        private string _remoteDbPort;
        public string RemoteDbPort
        {
            get { return _remoteDbPort; }
            set
            {
                _remoteDbPort = value;
                OnPropertyChanged();
            }
        }

       private string _remoteDbUserName;
        public string RemoteDbUserName
        {
            get { return _remoteDbUserName; }
            set
            {
                _remoteDbUserName = value;
                OnPropertyChanged();
            }
        }

        private string _remoteDbPassWord;
        public string RemoteDbPassWord
        {
            get { return _remoteDbPassWord; }
            set
            {
                _remoteDbPassWord = value;
                OnPropertyChanged();
            }
        } 
        private string _remoteDbTable;
        public string RemoteDbTable
        {
            get { return _remoteDbTable; }
            set
            {
                _remoteDbTable = value;
                OnPropertyChanged();
            }
        }

        #endregion


        #region command

        private ICommand _selectPictureDirectoryCommand { get; set; }

        public ICommand SelectPictureDirectoryCommand
        {
            get
            {
                if (_selectPictureDirectoryCommand == null)
                {
                    _selectPictureDirectoryCommand = new DelegateCommand(() => { SelectPictureDirectoryHandler(); });
                }

                return _selectPictureDirectoryCommand;
            }
        }

        private ICommand _openLoggingDirectoryCommand { get; set; }

        public ICommand OpenLoggingDirectoryCommand
        {
            get
            {
                if (_openLoggingDirectoryCommand == null)
                {
                    _openLoggingDirectoryCommand = new DelegateCommand(() => { OpenFolder(LogDirStr); });
                }

                return _openLoggingDirectoryCommand;
            }
        }

        private ICommand _openPictureDirectoryCommand { get; set; }

        public ICommand OpenPictureDirectoryCommand
        {
            get
            {
                if (_openPictureDirectoryCommand == null)
                {
                    _openPictureDirectoryCommand = new DelegateCommand(() => { OpenFolder(OutputPicDirStr); });
                }

                return _openPictureDirectoryCommand;
            }
        }

        private ICommand _openLocalDbDirectoryCommand { get; set; }

        public ICommand OpenLocalDbDirectoryCommand
        {
            get
            {
                if (_openLocalDbDirectoryCommand == null)
                {
                    _openLocalDbDirectoryCommand = new DelegateCommand(() => { OpenFolder(LoDaDirStr); });
                }

                return _openLocalDbDirectoryCommand;
            }
        }

        private void OpenFolder(string folderPath)
        {
            if (Directory.Exists(folderPath))
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    Arguments = folderPath,
                    FileName = "explorer.exe"
                };

                Process.Start(startInfo);
            }
            else
            {
                MessageBox.Show(string.Format("{0} Directory does not exist!", folderPath));
            }
        }

        private async void SelectPictureDirectoryHandler()
        {
            OpenDirectoryDialogArguments dialogArgs = new OpenDirectoryDialogArguments()
            {
                Width = 600,
                Height = 400,
                CreateNewDirectoryEnabled = true
            };

            var result = await OpenDirectoryDialog.ShowDialogAsync("RootDialog", dialogArgs);

            if (!result.Canceled)
            {
                OutputPicDirStr = result.DirectoryInfo.FullName;
            }
        }


        public ICommand _saveConfigCommand { get; set; }

        public ICommand SaveConfigCommand
        {
            get
            {
                if (_saveConfigCommand == null)
                {
                    _saveConfigCommand = new DelegateCommand(() =>
                    {
                        var inifile = new INIFile();

                        inifile.SetValue("RemoteDataBase", "HostName", RemoteDbHostName);
                        inifile.SetValue("RemoteDataBase", "Port", RemoteDbPort);
                        inifile.SetValue("RemoteDataBase", "User", RemoteDbUserName);
                        inifile.SetValue("RemoteDataBase", "Password", RemoteDbPassWord);
                        inifile.SetValue("RemoteDataBase", "Table", RemoteDbTable);
                        inifile.SetValue("OutputPicture", "Dir", OutputPicDirStr);

                        inifile.Persist("./config.ini");

                        InitialConfig.GetConfig();
                    });
                }

                return _saveConfigCommand;
            }
        }

        private ICommand _loadConfigCommand { get; set; }

        public ICommand LoadConfigCommand
        {
            get
            {
                if (_loadConfigCommand == null)
                {
                    _loadConfigCommand = new DelegateCommand(() =>
                    {
                        OutputPicDirStr = MasterPara.OutputPicDir;
                        RemoteDbHostName = MasterPara.RemoteDb.HostName;
                        RemoteDbPort = MasterPara.RemoteDb.Port.ToString();
                        RemoteDbUserName = MasterPara.RemoteDb.User;
                        RemoteDbPassWord = MasterPara.RemoteDb.Password;
                        RemoteDbTable = MasterPara.RemoteDb.Table;

                        LoDaDirStr = MasterPara.LocalDataDir;
                        LogDirStr = MasterPara.LoggingDir;
                    });
                }

                return _loadConfigCommand;
            }
        }

        #endregion
    }
}
