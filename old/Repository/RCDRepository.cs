using AreaCommonWeb.Models;
using RCDWeb.Areas.RCD.DAL;
using RCDWeb.Areas.RCD.Models;
using RCDWeb.Areas.RCD.Uility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RCDWeb.Areas.RCD.Repository
{
    public class RCDRepository : IDisposable
    {
        private RCDContext db;
        private IEnumerable<Canale> canali;
        private IEnumerable<TipiPagamento> tipiPagamento;
        private IEnumerable<Uffici> Uffici;
        private IEnumerable<Elementi> PlaceHolder;
        private IEnumerable<AbilitaElementiStato> AbilitaElementiStato;
        private IEnumerable<ParametriRichiestaConsegna> ParametriRichiestaConsegna;
        private IEnumerable<TipiDocumenti> TipiDocumenti;
        private IEnumerable<TipoAllegato> TipoAllegato;
        private IEnumerable<TipiRichiedente> TipiRichiedente;
        private IEnumerable<CondizioneStato> CondizioneStato;
        private IEnumerable<Stati> Stati;
        private IEnumerable<ErroriGenerici> ErroriGenerici;

        public RCDRepository(RCDContext context)
        {
            this.db = context;
            this.canali = db.Canale.Where(s => s.IsActive == true);
            this.tipiPagamento = db.TipiPagamento.Where(s => s.IsActive == true);
            this.Uffici = db.Uffici.Where(a => (a.IsActive));
            this.PlaceHolder = db.Elementi.Where(a => a.PlaceHolder != "");
            this.AbilitaElementiStato = db.AbilitaElementiStato.Include(a => a.Stato).Where(a => (a.IsActive == true));
            this.ParametriRichiestaConsegna = db.ParametriRichiestaConsegna.Include(a => a.Richiesta).Include(b => b.Consegna).Where(a => (a.IsActive == true));
            this.TipiDocumenti = db.TipiDocumenti.Where(s => s.IsActive == true);
            this.TipoAllegato = db.TipoAllegato.Include(a => a.StatoSuccessivoObbligatorio).Where(s => s.IsActive == true);
            this.TipiRichiedente = db.TipiRichiedente.Where(s => s.IsActive == true);
            this.CondizioneStato = db.CondizioneStato
                .Include(a => a.StatoIn)
                .Include(a => a.StatoOut)
                .Include(c => c.Richiesti)
                .Include(d => d.Richiesti.Select(e => e.Elemento))
                .Where(a => (a.IsActive == true));

            this.Stati = db.Stati.Where(s => s.IsActive == true);
            this.ErroriGenerici = db.ErroriGenerici.Where(s => s.IsActive == true);
        }


        public IEnumerable<Richieste> GetRichieste(FiltriRichiesteModel filtri)
        {
            var uocodedaricercare = filtri.myUocode;
            int istituto = 0;
            if (!String.IsNullOrWhiteSpace(filtri.filtroUocode))
            { 
                uocodedaricercare = filtri.filtroUocode; 
            }
            if (filtri.istituto!=null)
            {
                istituto = (int)filtri.istituto;
            }
            var richieste = db.Richieste
                .Include(a => a.Stato)
                .Include(b => b.Stati)
                .Where(d => ((d.Istituto==istituto)&&((d.UOCode == uocodedaricercare) || (d.UOCodeNext == uocodedaricercare))));

            if ((richieste != null) && (filtri.idStatoOrdine != null))
            {
                richieste = richieste.Where(a => a.Stato.Id == filtri.idStatoOrdine);
            }
            if ((richieste != null) && (filtri.istituto != null))
            {
                richieste = richieste.Where(a => a.Istituto == filtri.istituto);
            }
            if ((richieste!=null)&& (!String.IsNullOrWhiteSpace(filtri.matricolaOperatore)))
            {
                richieste = richieste.Where(a => a.Matricola == filtri.matricolaOperatore);
            }
            if ((richieste != null) && (filtri.ndg!=null))
            {
                richieste = richieste.Where(a => a.NDG == (long)filtri.ndg);
            }
            var datada = new DateUtil().TextToDate(filtri.dataOrdineDA);
            var dataa = new DateUtil().TextToDate(filtri.dataOrdineA);
            if ((richieste != null) && (datada != null) && (dataa != null))
            {
                var da = (DateTime)datada;
                var a = ((DateTime)dataa).AddDays(1);
                richieste = richieste.Where(f => (f.DataRichiesta!=null) && (f.DataRichiesta >= da) && (f.DataRichiesta <= a));
            }

            return richieste.ToList();
        }
        public IEnumerable<Stati> GetStati()
        {
            return this.Stati;
        }
        public ErroriGenerici GetErroreGenerico(string nome)
        {
            return this.ErroriGenerici.Where(a=>a.Nome==nome).FirstOrDefault();
        }
        public IEnumerable<LogStati> GetLogStati(Guid Id)
        {
            var result = db.Richieste
                .Include(l => l.Stati)
                .Include(d => d.Stati.Select(e => e.Stato))
                //.Include(d => d.Stati.Select(e => e.UOCodeNext))
                .FirstOrDefault(b => b.Id == Id).Stati
                .OrderBy(h => h.UltimaModifica);
            return result;
        }
        public EmailSettings EmailSettings()
        {
            return db.EmailSettings.Where(a=>a.isActive).FirstOrDefault();
        }
        public EmailTesti EmailTesto(string nome)
        {
            return db.EmailTesti.Where(a => a.IsActive && a.Nome==nome).FirstOrDefault();
        }
        public Allegati GetFile(Guid id)
        {
            return db.Allegati.Find((Guid)id);
        }
        public Stati GetStato(string Nome)
        {
            return Stati.Where(a => a.Stato == Nome).FirstOrDefault();
        }
        public Stati GetStato(int Id)
        {
            return Stati.Where(a => a.Id == Id).FirstOrDefault();
        }
        public void AddFile(Richieste richiestedb, GestioneViewModel richieste, ModelFileForm fileForm)
        {
            var file = new Allegati();
            file.Id = Guid.NewGuid();
            file.FileContent = fileForm.FileContent;
            file.FileName = fileForm.FileName;
            file.ContentType = fileForm.ContentType;
            file.Lenght = fileForm.Lenght;
            file.tipoAllegato = db.TipoAllegato.Find(richieste.tipoAllegato.Id);
            file.DescrizioneFile = richieste.DescrizioneFile;
            db.Allegati.Add(file);
            richiestedb.Allegati.Add(file);
        }
        public TipiDocumenti GetTipoDocumento(int tipodocumento)
        {
            return db.TipiDocumenti.Find(tipodocumento);
        }
        public void AddDocumento(Richieste richiestedb, DocumentiRichiesti documentodb)
        {
            documentodb.Documento = GetTipoDocumento(documentodb.Documento.Id);
            documentodb.Id = Guid.NewGuid();
            db.DocumentiRichiesti.Add(documentodb);
            richiestedb.DocumentiRichiesti.Add(documentodb);
        }
        public DocumentiRichiesti GetDocumento(Guid Id_Documento)
        {
            return db.DocumentiRichiesti.Find(Id_Documento);
        }
        public void UpdateDocumento(GestioneViewModel richieste, DocumentiRichiesti documentodb)
        {
            var documentodamodificare = GetDocumento(richieste.Id_Documento);
            documentodamodificare.Documento = GetTipoDocumento(documentodb.Documento.Id);
            documentodamodificare.Rapporto = richieste.Rapporto;
            documentodamodificare.DescrizioneDocumento = richieste.DescrizioneDocumento;
            documentodamodificare.Costo = richieste.Costo;
            documentodamodificare.NumCopie = richieste.NumCopie;
            documentodamodificare.DocOrig = richieste.DocOrig;
            documentodamodificare.NumCartone = richieste.NumCartone;
            db.Entry(documentodamodificare).State = EntityState.Modified;
        }
        public void DeleteDocumento(Guid Id_Documento)
        {
            if (Id_Documento != Guid.Empty)
            {
                DocumentiRichiesti documentiRichiesti = db.DocumentiRichiesti.Find(Id_Documento);
                if (documentiRichiesti != null)
                {
                    db.DocumentiRichiesti.Remove(documentiRichiesti);

                }
            }
        }
        public void DeleteAllegato(Guid Id_Allegato)
        {
            if (Id_Allegato != Guid.Empty)
            {
                Allegati allegato = db.Allegati.Find((Guid)Id_Allegato);
                if (allegato != null)
                {
                    db.Allegati.Remove(allegato);
                }
            }
        }
        public IEnumerable<CondizioneStato> GetStatiPossibili(string operationType, int Stato_Id)
        {
            return CondizioneStato
                .Where(a => ((a.operationType == operationType.Replace("Ok", "")) && (a.StatoIn.Id == Stato_Id)))
                .OrderByDescending(z => z.StatoOut.Id);
        }
        public Richieste GetRichiesta(Guid Id)
        {
            return db.Richieste
                .Include(a => a.Stato)
                .Include(b => b.DocumentiRichiesti)
                .Include(c => c.DocumentiRichiesti.Select(cc => cc.Documento))
                .Include(d => d.Allegati)
                .Include(e => e.Allegati.Select(ee => ee.tipoAllegato))
                .Include(f => f.CanaleRicezione)
                .Include(g => g.CanaleConsegna)
                //.Include(h => h.UOCode)
                //.Include(i => i.UOCodeNext)
                .Include(j => j.ModalitaPagamento)
                .Include(k => k.TipoRichiedente)
                .Include(l => l.Stati)
                .FirstOrDefault(b => b.Id == Id);
        }
        public Richieste GetRichiestaOrCreate(Guid Id)
        {
            if (Id == Guid.Empty)
            {
                var richiestedb = new Richieste();
                richiestedb.Id = Guid.NewGuid();
                richiestedb.DataInserimento = DateTime.Now;
                db.Richieste.Add(richiestedb);
                db.SaveChanges();
                Id = richiestedb.Id;
            }
            return GetRichiesta(Id);
        }
        public TipiRichiedente GetTipoRichiedente(string Nome)
        {
            return TipiRichiedente.Where(s => (s.Nome == Nome)).FirstOrDefault();
        }
        public TipiRichiedente GetTipoRichiedente(int id)
        {
            return TipiRichiedente.Where(s => (s.Id == id)).FirstOrDefault();
        }
        public IEnumerable<TipiRichiedente> GetTipiRichiedente()
        {
            return TipiRichiedente;
        }
        public IEnumerable<TipoAllegato> GetTipoAllegato()
        {
            return TipoAllegato;
        }
        public IEnumerable<TipiDocumenti> GetTipiDocumenti()
        {
            return TipiDocumenti;
        }
        public IEnumerable<ParametriRichiestaConsegna> GetParametriRichiestaConsegna()
        {
            return ParametriRichiestaConsegna;
        }
        public IEnumerable<AbilitaElementiStato> GetAbilitaElementiStato()
        {
            return AbilitaElementiStato;
        }
        public IEnumerable<AbilitaElementiStato> GetAbilitaElementiStato(int statoattuale_Id)
        {
            return AbilitaElementiStato.Where(a => ((a.Stato.Id == statoattuale_Id) || (a.Stato.Id == 0)));
        }
        public Canale GetCanale(string Nome)
        {
            return canali.Where(a => a.Nome == Nome).FirstOrDefault();
        }
        public Canale GetCanale(int id)
        {
            return db.Canale.Find(id);
        }
        public IEnumerable<Canale> GetCanaliRicezione()
        {
            return canali.Where(s => s.Ricezione == true);
        }
        public IEnumerable<Canale> GetCanaliConsegna()
        {
            return canali.Where(s => s.Consegna == true);
        }
        public IEnumerable<TipiPagamento> GetTipiPagamento()
        {
            return tipiPagamento;
        }
        public TipiPagamento GetTipoPagamento(int id)
        {
            return db.TipiPagamento.Where(a => (a.IsActive==true)&&(a.Id==id)).FirstOrDefault();
        }
        public TipiPagamento GetTipoPagamento(string nome)
        {
            return db.TipiPagamento.Where(a => (a.IsActive == true) && (a.Nome == nome)).FirstOrDefault();
        }
        public IEnumerable<Uffici> GetUffici()
        {
            return Uffici;
        }
        public IEnumerable<Uffici> GetUffici(int Istituto, string UOCodeNext_Id, string UOCode_Id)
        {
            return Uffici.Where(a => ((a.Istituto == Istituto) && (a.UOCODE != UOCodeNext_Id) && (a.UOCODE != UOCode_Id)));
        }
        public Uffici GetUfficio(int id)
        {
            return db.Uffici.Find(id);
        }
        public Uffici GetUfficio(string uocode)
        {
            return db.Uffici.Where(a=>a.UOCODE==uocode).FirstOrDefault();
        }
        public IEnumerable<Elementi> GetPlaceHolder()
        {
            return PlaceHolder;
        }
        public void DeleteRichiesta(Guid id)
        {
            var richieste = GetRichiesta(id);
            richieste.DocumentiRichiesti.ToList().ForEach(item =>
            {
                //System.Diagnostics.Debug.WriteLine(JsonConvert.SerializeObject(item));
                db.DocumentiRichiesti.Remove(item);
            });
            richieste.Allegati.ToList().ForEach(item =>
            {
                //System.Diagnostics.Debug.WriteLine(JsonConvert.SerializeObject(item));
                db.Allegati.Remove(item);
            });
            db.SaveChanges();
            db.Richieste.Remove(richieste);
            db.SaveChanges();
        }
        public void Update(Richieste entity)
        {
            db.Entry(entity).State = EntityState.Modified;
        }
        public void Update(DocumentiRichiesti entity)
        {
            db.Entry(entity).State = EntityState.Modified;
        }
        public void Update(Allegati entity)
        {
            db.Entry(entity).State = EntityState.Modified;
        }
        public void Save()
        {
            db.SaveChanges();
        }
        public void Save(Richieste richiestedb)
        {
            richiestedb.TotaleCosto = 0;
            richiestedb.UltimaModifica = DateTime.Now;
            if (richiestedb.DataStopBanca == null)
            {
                if (richiestedb.Stato.DataStopBanca)
                {
                    richiestedb.DataStopBanca = DateTime.Now;
                }
            }
            if (richiestedb.DataChiusa == null)
            {
                if (richiestedb.Stato.Chiuso)
                {
                    richiestedb.DataChiusa = DateTime.Now;
                }
            }
            db.Entry(richiestedb).State = EntityState.Modified;
            db.SaveChanges();
            UpdateLogStati(richiestedb);
        }
        private void UpdateLogStati(Richieste richiestedb)
        {
            var aggiorna = false;
            var ultimostatorichiesta = richiestedb.Stati.OrderByDescending(c => c.UltimaModifica).FirstOrDefault();
            if ((ultimostatorichiesta != null) && (ultimostatorichiesta.Stato != null) && (ultimostatorichiesta.UOCodeNext != null))
            {
                if ((ultimostatorichiesta.Stato.Id != richiestedb.Stato.Id) ||
                    (ultimostatorichiesta.UOCodeNext != richiestedb.UOCodeNext))
                {
                    aggiorna = true;
                }
            }
            else
            {
                aggiorna = true;
            }
            if (aggiorna)
            {
                var logstato = new LogStati()
                {
                    Id = Guid.NewGuid(),
                    UltimaModifica = DateTime.Now,
                    UOCodeNext = richiestedb.UOCodeNext,
                    Matricola = richiestedb.Matricola,
                    Motivazione = richiestedb.Motivazione,
                    Note = richiestedb.Note,
                    Stato = richiestedb.Stato
                };
                richiestedb.Stati.Add(logstato);
                db.SaveChanges();
            }
        }

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    db.Dispose();
                }
            }
            this.disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}