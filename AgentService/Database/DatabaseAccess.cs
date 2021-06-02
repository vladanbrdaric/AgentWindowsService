using AgentService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgentService.Database
{
    public class DatabaseAccess
    {
        // Create Database / Table
        public async Task CreateDatabase()
        {
            using (var db = new SqliteDbContext())
            {
                try
                {
                    await db.Database.EnsureCreatedAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                //await db.SaveChangesAsync();
            }
        }


        // Add new entry to the DB
        public async Task AddNewRecord(PrintJobModel printJob)
        {
            using (var db = new SqliteDbContext())
            {
                await db.PrintJob.AddAsync(printJob);
                await db.SaveChangesAsync();
            }
        }


        // Get last record ID
        public int GetLastRecordId()
        {
            int getRecordId = 0;

            using (var db = new SqliteDbContext())
            {
                getRecordId = db.PrintJob.OrderByDescending(r => r.JobId).FirstOrDefault().JobId;
            }

            return getRecordId;
        }


        // Get last record
        public PrintJobModel GetLastRecord()
        {
            PrintJobModel printJob = new PrintJobModel();
            using (var db = new SqliteDbContext())
            {
                printJob = db.PrintJob.OrderByDescending(r => r.JobId).FirstOrDefault();
            }

            return printJob;
        }


        // Update existing entry
        public async Task UpdateLastAddedRecord(int lastRecordId, string printJobStatus)
        {
            using (var db = new SqliteDbContext())
            {
                var job = db.PrintJob.Where(j => j.JobId == lastRecordId).FirstOrDefault();
                job.PrintStatus = printJobStatus;
                await db.SaveChangesAsync();
            }
        }


        // Read all records
        public List<PrintJobModel> GetAllRecords()
        {
            List<PrintJobModel> allRecords = new List<PrintJobModel>();

            using (var db = new SqliteDbContext())
            {
                allRecords = db.PrintJob.ToList();
            };

            return allRecords;
        }
    }
}
