using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PrintingService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
#if !DEBUG
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new PrintService()
            };
            ServiceBase.Run(ServicesToRun);
#else

            PrintingThreadManager.Run();

            //Hasta que dejamos de depurar
            Thread.Sleep(Timeout.Infinite);
#endif
        }
    }
}
