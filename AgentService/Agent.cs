using AgentService.Database;
using AgentService.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Topshelf;

namespace AgentService
{
    public class Agent : IAgent
    {
        string pathToLoggingFile = @"C:\AgentLog\LogFile.txt";
        string serversUrl = "ws://10.10.11.65:5000";
        string databaseName  = "PrintJobs.db";


        public async Task StartAsync()
        {
            // Log information to the log file.
            File.AppendAllText(pathToLoggingFile, $"{ DateTime.Now }  >>> Service has been started <<<" + Environment.NewLine);

            while (true)
            {
                using (var webSocket = new ClientWebSocket())
                {
                    try
                    {
                        // Log information to the log file.
                        File.AppendAllText(pathToLoggingFile, $"{ DateTime.Now }  Traying to establish a connection with the server..." + Environment.NewLine);

                        // Establishing a connection with the server (API)
                        await webSocket.ConnectAsync(new Uri(serversUrl), CancellationToken.None);

                        // Log information to the log file.
                        File.AppendAllText(pathToLoggingFile, $"{ DateTime.Now }  Connection with the server has been established" + Environment.NewLine);

                        // Define buffer for the incoming data
                        byte[] buffer = new byte[1024 * 4];

                        // While connection is open
                        while (webSocket.State == WebSocketState.Open)
                        {
                            // Store incoming data into variable result
                            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                            if (result.MessageType != WebSocketMessageType.Close)
                            {
                                await MainMethod(buffer, result.Count, databaseName, pathToLoggingFile);

                                // Write to the console that connection is still open.
                                Console.WriteLine("Connection is still open. Waiting for the 'PING' signal from the API...");
                            }
                            else
                            {
                                // when the conncetion has been closed
                                await StopAsync();
                                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        // Logg exception information
                        File.AppendAllText(pathToLoggingFile, $"{ DateTime.Now }  Exception: { ex.Message }" + Environment.NewLine);

                        // Wait 4 seconds before trying to reach the server again.
                        Thread.Sleep(4 * 1000);
                    }
                }
            }



        }

        /// <summary>
        /// Loggs information that the service has been stoped and the websocket connection has been closed.
        /// </summary>
        /// <returns></returns>
        public async Task StopAsync()
        {
            // Log the information about closed
            File.AppendAllText(pathToLoggingFile, $"{ DateTime.Now }  >>> Service has been stopped and the connection has been closed <<<" + Environment.NewLine);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="count"></param>
        /// <param name="db"></param>
        /// <param name="loggingFilePath"></param>
        /// <returns></returns>
        private async Task MainMethod(byte[] buffer, int count, string databaseName, string loggingFilePath)
        {
            
            PrintJobModel printJob = new PrintJobModel();
            PrinterInfoModel printerInfo = new PrinterInfoModel();
            Api api = new Api(loggingFilePath);
            PingHost pingHost = new PingHost();
            SnmpData snmpData = new SnmpData();
            DatabaseAccess db = new DatabaseAccess();

            // Convert received data from bytes into string
            var receivedDataString = Encoding.UTF8.GetString(buffer, 0, count);


            // If it's '1' user want to print something
            if (receivedDataString == "print")
            {
                // Call method that get data from api.
                printJob = await api.GetPrintJobApiCall();


                // Create database / table if it doesn't exist
                if (File.Exists(databaseName) == false)
                {
                    await db.CreateDatabase();
                }

                // Save received print job to the database.
                await db.AddNewRecord(printJob);

                // Get last record ID from the db
                int getRecordId = db.GetLastRecordId();

                // Send "FileToBePrinted" to the printer and wait for the answer from the printer.
                var printJobStatus = pingHost.SendPingSignal(printJob);

                //// Update printJobStatus in the database.
                await db.UpdateLastAddedRecord(getRecordId, printJobStatus.PrintStatus);

                // It will send the data back to the API with an Api post request.
                await api.PostPrintJobInfoApiCall(printJobStatus);

            }
            // If it's '2' user want to get some info about the printer.
            else if (receivedDataString == "getinfo")
            {
                // Call method that get data from api.
                printerInfo = await api.GetPrinterInfoApiCall();

                // Run SNMP Get request(s) against provided IP address, snmpCommunity and Oid.
                var printer = snmpData.snmpGet(printerInfo);

                // It will send the data back to the API with an Api post request.
                await api.PostPrinterUpTimeInfoApiCall(printer);
            }
        }
    }
}
