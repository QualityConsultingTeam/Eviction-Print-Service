using domain;
using service.nwe.config;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PrintingWindowsService
{
    public class PrintingThreadManager : IPrintManager
    {


        public static void Run()
        {
            Task.Run(() =>
            {
                Startup.initialization();

                // Preload static data
                Preloader loader = new Preloader();
                loader.execute();


                FetchProcess = new Thread(FetchFromApiProcess) { Priority = ThreadPriority.Normal, IsBackground = true };
                FetchProcess.Start();

                DocumentBuilderProcess = new Thread(DocumentBuildProcess) { Priority = ThreadPriority.Normal, IsBackground = true };
                DocumentBuilderProcess.Start();

                PrintProcess = new Thread(PrintingProcess) { Priority = ThreadPriority.Normal, IsBackground = true };
                PrintProcess.Start();
            });
        }

        public override void Start()
        {
            
            
        }

        #region Thread Loops Process

        /// <summary>
        /// Recupera los Documentos de la Web Api
        /// </summary>
        private static async void FetchFromApiProcess()
        {
            

            //mientras esta recuperando de la web Api los documentos a imprimir

            if (await GetNext() == null) PrintFormManager.ResetStateForms();
            
            while (IsRunning)
            {
                await GetNext();

                Thread.Sleep(TimeSpan.FromSeconds(SleepingTime));

            }
        }

        private static async  Task<PrintForm> GetNext()
        {
            var document = await PrintFormManager.GetNextForm();
            if (document != null && !DocumentBuildStack.ContainsKey(document.Id)) DocumentBuildStack.TryAdd(document.Id, document);

            return document;
        }


        /// <summary>
        /// Proceso de Impresion de documentos
        /// </summary>
        private static async void PrintingProcess()
        {
            

            while (IsRunning)
            {
                var document = DocumentPrintStack.FirstOrDefault();

                if (document.Value != null)
                {
                     
                    var service = new PrintManagerService();

                    if (service.SendToPrint(document.Value, "Enviar a OneNote 2013"))
                    {

                        // Eliminar y cambiar estado en la web api
                        await PrintFormManager.DeleteForm(document.Key);

                        // Eliminar en cola local
                        PrintForm form;
                        DocumentPrintStack.TryRemove(document.Key, out form);

                       
                    }

                }
                else Thread.Sleep(TimeSpan.FromSeconds(SleepingTime));
            }
        }

        /// <summary>
        /// Proceso de Creacion de Documentos
        /// </summary>
        private static void DocumentBuildProcess()
        {
            SetDefaultCulture();

            while (IsRunning)
            {

                var document = DocumentBuildStack.FirstOrDefault();

                if (document.Value != null)
                {
                    var service = new PrintManagerService();

                    if (service.GenerateDocument(document.Value))
                    {
                        // Si genera El documento en la carpeta temporal local 
                        PrintForm form;
                        DocumentBuildStack.TryRemove(document.Key, out form);

                        DocumentPrintStack.TryAdd(document.Key, document.Value);
                    }
                }
                else  Thread.Sleep(TimeSpan.FromSeconds(SleepingTime));

            }
        }


        private static void SetDefaultCulture()
        {
            var culture = new CultureInfo("en-US");

            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }
        #endregion

        #region Private Properties
        private static volatile bool IsRunning = true;

        private static ConcurrentDictionary<string, PrintForm> DocumentPrintStack = new ConcurrentDictionary<string, PrintForm>();
        private static ConcurrentDictionary<string, PrintForm> DocumentBuildStack = new ConcurrentDictionary<string, PrintForm>();

        private const int SleepingTime = 2; // Segundos!
        private const int SleepingTimeInitializing = 2; // Minutos !

        private static Thread FetchProcess;
        private static Thread DocumentBuilderProcess;
        private static Thread PrintProcess;
        #endregion  
    }
}
