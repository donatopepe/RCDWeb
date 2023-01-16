using RCDWeb.Areas.RCD.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Threading.Tasks;


namespace RCDWeb.Areas.RCD.DAL
{

    public class RCDContext : DbContext
    {

      static RCDContext()
      {
        Database.SetInitializer<RCDContext>(null);
      }


        public RCDContext():base("RCDContext")
        {
            base.Configuration.ValidateOnSaveEnabled = false;
        }

        public DbSet<RCDWeb.Areas.RCD.Models.TipoAllegato> TipoAllegato { get; set; }
        public DbSet<RCDWeb.Areas.RCD.Models.Canale> Canale { get; set; }
        public DbSet<RCDWeb.Areas.RCD.Models.Stati> Stati { get; set; }
        public DbSet<RCDWeb.Areas.RCD.Models.LogStati> LogStati { get; set; }
        public DbSet<RCDWeb.Areas.RCD.Models.Allegati> Allegati { get; set; }
        public DbSet<RCDWeb.Areas.RCD.Models.Richieste> Richieste { get; set; }
        public DbSet<RCDWeb.Areas.RCD.Models.DocumentiRichiesti> DocumentiRichiesti { get; set; }
        public DbSet<RCDWeb.Areas.RCD.Models.TipiDocumenti> TipiDocumenti { get; set; }
        public DbSet<RCDWeb.Areas.RCD.Models.Uffici> Uffici { get; set; }
        public DbSet<RCDWeb.Areas.RCD.Models.TipiPagamento> TipiPagamento { get; set; }
        public DbSet<RCDWeb.Areas.RCD.Models.AbilitaElementiStato> AbilitaElementiStato { get; set; }
        public DbSet<RCDWeb.Areas.RCD.Models.ParametriRichiestaConsegna> ParametriRichiestaConsegna { get; set; }
        public DbSet<RCDWeb.Areas.RCD.Models.Elementi> Elementi { get; set; }

        public DbSet<RCDWeb.Areas.RCD.Models.CondizioneStato> CondizioneStato { get; set; }
        public DbSet<RCDWeb.Areas.RCD.Models.Avvisi> Avvisi { get; set; }
        public DbSet<RCDWeb.Areas.RCD.Models.Condizioni> Richiesti { get; set; }
        public DbSet<RCDWeb.Areas.RCD.Models.TipiRichiedente> TipiRichiedente { get; set; }
        public DbSet<RCDWeb.Areas.RCD.Models.EmailSettings> EmailSettings { get; set; }
        public DbSet<RCDWeb.Areas.RCD.Models.EmailTesti> EmailTesti { get; set; }
        public DbSet<RCDWeb.Areas.RCD.Models.ErroriGenerici> ErroriGenerici { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
            
            //modelBuilder.Entity<CondizioneStato>().HasMany(o => o.Richiesti);
            //modelBuilder.Entity<CondizioneStato>().HasMany(o => o.Condizioni);
            modelBuilder.Entity<Richieste>().HasMany(o => o.Allegati);
            modelBuilder.Entity<Richieste>().HasMany(o => o.DocumentiRichiesti);
            modelBuilder.Entity<Richieste>().Ignore(t => t.GiorniAttesaBanca);
            modelBuilder.Entity<Richieste>().Ignore(t => t.GiorniAttesaCliente);
            modelBuilder.Entity<Richieste>().Ignore(t => t.TotaleCosto);
           
            modelBuilder.Ignore<GestioneViewModel>();
        }
    }
}
