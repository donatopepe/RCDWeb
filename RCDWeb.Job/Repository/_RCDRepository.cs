using AreaCommonWeb.DAL;
using AreaCommonWeb.Models;
using AreaCommonWeb.ModelView.Security;
using DAL.AreaCommonWeb;
using RCDWeb.Job.DAL;
using RCDWeb.Job.Models;
using RCDWeb.Job.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;



//RCDWeb.Job.Repository
namespace RCDWeb.Job.Repository
{
    public class RCDRepository : IDisposable
    {
        private ResourcesRepository resourcesRepository = new ResourcesRepository();
        private RCDContext db = new RCDContext();

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
        private IEnumerable<ParametriApp> ParametriApp;
        private IEnumerable<Avvisi> Avvisi;
        public int giorniscadenzabanca = 90;
        public int giorniscadenzacliente = 90;
        private IEnumerable<EmailSettings> emailSettings;
        private IEnumerable<EmailTesti> EmailTesti;
        private IEnumerable<CondizioniAllegati> CondizioniAllegati;

        public RCDRepository()
        {
            //var pippo = db.Database.CreateIfNotExists();
            //if (pippo)
            //{
            //    new InitialCreate().On(db);
            //}
            this.canali = db.Canale.Where(s => s.IsActive == true).ToList();
            this.tipiPagamento = db.TipiPagamento.Where(s => s.IsActive == true).ToList();
            this.Uffici = db.Uffici.Where(a => (a.IsActive)).ToList();
            this.PlaceHolder = db.Elementi.Where(a => a.PlaceHolder != "").ToList();
            this.AbilitaElementiStato = db.AbilitaElementiStato.Include(a => a.Stato).Where(a => (a.IsActive == true)).ToList();
            this.ParametriRichiestaConsegna = db.ParametriRichiestaConsegna.Include(a => a.Richiesta).Include(b => b.Consegna).Where(a => (a.IsActive == true)).ToList();
            this.TipiDocumenti = db.TipiDocumenti.Where(s => s.IsActive == true).ToList();
            this.TipoAllegato = db.TipoAllegato
                .Where(s => s.IsActive == true).OrderBy(a => a.Nome).ToList();
            this.TipiRichiedente = db.TipiRichiedente.Where(s => s.IsActive == true).ToList();
            this.CondizioneStato = db.CondizioneStato
                .Include(a => a.StatoIn)
                .Include(a => a.StatoOut)
                .Include(c => c.Richiesti)
                .Include(d => d.Richiesti.Select(e => e.Elemento))
                .Where(a => (a.IsActive == true)).ToList();
            this.CondizioniAllegati = db.CondizioniAllegati
                .Include(a => a.StatoIn)
                .Include(a => a.StatoOut)
                .Include(c => c.CanaleConsegna)
                .Include(d => d.CanaleRicezione)
                .Include(e => e.TipoAllegato)
                .Where(a => (a.IsActive == true)).ToList();
            this.Stati = db.Stati.Include(a => a.EmailTesti).ToList();
            this.ErroriGenerici = db.ErroriGenerici.Where(s => s.IsActive == true).ToList();
            this.ParametriApp = db.ParametriApp.Where(s => s.IsActive == true).ToList();
            this.Avvisi = db.Avvisi
                .Include(a => a.CanaleComunicazione)
                .Include(a => a.EmailTesti)
                .Where(s => s.IsActive == true).ToList();
            this.emailSettings = db.EmailSettings.Where(a => a.isActive).ToList();
            this.EmailTesti = db.EmailTesti.Where(a => a.IsActive).ToList();

            var gsb = GetParatroApp("giorniscadenzabanca");
            if (gsb != null) giorniscadenzabanca = Convert.ToInt32(gsb.Val);
            var gsc = GetParatroApp("giorniscadenzacliente");
            if (gsc != null) giorniscadenzacliente = Convert.ToInt32(gsc.Val);
        }
        public IEnumerable<Avvisi> GetAvvisi()
        {
            return this.Avvisi;
        }
        public ParametriApp GetParatroApp(string key)
        {
            return this.ParametriApp.Where(a => EqualsNormalized(a.Key, key)).FirstOrDefault();
        }
        public IEnumerable<Richieste> GetRichieste()
        {
            var richieste = db.Richieste
                .Include(a => a.Stato)
                //.Include(b => b.DocumentiRichiesti)
                //.Include(c => c.DocumentiRichiesti.Select(cc => cc.Documento))
                //.Include(d => d.Allegati)
                //.Include(e => e.Allegati.Select(ee => ee.tipoAllegato))
                .Include(f => f.CanaleRicezione)
                .Include(g => g.CanaleConsegna)
                //.Include(h => h.UOCode)
                //.Include(i => i.UOCodeNext)
                .Include(j => j.ModalitaPagamento)
                .Include(k => k.TipoRichiedente)
                .Include(l => l.Stati)
                .Where(a => (!a.Stato.Chiuso && a.Stato.IsActive));

            return richieste.OrderBy(a => a.UltimaModifica).ToList();
        }
        private bool uocodeOk(Richieste richiesta, string uocodedaricercare)
        {
            var result = ((richiesta.UOCode == uocodedaricercare) || (richiesta.UOCodeNext == uocodedaricercare));
            if (!result)
            {
                foreach (var logstato in richiesta.Stati)
                {
                    if (logstato.UOCodeNext == uocodedaricercare) return true;
                }
            }
            return result;
        }
        private bool EqualsNormalized(string a, string b)
        {
            if (!String.IsNullOrWhiteSpace(a)) a = a.Replace(" ", "").ToLower();
            if (!String.IsNullOrWhiteSpace(b)) b = b.Replace(" ", "").ToLower();
            return a == b;
        }
        public bool CheckNdgRichiesta(int? istituto, long? ndg, string codfiscale)
        {
            var richieste = db.Richieste
            .Include(a => a.Stato)
           .Where(d => (d.Stato.IsActive)).ToList();
            if (istituto != null)
            {
                richieste = richieste.Where(b => b.Istituto == (int)istituto).ToList();
            }
            if (ndg != null)
            {
                richieste = richieste.Where(b => b.NDG == (long)ndg).ToList();
            }
            if (!String.IsNullOrWhiteSpace(codfiscale))
            {
                richieste = richieste.Where(b => EqualsNormalized(b.CodiceFiscale, codfiscale)).ToList();
            }
            return richieste.Count > 0;
        }

