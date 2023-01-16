using RCDWeb.Job.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Threading.Tasks;
using RCDWeb.Job.Utility;


namespace RCDWeb.Job.DAL
{
    public class RCDContext : DbContext
    {

        static RCDContext()
        {
            Database.SetInitializer<RCDContext>(null);
        }


        public RCDContext() : base("RCDContext")
        {
            base.Configuration.ValidateOnSaveEnabled = false;
            this.Configuration.LazyLoadingEnabled = false;
        }

        public DbSet<TipoAllegato> TipoAllegato { get; set; }
        public DbSet<Canale> Canale { get; set; }
        public DbSet<Stati> Stati { get; set; }
        public DbSet<LogStati> LogStati { get; set; }
        public DbSet<Allegati> Allegati { get; set; }
        public DbSet<Richieste> Richieste { get; set; }
        public DbSet<DocumentiRichiesti> DocumentiRichiesti { get; set; }
        public DbSet<TipiDocumenti> TipiDocumenti { get; set; }
        public DbSet<Uffici> Uffici { get; set; }
        public DbSet<TipiPagamento> TipiPagamento { get; set; }
        public DbSet<AbilitaElementiStato> AbilitaElementiStato { get; set; }
        public DbSet<ParametriRichiestaConsegna> ParametriRichiestaConsegna { get; set; }
        public DbSet<Elementi> Elementi { get; set; }

        public DbSet<CondizioneStato> CondizioneStato { get; set; }
        public DbSet<Avvisi> Avvisi { get; set; }
        public DbSet<Condizioni> Richiesti { get; set; }
        public DbSet<TipiRichiedente> TipiRichiedente { get; set; }
        public DbSet<EmailSettings> EmailSettings { get; set; }
        public DbSet<EmailTesti> EmailTesti { get; set; }
        public DbSet<ErroriGenerici> ErroriGenerici { get; set; }
        public DbSet<ParametriApp> ParametriApp { get; set; }
        public DbSet<LogEmail> LogEmail { get; set; }
        public DbSet<CondizioniAllegati> CondizioniAllegati { get; set; }

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
            modelBuilder.Entity<Richieste>().Ignore(t => t.Addetto);
            modelBuilder.Entity<Richieste>().Ignore(t => t.Struttura);
            modelBuilder.Entity<Richieste>().Ignore(t => t.DescrizioneUOCode);
            //modelBuilder.Entity<Richieste>().Ignore(t => t.DescrizioneFilialeConsegna);
            modelBuilder.Entity<LogStati>().Ignore(t => t.Addetto);
            modelBuilder.Entity<LogStati>().Ignore(t => t.Struttura);
            modelBuilder.Entity<DocumentiRichiesti>().Ignore(t => t.Addetto);
            modelBuilder.Entity<DocumentiRichiesti>().Ignore(t => t.Struttura);
            modelBuilder.Entity<Allegati>().Ignore(t => t.Addetto);
            modelBuilder.Entity<Allegati>().Ignore(t => t.Struttura);
            //modelBuilder.Ignore<GestioneViewModel>();

            //            modelBuilder.Entity<Richieste>().Property(a => a.Protocollo).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            // modelBuilder.Entity<Richieste>().HasKey(l => new { l.Id, l.Protocollo });
        }
    }
}
