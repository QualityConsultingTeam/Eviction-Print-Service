using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintingWindowsService
{
     public abstract  class  IPrintManager
    {

           public  virtual  void Start() { }
    }


    public interface IPrintService
    {
         bool GenerateDocument(PrintForm form);

        bool SendToPrint(PrintForm form,string printerName =null);
    }
}
