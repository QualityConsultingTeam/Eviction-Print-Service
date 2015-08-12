using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PrintingService
{
    public class PrintFormManager
    {
        const string PrintToolWebAPI = "http://23.23.243.227/PrintTool/";

        public static async Task<PrintForm> GetNextForm()
        {
            using (var client = new HttpClient() { BaseAddress = new Uri(PrintToolWebAPI) })
            {
                var response = await client.GetAsync("api/Print");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<PrintForm>();
                }
                else
                {
                    return null;
                }
            }
        }

        public static async Task<bool> DeleteForm(string id)
        {
            using (var client = new HttpClient() { BaseAddress = new Uri(PrintToolWebAPI) })
            {
                var response = await client.DeleteAsync("api/Print?id=" + id);

                return response.IsSuccessStatusCode;
            }
        }

        public static async void ResetStateForms()
        {
            using (var client = new HttpClient() { BaseAddress = new Uri(PrintToolWebAPI) })
            {
                await client.PutAsync("api/Print", null);
            }
        }
    }

    public class PrintForm
    {
        public string Id { get; set; }

        public string EvictionId { get; set; }

        public string FormName { get; set; }

        public string Copies { get; set; }

        public string Attorney { get; set; }

        public string UserId { get; set; }

        public string ParentUserId { get; set; }

        public string UserType { get; set; }

        public string ReIssueCheck { get; set; }

        public DateTime Date { get; set; }

        public string State { get; set; }

        public string FilePath { get; set; }
    }
}
