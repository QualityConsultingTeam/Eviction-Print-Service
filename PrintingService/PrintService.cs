using PrintingService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace PrintingWindowsService
{
    public partial class PrintService : ServiceBase
    {
        public PrintService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                PrintingThreadManager.Run();
            }
            catch(Exception ex)
            {
                ex.WriteLog();
            }
        }

        protected override void OnStop()
        {
        }
    }
}