        //public IEnumerable<Richieste> GetRichieste(FiltriRichiesteModel filtri)
        //{
        //    var uocodedaricercare = filtri.myUocode;
        //    int istituto = 0;
        //    if (!String.IsNullOrWhiteSpace(filtri.filtroUocode))
        //    {
        //        uocodedaricercare = filtri.filtroUocode;
        //    }
        //    if (filtri.istituto != null)
        //    {
        //        istituto = (int)filtri.istituto;
        //    }
        //    //var richieste = db.Richieste
        //    //    .Include(a => a.Stato)
        //    //    .Include(b => b.Stati)
        //    //    .Where(d => ((d.Stato.IsActive) && (d.Istituto == istituto) && ((d.UOCode == uocodedaricercare) || (d.UOCodeNext == uocodedaricercare))));

        //    var richieste = db.Richieste
        //        .Include(a => a.Stato)
        //        .Include(b => b.Stati)
        //        .Where(d => ((d.Stato.IsActive) && (d.Istituto == istituto))).ToList();

        //    richieste = richieste.Where(d => uocodeOk(d, uocodedaricercare)).ToList();

        //    if ((richieste != null) && (filtri.idStatoOrdine != null))
        //    {
        //        richieste = richieste.Where(a => a.Stato.Id == filtri.idStatoOrdine).ToList();
        //    }
        //    if ((richieste != null) && (filtri.istituto != null))
        //    {
        //        richieste = richieste.Where(a => a.Istituto == filtri.istituto).ToList();
        //    }
        //    if ((richieste != null) && (!String.IsNullOrWhiteSpace(filtri.matricolaOperatore)))
        //    {
        //        richieste = richieste.Where(a => a.Matricola == filtri.matricolaOperatore).ToList();
        //    }
        //    if ((richieste != null) && (filtri.ndg != null))
        //    {
        //        richieste = richieste.Where(a => a.NDG == (long)filtri.ndg).ToList();
        //    }
        //    var datada = new DateUtil().TextToDate(filtri.dataOrdineDA);
        //    var dataa = new DateUtil().TextToDate(filtri.dataOrdineA);
        //    if ((richieste != null) && (datada != null) && (dataa != null))
        //    {
        //        var da = (DateTime)datada;
        //        var a = ((DateTime)dataa).AddDays(1);
        //        richieste = richieste.Where(f => (f.DataRichiesta != null) && (f.DataRichiesta >= da) && (f.DataRichiesta <= a)).ToList();
        //    }

