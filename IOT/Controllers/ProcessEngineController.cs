using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Configuration;
using System.Web.Http;

namespace IOT.Controllers
{
    public class exedata
    {
        public string eventId { get; set; }
        public string eventData { get; set; }
    }
    public class ProcessEngineController : ApiController
    {
        // POST api/ProcessEngine/ExecuteDataEvent
        [Route("api/ProcessEngine/Initialize/{graphid}")]
        [HttpPost]
        public HttpResponseMessage Initialize(int graphid)
        {
            HttpRequestMessage request = new HttpRequestMessage();
            request.Properties.Add(System.Web.Http.Hosting.HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
            string RepositoryURL = WebConfigurationManager.AppSettings["RepositoryURL"];
            string DCRUserName = WebConfigurationManager.AppSettings["DCRUserName"];
            string DCRPassword = WebConfigurationManager.AppSettings["DCRPassword"];

            try
            {

                string getGraphXMLURL = string.Format("{0}/api/graphs/{1}/sims", RepositoryURL, graphid);
                using (HttpClient httpClient = new HttpClient())
                {
                    byte[] byteArray = Encoding.ASCII.GetBytes(DCRUserName + ":" + DCRPassword);
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                    StringContent sc = new StringContent(JsonConvert.SerializeObject(null), Encoding.UTF8, "application/json");
                    using (HttpResponseMessage res = httpClient.PostAsync(getGraphXMLURL, sc).GetAwaiter().GetResult())
                    {
                        System.Threading.Tasks.Task<string> entityData = res.Content.ReadAsStringAsync();

                        if (res.IsSuccessStatusCode)
                        {
                            string simid = ((string[])res.Headers.GetValues("X-DCR-simulation-ID"))[0];
                            return request.CreateResponse(HttpStatusCode.OK, simid);
                        }
                    }
                }
                return request.CreateResponse(HttpStatusCode.InternalServerError, "Something went wrong with request, please try again");

            }
            catch (Exception es)
            {


                return request.CreateResponse(HttpStatusCode.InternalServerError, "Something went wrong with request, please try again" + es.Message);
            }

        }

        // POST api/ProcessEngine/ExecuteDataEvent
        [Route("api/ProcessEngine/executeEvent/{graphid}/{simid}")]
        [HttpPost]
        public HttpResponseMessage executeEvent(int graphid, int simid, exedata obj)
        {
            HttpRequestMessage request = new HttpRequestMessage();
            request.Properties.Add(System.Web.Http.Hosting.HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
            string RepositoryURL = WebConfigurationManager.AppSettings["RepositoryURL"];
            string DCRUserName = WebConfigurationManager.AppSettings["DCRUserName"];
            string DCRPassword = WebConfigurationManager.AppSettings["DCRPassword"];

            try
            {
                string data = obj.eventData;
                string id = obj.eventId;
                string getGraphXMLURL = string.Format("{0}/api/graphs/{1}/sims/{2}/events/{3}", RepositoryURL, graphid, simid, id);
                using (HttpClient httpClient = new HttpClient())
                {
                    byte[] byteArray = Encoding.ASCII.GetBytes(DCRUserName + ":" + DCRPassword);
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    var vm = new { DataXML = data };
                    StringContent sc = new StringContent(JsonConvert.SerializeObject(vm), Encoding.UTF8, "application/json");
                    using (HttpResponseMessage res = httpClient.PostAsync(getGraphXMLURL, sc).GetAwaiter().GetResult())
                    {
                        System.Threading.Tasks.Task<string> entityData = res.Content.ReadAsStringAsync();

                        if (res.IsSuccessStatusCode)
                        {

                            return request.CreateResponse(HttpStatusCode.OK, simid);
                        }
                    }
                }
                return request.CreateResponse(HttpStatusCode.InternalServerError, "Something went wrong with request, please try again");

            }
            catch (Exception es)
            {


                return request.CreateResponse(HttpStatusCode.InternalServerError, "Something went wrong with request, please try again" + es.Message);
            }

        }




        // POST api/ProcessEngine/ExecuteDataEvent
        [Route("api/ProcessEngine/getevents/{graphid}/{simid}")]
        [HttpGet]
        public HttpResponseMessage GetEvents(int graphid, int simid)
        {
            HttpRequestMessage request = new HttpRequestMessage();
            request.Properties.Add(System.Web.Http.Hosting.HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
            string RepositoryURL = WebConfigurationManager.AppSettings["RepositoryURL"];
            string DCRUserName = WebConfigurationManager.AppSettings["DCRUserName"];
            string DCRPassword = WebConfigurationManager.AppSettings["DCRPassword"];

            try
            {

                string getGraphXMLURL = string.Format("{0}/api/graphs/{1}/sims/{2}/events?filter=only-pending", RepositoryURL, graphid, simid);
                using (HttpClient httpClient = new HttpClient())
                {
                    byte[] byteArray = Encoding.ASCII.GetBytes(DCRUserName + ":" + DCRPassword);
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                    StringContent sc = new StringContent(JsonConvert.SerializeObject(null), Encoding.UTF8, "application/json");
                    using (HttpResponseMessage res = httpClient.GetAsync(getGraphXMLURL).GetAwaiter().GetResult())
                    {
                        System.Threading.Tasks.Task<string> entityData = res.Content.ReadAsStringAsync();

                        if (res.IsSuccessStatusCode)
                        {
                            string dcrObj = entityData.Result.ToString();
                            return request.CreateResponse(HttpStatusCode.OK, dcrObj);
                        }
                    }
                }
                return request.CreateResponse(HttpStatusCode.InternalServerError, "Something went wrong with request, please try again");

            }
            catch (Exception es)
            {


                return request.CreateResponse(HttpStatusCode.InternalServerError, "Something went wrong with request, please try again" + es.Message);
            }

        }


    }
}
