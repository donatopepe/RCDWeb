
using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using RCDWeb.Job.Repository;
using RCDWeb.Job.DAL;

namespace RCDWeb.Job
{
    [PersistJobDataAfterExecution]
    public class CheckRichiesteJob : IJob
    {
        private static readonly ILog Log = LogManager.GetLogger<CheckRichiesteJob>();

        public void Check()
        {
            try
            {
                RCDRepository repository = new RCDRepository();

                var avvisi = repository.GetAvvisi();
                foreach (var richiestedb in repository.GetRichieste())
                {
                    foreach (var avviso in avvisi)
                        if (avviso.EmailTesti != null)
                        {
                            var emailtesto = avviso.EmailTesti;
                            var lanciaavviso = true;
                            if (!String.IsNullOrWhiteSpace(richiestedb.Avvisi))
                            {
                                var avvisiinviati = richiestedb.Avvisi.Split(';');
                                if (avvisiinviati.Contains(Convert.ToString(avviso.Id)))
                                {
                                    lanciaavviso = false;  /// avviso già inviato
                                }
                            }


                            if (lanciaavviso && (avviso.CanaleComunicazione != null))
                            {
                                if (richiestedb.CanaleConsegna.Id != avviso.CanaleComunicazione.Id)   //l'avviso viene annullato se canale comunicazione avviso è presente ed è diverso dal canale di consegna
                                {
                                    lanciaavviso = false;
                                }
                            }
                            if (lanciaavviso)
                            {
                                if (avviso.IsBanca)
                                {
                                    if (richiestedb.Stato.ConteggioBanca && (richiestedb.GiorniAttesaBanca != null))
                                    {
                                        if (richiestedb.GiorniAttesaBanca <= avviso.Giorni)
                                        {
                                            lanciaavviso = false;
                                        }
                                    }
                                    else
                                    {
                                        lanciaavviso = false;
                                    }
                                }
                                else
                                {
                                    if (richiestedb.Stato.ConteggioCliente && (richiestedb.GiorniAttesaCliente != null))
                                    {
                                        if (richiestedb.GiorniAttesaCliente <= avviso.Giorni)
                                        {
                                            lanciaavviso = false;
                                        }
                                    }
                                    else
                                    {
                                        lanciaavviso = false;
                                    }
                                }
                            }


                            if (lanciaavviso)
                            {

                                emailtesto.Oggetto = avviso.Messaggio;
                                var uocodeList = new List<string>();
                                uocodeList.Add(richiestedb.UOCodeNext);
                                if (richiestedb.UOCode != richiestedb.UOCodeNext)
                                {
                                    uocodeList.Add(richiestedb.UOCode);
                                }

                                var result = repository.SendEmail(richiestedb, uocodeList, emailtesto, avviso.NotificaResp);
                                if (result)
                                {
                                    richiestedb.Avvisi += Convert.ToString(avviso.Id) + ";";
                                    repository.Save(richiestedb);
                                }
                            }
                        }

                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

        }

        public void Execute(IJobExecutionContext context)
        {
            Check();
        }
    }
}
