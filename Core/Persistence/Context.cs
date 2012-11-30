using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using Core.Model;
using Core.Migrations;

namespace Core.Persistence
{
    public class Context : DbContext
    {
        public DbSet<LogEntry> LogEntries { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Usage> Usages { get; set; }
        public DbSet<Reference> References { get; set; }

        //public Context()
        //    : base("NetduinoP1Logging")
        //{ }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<Context, Configuration>());

            modelBuilder.Entity<LogEntry>().Property(x => x.CurrentRetour).HasPrecision(10, 2);
            modelBuilder.Entity<LogEntry>().Property(x => x.CurrentUsage).HasPrecision(10, 2);
            modelBuilder.Entity<LogEntry>().Property(x => x.E1).HasPrecision(10, 3);
            modelBuilder.Entity<LogEntry>().Property(x => x.E1Retour).HasPrecision(10, 3);
            modelBuilder.Entity<LogEntry>().Property(x => x.E2).HasPrecision(10, 3);
            modelBuilder.Entity<LogEntry>().Property(x => x.E2Retour).HasPrecision(10, 3);
            modelBuilder.Entity<LogEntry>().Property(x => x.GasMeasurementValue).HasPrecision(10, 3);

            modelBuilder.Entity<Usage>().Property(x => x.E1Start).HasPrecision(10, 3);
            modelBuilder.Entity<Usage>().Property(x => x.E1Current).HasPrecision(10, 3);
            modelBuilder.Entity<Usage>().Property(x => x.E1RetourStart).HasPrecision(10, 3);
            modelBuilder.Entity<Usage>().Property(x => x.E1RetourCurrent).HasPrecision(10, 3);
            modelBuilder.Entity<Usage>().Property(x => x.E2Start).HasPrecision(10, 3);
            modelBuilder.Entity<Usage>().Property(x => x.E2Current).HasPrecision(10, 3);
            modelBuilder.Entity<Usage>().Property(x => x.E2RetourStart).HasPrecision(10, 3);
            modelBuilder.Entity<Usage>().Property(x => x.E2RetourCurrent).HasPrecision(10, 3);
            modelBuilder.Entity<Usage>().Property(x => x.GasStart).HasPrecision(10, 3);
            modelBuilder.Entity<Usage>().Property(x => x.GasCurrent).HasPrecision(10, 3);

        }


    }
}
