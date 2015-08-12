using Printing.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintingWindowsService
{
    public class PrintManagerService : IPrintService
    {
        public bool GenerateDocument(PrintForm form)
        {

           

            var config = new PrintConfig()
            {
                EvictionIds = new string[] { form.EvictionId },
                Forms = new string[] { form.FormName },
                PrintCount = new string[] { form.Copies },
                SelectedAttorney = form.Attorney,
                UserId = form.UserId,
                ParentUserId = form.ParentUserId,
                TemplateFolder =ConfigurationSettings.AppSettings["TemplateFolder"],//  @"E:\CASS TFS\Eviction  Site\Eviction_v1.0-Printing Tool\website\nwe2.0",
                MappingFolder = ConfigurationSettings.AppSettings["MappingFolder"] ,
                UserType = form.UserType ,
                ReIssueCheck =!string.IsNullOrEmpty(form.ReIssueCheck )&& Convert.ToBoolean( form.ReIssueCheck),


            };

            try
            {
               //form.FilePath =  PrintingTool.Print(config);
               // conectar la forma en la que se genera el documento. 

                return !string.IsNullOrEmpty(form.FilePath);
            }
            catch (Exception ex)
            {
                ex.WriteLog();
                return false;
            }
        }

        
        public bool SendToPrint(PrintForm form,string printerName =null)
        {
            this.pdfFileName = form.FilePath;
            this.printerName = printerName;
            PrintManagerService.adobeReaderPath = ConfigurationSettings.AppSettings["adobeReaderPath"];// @"C:\Program Files (x86)\Adobe\Reader 11.0\Reader\AcroRd32.exe";
            var extension = Path.GetExtension(this.pdfFileName);
            
            return extension.ToLower().Contains(".txt") && UseDefaultPrinter ?
             PrintTextFile(): Print();
        }

        private bool UseDefaultPrinter
        {
            get
            {
                var value = ConfigurationSettings.AppSettings["useAdobeReader"];

                return !string.IsNullOrEmpty(value) && Convert.ToBoolean(value);
            }
        }

        #region Print Methods
        private bool PrintTextFile()
        {
             System.ComponentModel.Container components;
           
           
            try
            {
                streamToPrint = new StreamReader(this.pdfFileName);
                printFont = new Font("Arial", 10);
                PrintDocument pd = new PrintDocument();
                pd.PrintPage += new PrintPageEventHandler
                   (this.pd_PrintPage);
                pd.Print();
                return true;
            }
            catch(Exception ex)
            {
                ex.WriteLog();
                return false;
            }
            finally
            {
                streamToPrint.Close();
            }
        }

        private void pd_PrintPage(object sender, PrintPageEventArgs ev)
        {
            float linesPerPage = 0;
            float yPos = 0;
            int count = 0;
            float leftMargin = ev.MarginBounds.Left;
            float topMargin = ev.MarginBounds.Top;
            string line = null;

            // Calculate the number of lines per page.
            linesPerPage = ev.MarginBounds.Height /
               printFont.GetHeight(ev.Graphics);

            // Print each line of the file. 
            while (count < linesPerPage &&
               ((line = streamToPrint.ReadLine()) != null))
            {
                yPos = topMargin + (count *
                   printFont.GetHeight(ev.Graphics));
                ev.Graphics.DrawString(line, printFont, Brushes.Black,
                   leftMargin, yPos, new StringFormat());
                count++;
            }

            // If more lines exist, print another page. 
            if (line != null)
                ev.HasMorePages = true;
            else
                ev.HasMorePages = false;
        }

        public PrintManagerService()
        {

        }

        private Font printFont;

        private StreamReader streamToPrint;

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfFilePrinter"/> class.
        /// </summary>
        /// <param name="pdfFileName">Name of the PDF file.</param>
        public PrintManagerService(string pdfFileName)
        {
            this.PdfFileName = pdfFileName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfFilePrinter"/> class.
        /// </summary>
        /// <param name="pdfFileName">Name of the PDF file.</param>
        /// <param name="printerName">Name of the printer.</param>
        public PrintManagerService(string pdfFileName, string printerName)
        {
            this.pdfFileName = pdfFileName;
            this.printerName = printerName;
        }

        /// <summary>
        /// Gets or sets the name of the PDF file to print.
        /// </summary>
        public string PdfFileName
        {
            get { return this.pdfFileName; }
            set { this.pdfFileName = value; }
        }
        string pdfFileName;

        /// <summary>
        /// Gets or sets the name of the printer. A typical name looks like '\\myserver\HP LaserJet PCL5'.
        /// </summary>
        /// <value>The name of the printer.</value>
        public string PrinterName
        {
            get { return this.printerName; }
            set { this.printerName = value; }
        }
        string printerName;

        /// <summary>
        /// Gets or sets the working directory.
        /// </summary>
        public string WorkingDirectory
        {
            get { return this.workingDirectory; }
            set { this.workingDirectory = value; }
        }
        string workingDirectory;

        /// <summary>
        /// Prints the PDF file.
        /// </summary>
        public bool Print()
        {
            return Print(-1);
        }

        /// <summary>
        /// Prints the PDF file.
        /// </summary>
        /// <param name="milliseconds">The number of milliseconds to wait for completing the print job.</param>
        public bool Print(int milliseconds)
        {
            if (this.printerName == null || this.printerName.Length == 0)
                this.printerName = PrintManagerService.defaultPrinterName;

            if (PrintManagerService.adobeReaderPath == null || PrintManagerService.adobeReaderPath.Length == 0)
                throw new InvalidOperationException("No full qualified path to AcroRd32.exe or Acrobat.exe is set.");

            if (this.printerName == null || this.printerName.Length == 0)
                throw new InvalidOperationException("No printer name set.");

            // Check whether file exists.
            string fqName = String.Empty;
            if (this.workingDirectory != null && this.workingDirectory.Length != 0)
                fqName = Path.Combine(this.workingDirectory, this.pdfFileName);
            else
                fqName = Path.Combine(Directory.GetCurrentDirectory(), this.pdfFileName);
            if (!File.Exists(fqName))
                throw new InvalidOperationException(String.Format("The file {0} does not exist.", fqName));

            // TODO: Check whether printer exists.

            try
            {
                DoSomeVeryDirtyHacksToMakeItWork();

                //acrord32.exe /t          <- seems to work best
                //acrord32.exe /h /p       <- some swear by this version
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = PrintManagerService.adobeReaderPath;
                string args = String.Format("/t \"{0}\" \"{1}\"", this.pdfFileName, this.printerName);
                //Debug.WriteLine(args);
                startInfo.Arguments = args;
                startInfo.CreateNoWindow = true;
                startInfo.ErrorDialog = false;
                startInfo.UseShellExecute = false;
                if (this.workingDirectory != null && this.workingDirectory.Length != 0)
                    startInfo.WorkingDirectory = this.workingDirectory;

                Process process = Process.Start(startInfo);
                if (!process.WaitForExit(milliseconds))
                {
                    // Kill Adobe Reader/Acrobat
                    process.Kill();
                }
                return true;
            }
            catch (Exception ex)
            {
                //throw ex;

                //TODo log 
                ex.WriteLog();
                return false;
            }
        }

        /// <summary>
        /// For reasons only Adobe knows the Reader seams to open and shows the document instead of printing it
        /// when it was not already running.
        /// If you use PDFsharp and have any suggestions to circumvent this function, please let us know.
        /// </summary>
        void DoSomeVeryDirtyHacksToMakeItWork()
        {
            if (PrintManagerService.runningAcro != null)
            {
                if (!PrintManagerService.runningAcro.HasExited)
                    return;
                PrintManagerService.runningAcro.Dispose();
                PrintManagerService.runningAcro = null;
            }
            // Is any Adobe Reader/Acrobat running?
            Process[] processes = Process.GetProcesses();
            int count = processes.Length;
            for (int idx = 0; idx < count; idx++)
            {
                try
                {
                    Process process = processes[idx];
                    ProcessModule module = process.MainModule;

                    if (String.Compare(Path.GetFileName(module.FileName), Path.GetFileName(PrintManagerService.adobeReaderPath), true) == 0)
                    {
                        // Yes: Fine, we can print.
                        PrintManagerService.runningAcro = process;
                        break;
                    }
                }
                catch { }
            }
            if (PrintManagerService.runningAcro == null)
            {
                // No: Start an Adobe Reader/Acrobat.
                // If you are within ASP.NET, good luck...
                PrintManagerService.runningAcro = Process.Start(PrintManagerService.adobeReaderPath);
                PrintManagerService.runningAcro.WaitForInputIdle();
            }
        }
        static Process runningAcro;

        /// <summary>
        /// Gets or sets the Adobe Reader or Adobe Acrobat path.
        /// A typical name looks like 'C:\Program Files\Adobe\Adobe Reader 7.0\AcroRd32.exe'.
        /// </summary>
        static public string AdobeReaderPath
        {
            get { return PrintManagerService.adobeReaderPath; }
            set { PrintManagerService.adobeReaderPath = value; }
        }
        static string adobeReaderPath;

        /// <summary>
        /// Gets or sets the name of the default printer. A typical name looks like '\\myserver\HP LaserJet PCL5'.
        /// </summary>
        static public string DefaultPrinterName
        {
            get { return PrintManagerService.defaultPrinterName; }
            set { PrintManagerService.defaultPrinterName = value; }
        }
        static string defaultPrinterName;

        #endregion print Methods.
    }
}

