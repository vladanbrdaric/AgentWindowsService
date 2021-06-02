using AgentService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AgentService.Database
{
    public class SqliteDbContext : DbContext
    {
        public DbSet<PrintJobModel> PrintJob { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("FileName=PrintJobs.db", options =>
            {
                options.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
            });
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PrintJobModel>().ToTable("PrintJobs");
            modelBuilder.Entity<PrintJobModel>(entity =>
            {
                entity.HasKey(k => k.JobId);
            });
        }

    }
}
