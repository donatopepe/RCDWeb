using RCDWeb.Areas.RCD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Office.Interop.Word;
using System.Web.Mvc;
using System.IO;
using Elmah;
using Newtonsoft.Json;

namespace RCDWeb.Areas.RCD.Uility
{
    //public enum Modello 
    //{
    //    Richiesta,
    //    Consegna,
    //    Comunicazione_Mod1,
    //    Comunicazione_Mod2,
    //    Disposizione_add_spese
    //}


    public class StampaDocumenti
    {

        

        private Richieste richesta;
        //private string fullpath = "d:\\ApplicazioniMvc\\Portal\\Doc_Files\\RCD\\";
        private string fullpath = @"Doc_Files/RCD/";
        private string filetemp = @"Doc_Files/RCD/temp/";

        public StampaDocumenti(Richieste richiesta)
        {
            this.richesta = richiesta;
            //var assemblyPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            //System.Diagnostics.Debug.WriteLine("Path assembly: >>>>>>>>>>>>  "+assemblyPath);
            //var ex = new Exception("Path assembly: >>>>>>>>>>>>  " + assemblyPath);
            //ErrorSignal.FromCurrentContext().Raise(ex);

            //var referenceDirectory = new DirectoryInfo(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(assemblyPath), pathfile));
            //this.fullpath = referenceDirectory.ToString();

            //this.Compila_Richiesta();
            //switch (modello)
            //{
            //    case Modello.Richiesta:
            //        this.Compila_Richiesta();
            //    case Modello.Consegna:
            //        this.Compila_Richiesta();
            //    default:

            //        break;
            //}
        }

