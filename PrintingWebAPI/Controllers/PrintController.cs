using Amazon.SimpleDB;
using Amazon.SimpleDB.Model;
using environment;
using PrintingWebAPI.Util;
using service.nwe.printing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PrintingWebAPI.Controllers
{
    public class PrintController : ApiController
    {
        public IHttpActionResult Get()
        {
            EnvironmentManager.getInstance().setConfig(new WebAPIEnvironmentConfig());

            PrintForm printForm = PrintFormRepository.getInstance().byQuery("select * from `nwe.dev.printforms` where state='Ready' limit 1").FirstOrDefault();

            if (printForm == null)
                return NotFound();

            printForm.State = "Processing";

            PrintFormRepository.getInstance().update(printForm);

            return Ok<PrintForm>(printForm);
        }

        public IHttpActionResult Put()
        {
            EnvironmentManager.getInstance().setConfig(new WebAPIEnvironmentConfig());

            var forms = PrintFormRepository.getInstance().byQuery("select * from `nwe.dev.printforms` where state='Processing'").ToList();

            foreach (var f in forms)
            {
                f.State = "Ready";

                PrintFormRepository.getInstance().update(f);
            }

            return Ok();
        }

        public IHttpActionResult Delete(string id)
        {
            EnvironmentManager.getInstance().setConfig(new WebAPIEnvironmentConfig());

            PrintForm printForm = PrintFormRepository.getInstance().byId(id);

            if (printForm == null)
                return NotFound();

            PrintFormRepository.getInstance().deletePrintForm(printForm);

            return Ok();
        }
    }
}
