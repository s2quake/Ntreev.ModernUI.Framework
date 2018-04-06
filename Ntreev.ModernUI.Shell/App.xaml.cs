using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Ntreev.ModernUI.Shell
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            //var toolkitLicensePath = Environment.GetEnvironmentVariable("xceed_toolkit_license", EnvironmentVariableTarget.User);
            //if (File.Exists(toolkitLicensePath) == true)
            //{
            //    Xceed.Wpf.Toolkit.Licenser.LicenseKey = File.ReadAllText(toolkitLicensePath);
            //}
            //var datagridLicensePath = Environment.GetEnvironmentVariable("xceed_datagrid_license", EnvironmentVariableTarget.User);
            //if (File.Exists(datagridLicensePath) == true)
            //{
            //    Xceed.Wpf.DataGrid.Licenser.LicenseKey = File.ReadAllText(datagridLicensePath);
            //}
        }
    }
}
