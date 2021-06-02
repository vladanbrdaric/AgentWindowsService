using AgentService.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AgentService
{
    public class Api
    {
        private readonly string _pathToLoggingFile;

        public Api(string pathToLogFile)
        {
            _pathToLoggingFile = pathToLogFile;
        }
        // Call PrintController/GetPrintJob Api method.
        public async Task<PrintJobModel> GetPrintJobApiCall()
        {
            HttpClient _client = new HttpClient();
            //var _client = _httpClientFactory.CreateClient();


            var response = await _client.GetAsync("http://10.10.11.65:5000/api/Print");
            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var responseText = await response.Content.ReadAsStringAsync();

                File.AppendAllText(_pathToLoggingFile, $"{ DateTime.Now }  Data received from API: { responseText }" + Environment.NewLine);
                var printJob = JsonSerializer.Deserialize<PrintJobModel>(responseText, options);

                return printJob;
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }


        // Post to PrinterController/ProcessPrintJobStatusInfo
        public async Task PostPrintJobInfoApiCall(PrintJobModel printJob)
        {

            HttpClient _client = new HttpClient();
            //var _client = _httpClientFactory.CreateClient();

            Console.WriteLine($"Data be sent to the API: { printJob.PrinterName }, { printJob.PrintStatus }");
            File.AppendAllText(_pathToLoggingFile, $"{ DateTime.Now }  Data be sent to the API: { printJob.PrinterName }, { printJob.PrintStatus }" + Environment.NewLine);
            var response = await _client.PostAsync("http://10.10.11.65:5000/api/Print", new StringContent(JsonSerializer.Serialize(printJob), Encoding.UTF8, "application/json"));
        }


        // --------------------------------------------------------------------------------------------------------------------------------------------------------------------------


        // Call GetPrinter/GetPrinterUpTimeInfo
        public async Task<PrinterInfoModel> GetPrinterInfoApiCall()
        {
            HttpClient _client = new HttpClient();
            //var _client = _httpClientFactory.CreateClient();

            var response = await _client.GetAsync("http://10.10.11.65:5000/api/GetPrinter");



            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var responseText = await response.Content.ReadAsStringAsync();
                File.AppendAllText(_pathToLoggingFile, $"{ DateTime.Now }  Data received from API: { responseText }" + Environment.NewLine);

                var printerInfo = JsonSerializer.Deserialize<PrinterInfoModel>(responseText, options);

                return printerInfo;
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }

        // Post to GetPrinter/ProcessPrinterUpTimeInfo
        public async Task PostPrinterUpTimeInfoApiCall(PrinterInfoModel printer)
        {
            HttpClient _client = new HttpClient();
            //var _client = _httpClientFactory.CreateClient();

            File.AppendAllText(_pathToLoggingFile, $"{ DateTime.Now }  Data be sent to the API: { printer.Ip }" + Environment.NewLine);
            foreach (var p in printer.OidList)
            {
                File.AppendAllText(_pathToLoggingFile, $"   { p.Name }" + Environment.NewLine);
                File.AppendAllText(_pathToLoggingFile, $"   { p.Response }" + Environment.NewLine);
            }


            var response = await _client.PostAsync("http://10.10.11.65:5000/api/GetPrinter", new StringContent(JsonSerializer.Serialize(printer), Encoding.UTF8, "application/json"));
        }
    }
}