        private Document AssegnaCampi(Document doc)
        {
            try
            {
                int ex = 0;
                
                ErrorSignal.FromCurrentContext().Raise(new Exception("Richiesta: "+JsonConvert.SerializeObject(this.richesta)));

                Microsoft.Office.Interop.Word.Range range = doc.Content;
                string filiale = this.richesta.Filiale == null ? "" : this.richesta.Filiale;

                ErrorSignal.FromCurrentContext().Raise(new Exception(Convert.ToString(ex++)));
                range.Find.Execute(FindText: "FILIALE_PROG", ReplaceWith: filiale, Replace: WdReplace.wdReplaceAll);
                ErrorSignal.FromCurrentContext().Raise(new Exception(Convert.ToString(ex++)));
                range.Find.Execute(FindText: "INDIRIZZO_FILIALE", ReplaceWith: "??????????", Replace: WdReplace.wdReplaceAll);
                ErrorSignal.FromCurrentContext().Raise(new Exception(Convert.ToString(ex++)));
                range.Find.Execute(FindText: "DATA_PROG", ReplaceWith: this.richesta.DataRichiesta, Replace: WdReplace.wdReplaceAll);
                ErrorSignal.FromCurrentContext().Raise(new Exception(Convert.ToString(ex++)));
                range.Find.Execute(FindText: "NDG_PROG", ReplaceWith: this.richesta.NDG, Replace: WdReplace.wdReplaceAll);
                ErrorSignal.FromCurrentContext().Raise(new Exception(Convert.ToString(ex++)));
                range.Find.Execute(FindText: "COGNOME_NOME_PROG", ReplaceWith: this.richesta.IntestazioneNDG, Replace: WdReplace.wdReplaceAll);
                ErrorSignal.FromCurrentContext().Raise(new Exception(Convert.ToString(ex++)));
                string piva = (this.richesta.PIva == null) ? "" : "/" + this.richesta.PIva;
                range.Find.Execute(FindText: "CF_PI_PROG", ReplaceWith: this.richesta.CodiceFiscale + piva, Replace: WdReplace.wdReplaceAll);
                ErrorSignal.FromCurrentContext().Raise(new Exception(Convert.ToString(ex++)));
                range.Find.Execute(FindText: "LUOGO_NASCITA_PROG", ReplaceWith: this.richesta.LuogoNascita, Replace: WdReplace.wdReplaceAll);
                ErrorSignal.FromCurrentContext().Raise(new Exception(Convert.ToString(ex++)));
                range.Find.Execute(FindText: "INDIRIZZO_PROG", ReplaceWith: this.richesta.Indirizzo, Replace: WdReplace.wdReplaceAll);
                ErrorSignal.FromCurrentContext().Raise(new Exception(Convert.ToString(ex++)));

                range.Find.Execute(FindText: "TOTALE_DOCUMENTI", ReplaceWith: String.Format("{0:#.00}", this.richesta.ImportoTotale), Replace: WdReplace.wdReplaceAll);
                ErrorSignal.FromCurrentContext().Raise(new Exception(Convert.ToString(ex++)));

                string pagamento = (this.richesta.ModalitaPagamento == null) ? "" : this.richesta.ModalitaPagamento.Nome.ToUpper();
                ErrorSignal.FromCurrentContext().Raise(new Exception(Convert.ToString(ex++)));
                range.Find.Execute(FindText: "CC_PROG",
                    ReplaceWith: pagamento == "CONTO" ? "X" : "", Replace: WdReplace.wdReplaceAll,
                                    MatchCase: WdFindMatch.wdMatchField);
                range.Find.Execute(FindText: "CONTOCORRENTE_PROG", ReplaceWith: this.richesta.NumeroConto, Replace: WdReplace.wdReplaceAll);
                ErrorSignal.FromCurrentContext().Raise(new Exception(Convert.ToString(ex++)));
                range.Find.Execute(FindText: "CAS_PROG",
                    ReplaceWith: pagamento == "CASSA" ? "X" : "", Replace: WdReplace.wdReplaceAll);

                range.Find.Execute(FindText: "M_01",
                    ReplaceWith: this.richesta.CanaleConsegna.Nome.ToUpper() == "EMAIL" ? "X" : "", Replace: WdReplace.wdReplaceAll);
                ErrorSignal.FromCurrentContext().Raise(new Exception(Convert.ToString(ex++)));
                range.Find.Execute(FindText: "MAIL_CLIENTE_PROG", ReplaceWith: this.richesta.email, Replace: WdReplace.wdReplaceAll);
                ErrorSignal.FromCurrentContext().Raise(new Exception(Convert.ToString(ex++)));

                range.Find.Execute(FindText: "M_02",
                    ReplaceWith: this.richesta.CanaleConsegna.Nome.ToUpper() == "PEC" ? "X" : "", Replace: WdReplace.wdReplaceAll);
                ErrorSignal.FromCurrentContext().Raise(new Exception(Convert.ToString(ex++)));
                range.Find.Execute(FindText: "PEC_CLIENTE_PROG  ", ReplaceWith: this.richesta.pec, Replace: WdReplace.wdReplaceAll);
                ErrorSignal.FromCurrentContext().Raise(new Exception(Convert.ToString(ex++)));

                range.Find.Execute(FindText: "M_03",
                    ReplaceWith: this.richesta.CanaleConsegna.Nome.ToUpper() == "FILIALE" ? "X" : "", Replace: WdReplace.wdReplaceAll);
                ErrorSignal.FromCurrentContext().Raise(new Exception(Convert.ToString(ex++)));

                range.Find.Execute(FindText: "TOTALE_DOCUMENTI", ReplaceWith: String.Format("{0:#.00}", this.richesta.ImportoTotale), Replace: WdReplace.wdReplaceAll);
                ErrorSignal.FromCurrentContext().Raise(new Exception(Convert.ToString(ex++)));

                range.Find.Execute(FindText: "LUOGO_PIEPAGINA", ReplaceWith: this.richesta.Luogo, Replace: WdReplace.wdReplaceAll);
                ErrorSignal.FromCurrentContext().Raise(new Exception(Convert.ToString(ex++)));
                range.Find.Execute(FindText: "DATAOGGI", ReplaceWith: DateTime.Now.ToShortDateString(), Replace: WdReplace.wdReplaceAll);
                ErrorSignal.FromCurrentContext().Raise(new Exception(Convert.ToString(ex++)));
            }
            catch(Exception ex)
            {
                //System.Diagnostics.Debug.WriteLine(ex.Message);
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
            
            return doc;
        }

        public byte[] Compila_Richiesta()
        {
            var file_memoria = new MemoryStream();

            try
            {
                string nomefile = "Modulo_Richiesta_Documentazione_REV_new.docx";
                object m = System.Reflection.Missing.Value;
                object oldFileName;


                oldFileName = this.fullpath + nomefile;
                Directory.CreateDirectory(filetemp);
                filetemp += DateTime.UtcNow.Ticks+@".docx";
                File.Copy(oldFileName.ToString(),filetemp);
                System.Diagnostics.Debug.WriteLine(oldFileName);


                var ex = new Exception("File: >>>>>>>>>>>>  " + oldFileName + ":" + File.Exists(oldFileName.ToString()));
                ErrorSignal.FromCurrentContext().Raise(ex);

                oldFileName = filetemp;

                ex = new Exception("File: >>>>>>>>>>>>  " + filetemp + ":" + File.Exists(filetemp.ToString()));
                ErrorSignal.FromCurrentContext().Raise(ex);


                //string pippo = @"\\localhost\ApplicazioniMvc\Portal\Doc_Files\RCD\";
                // ex = new Exception("File: >>>>>>>>>>>>  " + pippo + ":" + File.Exists(pippo + nomefile));
                //ErrorSignal.FromCurrentContext().Raise(ex);
                //pippo = @"\\\localhost\\ApplicazioniMvc\\Portal\\Doc_Files\\RCD\\";
                //ex = new Exception("File: >>>>>>>>>>>>  " + pippo + ":" + File.Exists(pippo + nomefile));
                //ErrorSignal.FromCurrentContext().Raise(ex);
                //pippo = @"c:\ApplicazioniMvc\Portal\Doc_Files\RCD\";
                // ex = new Exception("File: >>>>>>>>>>>>  " + pippo + ":" + File.Exists(pippo + nomefile));
                //ErrorSignal.FromCurrentContext().Raise(ex);
                //pippo = @"d:\ApplicazioniMvc\Portal\Doc_Files\RCD\";
                // ex = new Exception("File: >>>>>>>>>>>>  " + pippo + ":" + File.Exists(pippo + nomefile));
                //ErrorSignal.FromCurrentContext().Raise(ex);



                //object oldFileName = (object)pathfile + "Modulo_Richiesta_Documentazione_REV_new.docx";
                object readOnly = (object)false;
                Microsoft.Office.Interop.Word.Application ac = null;
                ac = new Microsoft.Office.Interop.Word.Application();
                // Now we open the document.
                Microsoft.Office.Interop.Word.Document doc = ac.Documents.Open(ref oldFileName, ref m, ref readOnly,
                     ref m, ref m, ref m, ref m, ref m, ref m, ref m,
                     ref m, ref m, ref m, ref m, ref m, ref m);

                ex = new Exception("<<<<<<<<<<<<< Apertura file word >>>>>>>>>>>>>>>>");
                ErrorSignal.FromCurrentContext().Raise(ex);

                //Microsoft.Office.Interop.Word.Range range = doc.Content;

                //range.Find.Execute(FindText: "FILIALE_PROG", ReplaceWith: this.richesta.Filiale, Replace: WdReplace.wdReplaceAll);
                //range.Find.Execute(FindText: "DATA_PROG", ReplaceWith: this.richesta.DataRichiesta, Replace: WdReplace.wdReplaceAll);
                //range.Find.Execute(FindText: "NDG_PROG", ReplaceWith: this.richesta.NDG, Replace: WdReplace.wdReplaceAll);
                //range.Find.Execute(FindText: "COGNOME_NOME_PROG", ReplaceWith: this.richesta.IntestazioneNDG, Replace: WdReplace.wdReplaceAll);
                //string piva = (this.richesta.PIva == null) ? "" : "/" + this.richesta.PIva;
                //range.Find.Execute(FindText: "CF_PI_PROG", ReplaceWith: this.richesta.CodiceFiscale + piva, Replace: WdReplace.wdReplaceAll);
                //range.Find.Execute(FindText: "LUOGO_NASCITA_PROG", ReplaceWith: this.richesta.LuogoNascita, Replace: WdReplace.wdReplaceAll);
                //range.Find.Execute(FindText: "INDIRIZZO_PROG", ReplaceWith: this.richesta.Indirizzo, Replace: WdReplace.wdReplaceAll);

                doc = this.AssegnaCampi(doc);
                Microsoft.Office.Interop.Word.Range range = doc.Content;

                ex = new Exception("<<<<<<<<<<<<< Campi assegnati file word >>>>>>>>>>>>>>>>");
                ErrorSignal.FromCurrentContext().Raise(ex);

                //string des_rap = "";
                //string cartone = "";
                //foreach (Table table in doc.Tables)
                //{
                //    if (table.Title == "TABELLA_ALLEGATI")
                //    {
                //        foreach (DocumentiRichiesti doc_ric in this.richesta.DocumentiRichiesti)
                //        {
                //            table.Rows[table.Rows.Count].Cells[1].Range.Text =
                //                   doc_ric.NumCopie.ToString();

                //            des_rap = string.IsNullOrEmpty(doc_ric.Rapporto) ? "" : " " + doc_ric.Rapporto;
                //            table.Rows[table.Rows.Count].Cells[2].Range.Text = doc_ric.DescrizioneDocumento + des_rap;


                //            table.Rows[table.Rows.Count].Cells[3].Range.Text =
                //                doc_ric.DocOrig ? "X" : "";

                //            table.Rows[table.Rows.Count].Cells[4].Range.Text =
                //                 !doc_ric.DocOrig ? "X" : "";

                //            cartone = doc_ric.NumCartone == null ? "" : doc_ric.NumCartone.ToString();
                //            table.Rows[table.Rows.Count].Cells[6].Range.Text = cartone;

                //            table.Rows.Add();
                //        }
                //        break;
                //    }
                //}




                //string pagamento = (this.richesta.ModalitaPagamento == null) ? "" : this.richesta.ModalitaPagamento.Nome.ToUpper();
                //range.Find.Execute(FindText: "CC_PROG",
                //    ReplaceWith: pagamento == "CONTO" ? "X" : "", Replace: WdReplace.wdReplaceAll,
                //                    MatchCase: WdFindMatch.wdMatchField);
                //range.Find.Execute(FindText: "CONTOCORRENTE_PROG", ReplaceWith: this.richesta.NumeroConto, Replace: WdReplace.wdReplaceAll);
                //range.Find.Execute(FindText: "CAS_PROG",
                //    ReplaceWith: pagamento == "CASSA" ? "X" : "", Replace: WdReplace.wdReplaceAll);

                //range.Find.Execute(FindText: "M_01",
                //    ReplaceWith: this.richesta.CanaleConsegna.Nome.ToUpper() == "EMAIL" ? "X" : "", Replace: WdReplace.wdReplaceAll);
                //range.Find.Execute(FindText: "MAIL_CLIENTE_PROG  ", ReplaceWith: this.richesta.email, Replace: WdReplace.wdReplaceAll);

                //range.Find.Execute(FindText: "M_02",
                //    ReplaceWith: this.richesta.CanaleConsegna.Nome.ToUpper() == "PEC" ? "X" : "", Replace: WdReplace.wdReplaceAll);
                //range.Find.Execute(FindText: "PEC_CLIENTE_PROG  ", ReplaceWith: this.richesta.pec, Replace: WdReplace.wdReplaceAll);

                //range.Find.Execute(FindText: "M_03",
                //    ReplaceWith: this.richesta.CanaleConsegna.Nome.ToUpper() == "FILIALE" ? "X" : "", Replace: WdReplace.wdReplaceAll);

                ex = new Exception("<<<<<<<<<<<<< Inzio creazione file temporaneo >>>>>>>>>>>>>>>> " + filetemp);
                ErrorSignal.FromCurrentContext().Raise(ex);

                //string filetemp = Path.GetTempFileName();


                // doc.SaveAs2(filetemp);
                doc.Save();

                ex = new Exception("<<<<<<<<<<<<< File creato temporaneo >>>>>>>>>>>>>>>> " + filetemp);
                ErrorSignal.FromCurrentContext().Raise(ex);

                doc.Close();

                ex = new Exception("<<<<<<<<<<<<< Chiudo Doc e apro stram temporaneo >>>>>>>>>>>>>>>> " + filetemp);
                ErrorSignal.FromCurrentContext().Raise(ex);

                Stream stream = File.Open(filetemp, FileMode.Open);
                
                stream.CopyTo(file_memoria);
                stream.Close();

                ex = new Exception("<<<<<<<<<<<<< Chiudo stram temporaneo >>>>>>>>>>>>>>>> " + filetemp);
                ErrorSignal.FromCurrentContext().Raise(ex);

                //File.Delete(filetemp);
            }
            catch(Exception ex)
            {
                //System.Diagnostics.Debug.WriteLine(ex.Message);
                ErrorSignal.FromCurrentContext().Raise(ex);
            }

            var ex2 = new Exception("<<<<<<<<<<<<< Ritorno ok >>>>>>>>>>>>>>>> " + filetemp);
            ErrorSignal.FromCurrentContext().Raise(ex2);
            return file_memoria.ToArray();
        }

        public byte[] Compila_Consegna()
        {
            var file_memoria = new MemoryStream();
            try
            {
                string nomefile = "Ricevuta_consegna_copia_documentazione_new.docx";
                object m = System.Reflection.Missing.Value;
                object oldFileName;
                oldFileName = this.fullpath + nomefile;


                //object oldFileName = (object)pathfile +"Ricevuta_consegna_copia_documentazione_new.docx";
                object readOnly = (object)false;
                Microsoft.Office.Interop.Word.Application ac = null;
                ac = new Microsoft.Office.Interop.Word.Application();
                // Now we open the document.
                Microsoft.Office.Interop.Word.Document doc = ac.Documents.Open(ref oldFileName, ref m, ref readOnly,
                     ref m, ref m, ref m, ref m, ref m, ref m, ref m,
                     ref m, ref m, ref m, ref m, ref m, ref m);

                doc = this.AssegnaCampi(doc);
                Microsoft.Office.Interop.Word.Range range = doc.Content;

                string canalerichiesta = this.richesta.CanaleRicezione.Nome.ToUpper();
                switch (canalerichiesta)
                {
                    case "FILIALE":
                        {
                            range.Find.Execute(FindText: "M_03", ReplaceWith: "X", Replace: WdReplace.wdReplaceAll);
                            range.Find.Execute(FindText: "M_02", ReplaceWith: "", Replace: WdReplace.wdReplaceAll);
                            range.Find.Execute(FindText: "M_04", ReplaceWith: "", Replace: WdReplace.wdReplaceAll);
                            range.Find.Execute(FindText: "DATA_RICHIESTA_1", ReplaceWith: this.richesta.DataRichiesta, Replace: WdReplace.wdReplaceAll);
                            range.Find.Execute(FindText: "DATA_RICHIESTA_2", ReplaceWith: "", Replace: WdReplace.wdReplaceAll);
                            range.Find.Execute(FindText: "DATA_RICHIESTA_3", ReplaceWith: "", Replace: WdReplace.wdReplaceAll);
                            break;
                        }

                    case "PEC":
                        {
                            range.Find.Execute(FindText: "M_03", ReplaceWith: "", Replace: WdReplace.wdReplaceAll);
                            range.Find.Execute(FindText: "M_02", ReplaceWith: "X", Replace: WdReplace.wdReplaceAll);
                            range.Find.Execute(FindText: "M_04", ReplaceWith: "", Replace: WdReplace.wdReplaceAll);

                            range.Find.Execute(FindText: "DATA_RICHIESTA_2", ReplaceWith: this.richesta.DataRichiesta, Replace: WdReplace.wdReplaceAll);
                            range.Find.Execute(FindText: "DATA_RICHIESTA_1", ReplaceWith: "", Replace: WdReplace.wdReplaceAll);
                            range.Find.Execute(FindText: "DATA_RICHIESTA_3", ReplaceWith: "", Replace: WdReplace.wdReplaceAll);
                            break;
                        }
                    case "RACCOMANDATA":
                        {
                            range.Find.Execute(FindText: "M_03", ReplaceWith: "", Replace: WdReplace.wdReplaceAll);
                            range.Find.Execute(FindText: "M_02", ReplaceWith: "", Replace: WdReplace.wdReplaceAll);
                            range.Find.Execute(FindText: "M_04", ReplaceWith: "X", Replace: WdReplace.wdReplaceAll);

                            range.Find.Execute(FindText: "DATA_RICHIESTA_3", ReplaceWith: this.richesta.DataRichiesta, Replace: WdReplace.wdReplaceAll);
                            range.Find.Execute(FindText: "DATA_RICHIESTA_1", ReplaceWith: "", Replace: WdReplace.wdReplaceAll);
                            range.Find.Execute(FindText: "DATA_RICHIESTA_2", ReplaceWith: "", Replace: WdReplace.wdReplaceAll);
                            break;
                        }
                }

                range.Find.Execute(FindText: "DATA_RICEVUTA_DOC", ReplaceWith: this.richesta.DataConsegna, Replace: WdReplace.wdReplaceAll);

                string des_rap = "";

                foreach (Table table in doc.Tables)
                {
                    if (table.Title == "TABELLA_ALLEGATI")
                    {
                        foreach (DocumentiRichiesti doc_ric in this.richesta.DocumentiRichiesti)
                        {
                            table.Rows[table.Rows.Count].Cells[1].Range.Text =
                                   doc_ric.NumCopie.ToString();

                            des_rap = string.IsNullOrEmpty(doc_ric.Rapporto) ? "" : " " + doc_ric.Rapporto;
                            table.Rows[table.Rows.Count].Cells[2].Range.Text = doc_ric.DescrizioneDocumento + des_rap;


                            table.Rows[table.Rows.Count].Cells[3].Range.Text =
                                doc_ric.DocOrig ? "X" : "";

                            table.Rows[table.Rows.Count].Cells[4].Range.Text =
                                 !doc_ric.DocOrig ? "X" : "";

                            table.Rows.Add();
                        }
                        break;
                    }
                }

                //string filetemp = Path.GetTempFileName();
                string filetemp = fullpath + "\\temp\\";
                Directory.CreateDirectory(filetemp);
                filetemp += DateTime.UtcNow.Ticks;
                doc.SaveAs2(filetemp);
                doc.Close();

                Stream stream = File.Open(filetemp, FileMode.Open);
                
                stream.CopyTo(file_memoria);
                stream.Close();
                File.Delete(filetemp);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
  

            return file_memoria.ToArray();
        }

        public byte[] Compila_Addspese()
        {
            var file_memoria = new MemoryStream();
            try
            {
                object nomefile = "Lettera_Disposizione_Addebito_Spese_Copie_new.docx";
                object m = System.Reflection.Missing.Value;
                object oldFileName;

                //if (File.Exists(this.fullpath + nomefile))
                    oldFileName = this.fullpath + nomefile;
                //else
                //    oldFileName = this.pathfile + nomefile;


                //object oldFileName = (object)pathfile + "Lettera_Disposizione_Addebito_Spese_Copie_new.docx";
                object readOnly = (object)false;
                Microsoft.Office.Interop.Word.Application ac = null;
                ac = new Microsoft.Office.Interop.Word.Application();
                // Now we open the document.
                Microsoft.Office.Interop.Word.Document doc = ac.Documents.Open(ref oldFileName, ref m, ref readOnly,
                     ref m, ref m, ref m, ref m, ref m, ref m, ref m,
                     ref m, ref m, ref m, ref m, ref m, ref m);

                doc = this.AssegnaCampi(doc);

                Microsoft.Office.Interop.Word.Range range = doc.Content;
                range.Find.Execute(FindText: "Data_Avviso ", ReplaceWith: this.richesta.DataAvviso, Replace: WdReplace.wdReplaceAll);

                string des_rap = "";

                foreach (Table table in doc.Tables)
                {
                    if (table.Title == "TABELLA_ALLEGATI")
                    {
                        foreach (DocumentiRichiesti doc_ric in this.richesta.DocumentiRichiesti)
                        {
                            table.Rows[table.Rows.Count].Cells[1].Range.Text =
                                   doc_ric.NumCopie.ToString();

                            des_rap = string.IsNullOrEmpty(doc_ric.Rapporto) ? "" : " " + doc_ric.Rapporto;
                            table.Rows[table.Rows.Count].Cells[2].Range.Text = doc_ric.DescrizioneDocumento + des_rap;

                            table.Rows[table.Rows.Count].Cells[3].Range.Text = String.Format("{0:#.00}", doc_ric.Costo);

                            table.Rows.Add();
                        }
                        break;
                    }
                }

                //string filetemp = Path.GetTempFileName();
                string filetemp = fullpath + "\\temp\\";
                Directory.CreateDirectory(filetemp);
                filetemp += DateTime.UtcNow.Ticks;
                doc.SaveAs2(filetemp);
                doc.Close();

                Stream stream = File.Open(filetemp, FileMode.Open);
                
                stream.CopyTo(file_memoria);
                stream.Close();
                File.Delete(filetemp);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
            

            return file_memoria.ToArray();
        }


        public byte[] Compila_ModDocumentiGiacenza()
        {
            var file_memoria = new MemoryStream();

            try
            {
                object nomefile = "FAC-simile_Comunic_Mod1_new.docx";
                object m = System.Reflection.Missing.Value;
                object oldFileName;

                //if (File.Exists(this.fullpath + nomefile))
                    oldFileName = this.fullpath + nomefile;
                //else
                //    oldFileName = this.pathfile + nomefile;

                //object oldFileName = (object)pathfile + "FAC-simile_Comunic_Mod1_new.docx";
                //object oldFileName = (object)"c:\\sviluppo\\TEST_word2.docx";
                object readOnly = (object)false;
                Microsoft.Office.Interop.Word.Application ac = null;
                ac = new Microsoft.Office.Interop.Word.Application();
                // Now we open the document.
                Microsoft.Office.Interop.Word.Document doc = ac.Documents.Open(ref oldFileName, ref m, ref readOnly,
                     ref m, ref m, ref m, ref m, ref m, ref m, ref m,
                     ref m, ref m, ref m, ref m, ref m, ref m);

                doc = this.AssegnaCampi(doc);

                Microsoft.Office.Interop.Word.Range range = doc.Content;

                range.Find.Execute(FindText: "PEC_CLIENTE_PROG  ", ReplaceWith: this.richesta.pec, Replace: WdReplace.wdReplaceAll);


                string des_rap = "";
                foreach (Table table in doc.Tables)
                {
                    if (table.Title == "TABELLA_ALLEGATI")
                    {
                        foreach (DocumentiRichiesti doc_ric in this.richesta.DocumentiRichiesti)
                        {
                            table.Rows[table.Rows.Count].Cells[1].Range.Text =
                                   doc_ric.NumCopie.ToString();

                            des_rap = string.IsNullOrEmpty(doc_ric.Rapporto) ? "" : " " + doc_ric.Rapporto;
                            table.Rows[table.Rows.Count].Cells[2].Range.Text = doc_ric.DescrizioneDocumento + des_rap;

                            table.Rows[table.Rows.Count].Cells[3].Range.Text = String.Format("{0:#.00}", doc_ric.Costo);

                            table.Rows.Add();
                        }

                        break;
                    }
                }

                range.Find.Execute(FindText: "CANALE_CONSEGNA", ReplaceWith: this.richesta.CanaleConsegna.Descrizione, Replace: WdReplace.wdReplaceAll);
                range.Find.Execute(FindText: "CONTOCORRENTE_PROG", ReplaceWith: this.richesta.NumeroConto, Replace: WdReplace.wdReplaceAll);


                //string filetemp = Path.GetTempFileName();
                string filetemp = fullpath + "\\temp\\";
                Directory.CreateDirectory(filetemp);
                filetemp += DateTime.UtcNow.Ticks; doc.SaveAs2(filetemp);
                doc.Close();

                Stream stream = File.Open(filetemp, FileMode.Open);
                
                stream.CopyTo(file_memoria);
                stream.Close();
                File.Delete(filetemp);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                ErrorSignal.FromCurrentContext().Raise(ex);
            }

            

            return file_memoria.ToArray();
        }

        
        public byte[] Compila_ModInvioDocumentazione()
        {
            var file_memoria = new MemoryStream();

            try
            {
                object nomefile = "FAC -simile Comunic_Mod2_new.docx";
                object m = System.Reflection.Missing.Value;
                object oldFileName;

                //if (File.Exists(this.fullpath + nomefile))
                    oldFileName = this.fullpath + nomefile;
                //else
                //    oldFileName = this.pathfile + nomefile;

                //object oldFileName = (object)pathfile + "FAC -simile Comunic_Mod2_new.docx";
                //object oldFileName = (object)"c:\\sviluppo\\TEST_word2.docx";
                object readOnly = (object)false;
                Microsoft.Office.Interop.Word.Application ac = null;
                ac = new Microsoft.Office.Interop.Word.Application();
                // Now we open the document.
                Microsoft.Office.Interop.Word.Document doc = ac.Documents.Open(ref oldFileName, ref m, ref readOnly,
                     ref m, ref m, ref m, ref m, ref m, ref m, ref m,
                     ref m, ref m, ref m, ref m, ref m, ref m);

                doc = this.AssegnaCampi(doc);

                Microsoft.Office.Interop.Word.Range range = doc.Content;

                range.Find.Execute(FindText: "PEC_CLIENTE_PROG  ", ReplaceWith: this.richesta.pec, Replace: WdReplace.wdReplaceAll);


                string des_rap = "";
                foreach (Table table in doc.Tables)
                {
                    if (table.Title == "TABELLA_ALLEGATI")
                    {
                        foreach (DocumentiRichiesti doc_ric in this.richesta.DocumentiRichiesti)
                        {
                            table.Rows[table.Rows.Count].Cells[1].Range.Text =
                                   doc_ric.NumCopie.ToString();

                            des_rap = string.IsNullOrEmpty(doc_ric.Rapporto) ? "" : " " + doc_ric.Rapporto;
                            table.Rows[table.Rows.Count].Cells[2].Range.Text = doc_ric.DescrizioneDocumento + des_rap;

                            table.Rows[table.Rows.Count].Cells[3].Range.Text = String.Format("{0:#.00}", doc_ric.Costo);

                            table.Rows.Add();
                        }

                        break;
                    }
                }


                //doc.SaveAs2(this.pathfile+ this.richesta.Id +".docx");
                //string filetemp = Path.GetTempFileName();
                string filetemp = fullpath + "\\temp\\";
                Directory.CreateDirectory(filetemp);
                filetemp += DateTime.UtcNow.Ticks; doc.SaveAs2(filetemp);
                doc.Close();

                //var filestream = new FileStream("c:\\sviluppo\\test2.docx", FileMode.Open, FileAccess.Read);
                //var fsresult = new FileStreamResult(filestream, "application/vnd.openxmlformats-officedocument.wordprocessingm");
                //return fsresult;

                Stream stream = File.Open(filetemp, FileMode.Open);
                
                stream.CopyTo(file_memoria);
                stream.Close();
                File.Delete(filetemp);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                ErrorSignal.FromCurrentContext().Raise(ex);
            }

            

            return file_memoria.ToArray();
        }
    }
}