        //    return richieste.OrderBy(a => a.UltimaModifica).ToList();
        //}
        public IEnumerable<Stati> GetStati()
        {
            return this.Stati;
        }
        public ErroriGenerici GetErroreGenerico(string nome)
        {
            return this.ErroriGenerici.Where(a => EqualsNormalized(a.Nome, nome)).FirstOrDefault();
        }
        public IEnumerable<LogStati> GetLogStati(Guid Id)
        {
            var result = db.Richieste
                .Include(l => l.Stati)
                .Include(d => d.Stati.Select(e => e.Stato))
                //.Include(d => d.Stati.Select(e => e.UOCodeNext))
                .FirstOrDefault(b => b.Id == Id).Stati
                .OrderBy(h => h.UltimaModifica);
            return result.ToList();
        }
        public EmailSettings EmailSettings()
        {
            var result = emailSettings;
            return result.FirstOrDefault();
        }
        public EmailTesti EmailTesto(string nome)
        {
            return EmailTesti.Where(a => a.IsActive && EqualsNormalized(a.Nome, nome)).FirstOrDefault();
        }
        public Allegati GetFile(Guid id)
        {
            return db.Allegati.Find((Guid)id);
        }
        public Stati GetStato(string Nome)
        {
            return Stati.Where(a => EqualsNormalized(a.Stato, Nome)).FirstOrDefault();
        }
        public Stati GetStato(int Id)
        {
            return Stati.Where(a => a.Id == Id).FirstOrDefault();
        }
        //public void AddFile(Richieste richiestedb, GestioneViewModel richieste, ModelFileForm fileForm)
        //{
        //    var file = new Allegati();
        //    file.Id = Guid.NewGuid();
        //    file.FileContent = fileForm.FileContent;
        //    file.FileName = fileForm.FileName;
        //    file.ContentType = fileForm.ContentType;
        //    file.Lenght = fileForm.Lenght;
        //    file.tipoAllegato = db.TipoAllegato.Find(richieste.tipoAllegato.Id);
        //    file.DescrizioneFile = richieste.DescrizioneFile;
        //    db.Allegati.Add(file);
        //    richiestedb.Allegati.Add(file);
        //}
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
        //public void UpdateDocumento(GestioneViewModel richieste, DocumentiRichiesti documentodb)
        //{
        //    var documentodamodificare = GetDocumento(richieste.Id_Documento);
        //    documentodamodificare.Documento = GetTipoDocumento(documentodb.Documento.Id);
        //    documentodamodificare.Rapporto = richieste.Rapporto;
        //    documentodamodificare.DescrizioneDocumento = richieste.DescrizioneDocumento;
        //    documentodamodificare.Costo = richieste.Costo;
        //    documentodamodificare.NumCopie = richieste.NumCopie;
        //    documentodamodificare.DocOrig = richieste.DocOrig;
        //    documentodamodificare.NumCartone = richieste.NumCartone;
        //    db.Entry(documentodamodificare).State = EntityState.Modified;
        //}
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
        public IEnumerable<CondizioneStato> GetStatiPossibili(int Stato_Id)
        {
            return CondizioneStato
                .Where(a => (a.StatoIn.Id == Stato_Id))
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
                var max = db.Richieste.Max(a => (long?)a.Protocollo);
                if (max == null)
                {
                    richiestedb.Protocollo = 1;
                }
                else
                {
                    richiestedb.Protocollo = (long)max + 1;
                }
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
            return canali.Where(a => EqualsNormalized(a.Nome, Nome)).FirstOrDefault();
        }
        public Canale GetCanale(int id)
        {
            return canali.Where(a => a.Id == id).FirstOrDefault();
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
            return tipiPagamento.Where(a => (a.IsActive == true) && (a.Id == id)).FirstOrDefault();
        }
        public TipiPagamento GetTipoPagamento(string nome)
        {
            return tipiPagamento.Where(a => (a.IsActive == true) && (EqualsNormalized(a.Nome, nome))).FirstOrDefault();
        }
        public IEnumerable<Uffici> GetUffici()
        {
            return Uffici;
        }
        public IEnumerable<Uffici> GetUffici(int Istituto, string miouocode)
        {
            return Uffici.Where(a => ((a.Istituto == Istituto) && (!EqualsNormalized(a.UOCODE, miouocode))));
        }
        public Uffici GetUfficio(int id)
        {
            return Uffici.Where(a => a.Id == id).FirstOrDefault();
        }
        public Uffici GetUfficio(string uocode)
        {
            return Uffici.Where(a => EqualsNormalized(a.UOCODE, uocode)).FirstOrDefault();
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
                richiestedb.Motivazione = "";
                db.SaveChanges();
            }
        }

