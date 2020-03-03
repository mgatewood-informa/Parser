using Informa.IntelligenceOne.Parser.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;


namespace Informa.IntelligenceOne.Parser.DAL
{
    public partial class InformaContext : DbContext
    {
        public InformaContext(DbContextOptions<InformaContext> options) : base(options) 
        {
            //this.Configuration.AutoDetectChangesEnabled = false;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ZoneConfig>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ZoneConfigId");
            });

            modelBuilder.Entity<BankResults>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ResultsId");
            });
        }
        public virtual DbSet<ZoneConfig> ZoneConfig { get; set; }
    }
}
