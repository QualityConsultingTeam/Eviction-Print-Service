using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PrintingServiceInstaller
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void InstallBtn_Click(object sender, RoutedEventArgs e)
        {

            StartProcess(Servicename);
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void UninstallServiceBtn_Click(object sender, RoutedEventArgs e)
        {

            StartProcess("/u " + Servicename);
        }


        private bool VerifyFiles()
        {
            return System.IO.File.Exists(PathHelper.GetFullPath("InstallUtil.exe"))
                && System.IO.File.Exists(PathHelper.GetFullPath("PrintingWindowsService.exe"));
        }

        private void StartProcess(string Arguments)
        {
            ProcessStartInfo info = new ProcessStartInfo();

            if (!VerifyFiles())
            {
                MessageBox.Show(this, "Error al instalar el Servicio Windows verifique si existe en la carpeta la Herramienta InstallUtil y el servicio", "error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }



            info.WindowStyle = ProcessWindowStyle.Normal;
            info.FileName = PathHelper.GetFullPath("InstallUtil.exe");
            info.Arguments = Arguments;
            info.CreateNoWindow = false;
            info.UseShellExecute = false;
            info.RedirectStandardOutput = true;
            info.RedirectStandardError = true;

            Process process = new Process();
            process.StartInfo = info;
            process.Start();
            process.WaitForExit();

            var output = process.StandardOutput.ReadToEnd();
            MessageBox.Show(output);
        }

        private string Servicename = "\"PrintingWindowsService.exe\"";
    }
}
