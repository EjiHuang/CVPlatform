using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace App_ui.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        #region Command

        /// <summary>
        /// 命令：显示关于对话框
        /// </summary>
        [Command]
        public void ShowAboutDialogCommand()
        {
            MessageBox.Show("Test");
        }

        #endregion
    }
}