        public void UpdateLogEmail(LogEmail logemail)
        {
            logemail.Id = Guid.NewGuid();
            logemail.Data = DateTime.Now;
            db.LogEmail.Add(logemail);
            db.SaveChanges();
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

        public bool SendEmail(Richieste richiestedb, List<string> uocodeList, EmailTesti emailtesto, bool NotificaResp)
        {
            var result = false;
            if (emailtesto != null)
            {
                result = true;
                //string fullUrl = "";
                string host = ConfigurationManager.AppSettings["Host"];
                string nomePortale = ConfigurationManager.AppSettings["NomePortale"];
                string hostName = @"http://sso.example.com/Webapps/Portal/login.aspx?url=http://" + host + nomePortale;
                string fullUrl = hostName + @"/RCD/Richiesta/GestioneRichieste/" + richiestedb.Id;
                //fullUrl = this.Url.Action("GestioneRichieste", "Richiesta", new { }, this.Request.Url.Scheme);
                var oggetto = emailtesto.Oggetto;
                var Corpo = emailtesto.Corpo;

                Corpo = Corpo.Replace("{protocollo}", richiestedb.Protocollo.ToString());
                Corpo = Corpo.Replace("{descrizioneStato}", richiestedb.Stato.Descrizione);
                Corpo = Corpo.Replace("{Canalerichiesta}", richiestedb.CanaleRicezione.Descrizione);
                Corpo = Corpo.Replace("{Tipologiarichiedente}", richiestedb.TipoRichiedente.Descrizione);
                var datarich = richiestedb.DataRichiesta;
                if (datarich != null)
                {
                    var datar = (DateTime)datarich;

                    Corpo = Corpo.Replace("{dataRichiesta}", (datar).ToString("dd/MM/yyyy"));
                    Corpo = Corpo.Replace("{dataScadenzaBanca}", (datar.AddDays(giorniscadenzabanca)).ToString("dd/MM/yyyy"));
                }
                else
                {
                    Corpo = Corpo.Replace("{dataRichiesta}", "");
                    Corpo = Corpo.Replace("{dataScadenzaBanca}", "");
                }
                var datastopbanca = richiestedb.DataStopBanca;
                if (datastopbanca != null)
                {
                    var datas = (DateTime)datastopbanca;
                    Corpo = Corpo.Replace("{dataScadenzaCliente}", (datas.AddDays(giorniscadenzacliente)).ToString("dd/MM/yyyy"));
                }
                else
                {
                    Corpo = Corpo.Replace("{dataScadenzaCliente}", "");
                }

                Corpo = Corpo.Replace("{ndg}", richiestedb.NDG.ToString());
                Corpo = Corpo.Replace("{IntestazioneNDG}", richiestedb.IntestazioneNDG);
                try
                {
                    Corpo = Corpo.Replace("{operatore}", resourcesRepository.GetNomeUtente(richiestedb.Matricola));
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    //ErrorSignal.FromCurrentContext().Raise(ex);
                }
                Corpo = Corpo.Replace("{filiale}", richiestedb.Filiale);
                //Corpo = Corpo.Replace("{UOCode}", richiestedb.UOCode);
                //Corpo = Corpo.Replace("{UOCodeNext}", richiestedb.UOCodeNext);
                Corpo = Corpo.Replace("{note}", richiestedb.Note);
                Corpo = Corpo.Replace("{motivazione}", richiestedb.Motivazione);

                Corpo = Corpo.Replace("{link}", fullUrl);

                var emailset = EmailSettings();
                var sendmail = new EmailSender(emailset);

                var emailUoCode = "";
                foreach (var item in uocodeList)
                {
                    try
                    {
                        if (!String.IsNullOrWhiteSpace(emailUoCode)) emailUoCode += ", ";
                        emailUoCode += resourcesRepository.GetEmailByUoCode(item, Convert.ToString(richiestedb.Istituto));
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.Message);
                        //ErrorSignal.FromCurrentContext().Raise(ex);
                    }
                    if (NotificaResp)
                    {
                        try
                        {
                            var respMatr = resourcesRepository.GetRespUoCode(item);
                            if (!String.IsNullOrWhiteSpace(emailUoCode)) emailUoCode += ", ";
                            emailUoCode += resourcesRepository.GetEmailByMatrResp(respMatr);
                        }
                        catch (Exception ex)
                        {
                            //ErrorSignal.FromCurrentContext().Raise(ex);
                            System.Diagnostics.Debug.WriteLine(ex.Message);
                        }
                    }
                }
                var logemail = new LogEmail() { Richiesta = richiestedb, ErrorMessage = "NonOk", Result = false, Subject = oggetto, Message = Corpo, IsBodyHtml = emailtesto.Html };
                try
                {
                    if (!String.IsNullOrWhiteSpace(emailUoCode))
                    {
                        logemail.Email = emailUoCode;
                        var resultEmail = sendmail.SendEmail(emailUoCode, oggetto, Corpo, emailtesto.Html);
                        if (resultEmail != null)
                        {
                            if (!resultEmail.Result) result = false;
                            logemail.Result = resultEmail.Result;
                            logemail.ErrorMessage = resultEmail.ErrorMessage;
                        }
                        else
                        {
                            result = false;
                            logemail.Result = false;
                            logemail.ErrorMessage = "Invio email anomalia";
                        }
                    }
                    else
                    {
                        result = false;
                        logemail.Result = false;
                        logemail.ErrorMessage = "Email destinatario assente";
                    }
                }
                catch (Exception ex)
                {
                    result = false;
                    logemail.ErrorMessage = ex.Message;
                    logemail.Result = false;
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    //ErrorSignal.FromCurrentContext().Raise(ex);
                }
                UpdateLogEmail(logemail);

            }

            return result;
        }
    }
}