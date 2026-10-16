using RCDWeb.Areas.RCD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Elmah;
using Newtonsoft.Json;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Text.RegularExpressions;
using Table = DocumentFormat.OpenXml.Wordprocessing.Table;
using AreaCommonWeb.DAL;
using AreaCommonWeb.Business;
using DAL.AreaCommonWeb;
using RCDWeb.Areas.RCD.Repository;

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

        private CommonBusiness commonBusiness = new CommonBusiness();
        private MemoryStream file_memoria;
        private Richieste richesta;

        //private static string fullpath = @"Doc_Files/RCD/";
        private string fullpath;
        private string filetemp;

        //public void OpenAndAddTextToWordDocument(string filepath, string txt)
        //{
        //    Directory.CreateDirectory(filetemp);
        //    filetemp += DateTime.UtcNow.Ticks + @".docx";
        //    File.Copy(fullpath+@"Modulo_Richiesta_Documentazione_REV_new.docx", filetemp);

        //    using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(filetemp, true))
        //    {
        //        string docText = null;
        //        using (StreamReader sr = new StreamReader(wordDoc.MainDocumentPart.GetStream()))
        //        {
        //            docText = sr.ReadToEnd();
        //        }

        //        string filiale = this.richesta.Filiale == null ? "" : this.richesta.Filiale;
        //        Regex regexText = new Regex("FILIALE_PROG");
        //        docText = regexText.Replace(docText, filiale);

        //        using (StreamWriter sw = new StreamWriter(wordDoc.MainDocumentPart.GetStream(FileMode.Create)))
        //        {
        //            sw.Write(docText);
        //        }

        //        Body bod = wordDoc.MainDocumentPart.Document.Body;
        //        foreach (Table t in bod.Descendants<Table>().Where(tbl => tbl.InnerText.Contains("myTable")))
        //        {
        //            string titolo = t.InnerText;
        //            //t.Append(new TableRow(new TableCell(new Paragraph(new Run(new Text("test"))))));
        //        }

        //        //aggiunge una riga
        //        TableRow row;
        //        TableRow newriga;
        //        TableCell cell;
        //        Run run;
        //        Text testo;
        //        string des_rap, cartone;
        //        DocumentFormat.OpenXml.Wordprocessing.Paragraph parag;
        //        foreach (Table t in bod.Descendants<Table>().Where(t=> t.InnerXml.Contains("TABELLA_ALLEGATI")))
        //        {
        //            foreach (DocumentiRichiesti doc_ric in this.richesta.DocumentiRichiesti)
        //            {
        //                newriga = (TableRow)t.Elements<TableRow>().ElementAt(t.Elements<TableRow>().Count()-1).Clone();

        //                row = t.Elements<TableRow>().ElementAt(t.Elements<TableRow>().Count() - 1);//prima riga vuota
        //                row.ElementAt(1).Elements<DocumentFormat.OpenXml.Wordprocessing.Paragraph>().First().Elements<Run>().First().Elements<Text>()
        //                    .First().Text = doc_ric.NumCopie.ToString();

        //                des_rap = string.IsNullOrEmpty(doc_ric.Rapporto) ? "" : " " + doc_ric.Rapporto;
        //                row.ElementAt(2).Elements<DocumentFormat.OpenXml.Wordprocessing.Paragraph>().First().Elements<Run>().First().Elements<Text>()
        //                    .First().Text = doc_ric.DescrizioneDocumento + des_rap;

        //                row.ElementAt(3).Elements<DocumentFormat.OpenXml.Wordprocessing.Paragraph>().First().Elements<Run>().First().Elements<Text>()
        //                        .First().Text = doc_ric.DocOrig ? "X" : "";

        //                row.ElementAt(4).Elements<DocumentFormat.OpenXml.Wordprocessing.Paragraph>().First().Elements<Run>().First().Elements<Text>()
        //                        .First().Text = !doc_ric.DocOrig ? "X" : "";

        //                cartone = doc_ric.NumCartone == null ? "" : doc_ric.NumCartone.ToString();
        //                row.ElementAt(6).Elements<DocumentFormat.OpenXml.Wordprocessing.Paragraph>().First().Elements<Run>().First().Elements<Text>()
        //                    .First().Text = cartone;

        //                //table.Rows.Add();
        //                t.Append(newriga);
        //            }

        //            //cell = row.Elements<TableCell>().ElementAt(2);
        //            //// Find the first paragraph in the table cell.  
        //            //parag = cell.Elements<DocumentFormat.OpenXml.Wordprocessing.Paragraph>().First();
        //            //// Find the first run in the paragraph.  
        //            //run = parag.Elements<Run>().First();
        //            //// Set the text for the run.  
        //            //testo = run.Elements<Text>().First();
        //            //testo.Text = "yyy3";

        //            //nuovariga
        //            t.Append(new TableRow(new TableCell(new DocumentFormat.OpenXml.Wordprocessing. Paragraph(new Run(new Text("test"))))));
        //        }

        //        wordDoc.Save();
        //        wordDoc.Close();
        //    }



        //    Stream stream = File.Open(filetemp, FileMode.Open);
        //    var file_memoria = new MemoryStream();
        //    stream.CopyTo(file_memoria);
        //    stream.Close();

        //    //deve restituire 
        //    //return file_memoria.ToArray();


        //}

        public StampaDocumenti(Richieste richiesta)
        {
            this.richesta = richiesta;
            this.fullpath = new RCDRepository().GetParatroApp("docfilesfullpath").Val;
            this.filetemp = fullpath + @"temp/";
            //OpenAndAddTextToWordDocument(strDoc, strTxt);                      
        }

        private string CaratteriSpeciali(string campodb)
        {
            //        '"':"&quot;"
            //'&':"&amp;"
            //"'":"&apos;"
            //"<":"&lt;"
            //">":"&gt;"
            string valore = string.IsNullOrEmpty(campodb) ? " " : campodb;
            valore = valore.Replace("\"", "&quot;");
            valore = valore.Replace("&", "&amp;");
            valore = valore.Replace("'", "&apos;");
            valore = valore.Replace("<", "&lt;");
            valore = valore.Replace(">", "&gt;");

            return valore;
        }

        private bool AssegnaCampi(WordprocessingDocument wordDoc)
        {
            try
            {

                string docText = null;
                using (StreamReader sr = new StreamReader(wordDoc.MainDocumentPart.GetStream()))
                {
                    docText = sr.ReadToEnd();
                }

                Regex regexText;

                string canalericezione = " ";
                if (this.richesta.CanaleRicezione != null)
                    canalericezione = this.richesta.CanaleRicezione.Nome.ToUpper();

                string canaleconsegna = " ";
                if (this.richesta.CanaleConsegna != null)
                    canaleconsegna = this.richesta.CanaleConsegna.Nome.ToUpper();
                string descrizione_filiale = CaratteriSpeciali(this.richesta.Filiale);

                string uocode = this.richesta.FilialeUOcode;
                string luogo = this.richesta.Luogo;

                //ErrorSignal.FromCurrentContext().Raise(new Exception("canalericezione " + this.richesta.CanaleRicezione.Nome + " canaleconsegna " + this.richesta.CanaleConsegna.Nome));

                string datnas_sede_cciaa_albo = "";
                if (this.richesta.Cointestatari == null || this.richesta.Cointestatari.Count <= 0)
                {
                    datnas_sede_cciaa_albo = CaratteriSpeciali(this.richesta.LuogoNascita);
                    if (this.richesta.DataNascita != null)
                    {
                        if (!String.IsNullOrWhiteSpace(datnas_sede_cciaa_albo)) datnas_sede_cciaa_albo += " ";
                        datnas_sede_cciaa_albo = datnas_sede_cciaa_albo + Convert.ToDateTime(this.richesta.DataNascita).ToShortDateString();
                    }
                    if (!string.IsNullOrEmpty(this.richesta.IscrizioneCCIAA))
                    {
                        if (!String.IsNullOrWhiteSpace(datnas_sede_cciaa_albo)) datnas_sede_cciaa_albo += " ";
                        datnas_sede_cciaa_albo = datnas_sede_cciaa_albo + this.richesta.IscrizioneCCIAA;
                    }
                    if (!string.IsNullOrEmpty(this.richesta.AlboProfessionale))
                    {
                        if (!String.IsNullOrWhiteSpace(datnas_sede_cciaa_albo)) datnas_sede_cciaa_albo += " ";
                        datnas_sede_cciaa_albo = datnas_sede_cciaa_albo + this.richesta.AlboProfessionale;
                    }
                }
                else
                {
                    foreach (var coint in this.richesta.Cointestatari.OrderBy(a=>a.IntestazioneNDG))
                    {
                        if (!String.IsNullOrWhiteSpace(datnas_sede_cciaa_albo)) datnas_sede_cciaa_albo += "</w:t><w:br/><w:t>";
                        datnas_sede_cciaa_albo += CaratteriSpeciali(coint.LuogoNascita);
                        if (coint.DataNascita != null)
                        {
                            if (!String.IsNullOrWhiteSpace(datnas_sede_cciaa_albo)) datnas_sede_cciaa_albo += " ";
                            datnas_sede_cciaa_albo = datnas_sede_cciaa_albo + CaratteriSpeciali(Convert.ToDateTime(coint.DataNascita).ToShortDateString());
                        }
                        if (!string.IsNullOrEmpty(coint.IscrizioneCCIAA))
                        {
                            if (!String.IsNullOrWhiteSpace(datnas_sede_cciaa_albo)) datnas_sede_cciaa_albo += " ";
                            datnas_sede_cciaa_albo = datnas_sede_cciaa_albo + CaratteriSpeciali(coint.IscrizioneCCIAA);
                        }
                        if (!string.IsNullOrEmpty(coint.AlboProfessionale))
                        {
                            if (!String.IsNullOrWhiteSpace(datnas_sede_cciaa_albo)) datnas_sede_cciaa_albo += " ";
                            datnas_sede_cciaa_albo = datnas_sede_cciaa_albo + CaratteriSpeciali(coint.AlboProfessionale);
                        }
                    }
                }
                regexText = new Regex("LUOGOEDATADINASCITAPROG");
                docText = regexText.Replace(docText, datnas_sede_cciaa_albo);


                if (!string.IsNullOrEmpty(this.richesta.Filiale))
                {
                    regexText = new Regex("FILIALE_PROG");
                    docText = regexText.Replace(docText, CaratteriSpeciali(descrizione_filiale));

                    try
                    {
                        ResourcesRepository resourcesRepository = new ResourcesRepository();

                        //ErrorSignal.FromCurrentContext().Raise(new Exception("this.richesta.FilialeUOcode " + this.richesta.FilialeUOcode + " this.richesta.Istituto " + this.richesta.Istituto));

                        var cap_Comune = resourcesRepository.GetComuneByUo(this.richesta.FilialeUOcode, this.richesta.Istituto).CAP;
                        //ErrorSignal.FromCurrentContext().Raise(new Exception("cap_Comune " + cap_Comune));

                        var provincia_Comune = resourcesRepository.GetComuneByUo(this.richesta.FilialeUOcode, this.richesta.Istituto).Provincia;
                        var comune = resourcesRepository.GetComuneByUo(this.richesta.FilialeUOcode, this.richesta.Istituto).Comune;
                        //ErrorSignal.FromCurrentContext().Raise(new Exception("provincia_Comune " + provincia_Comune));

                        var indirizzo_Filiale = " ";
                        var infoUocode = resourcesRepository.GetDimUoByUocode(this.richesta.FilialeUOcode).FirstOrDefault();
                        if (infoUocode != null)
                        {
                            indirizzo_Filiale = infoUocode.Indirizzo;
                        }
                        //ErrorSignal.FromCurrentContext().Raise(new Exception("indirizzo_Filiale " + indirizzo_Filiale));


                        regexText = new Regex("INDIRIZZO_FILIALE");
                        docText = regexText.Replace(docText, CaratteriSpeciali(indirizzo_Filiale));

                        regexText = new Regex("CAP_CITTA_PROG");
                        docText = regexText.Replace(docText, CaratteriSpeciali(cap_Comune + " " + comune + " (" + provincia_Comune + ")"));

                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.Message);
                        ErrorSignal.FromCurrentContext().Raise(ex);
                    }

                }
                else
                {
                    //pulisco campi per eventuale compilazione manuale
                    regexText = new Regex("FILIALE_PROG");
                    docText = regexText.Replace(docText, CaratteriSpeciali(descrizione_filiale));

                    regexText = new Regex("INDIRIZZO_FILIALE");
                    docText = regexText.Replace(docText, "");

                    regexText = new Regex("CAP_CITTA_PROG");
                    docText = regexText.Replace(docText, "");
                }


                regexText = new Regex("DATA_PROG");
                docText = regexText.Replace(docText,
                    CaratteriSpeciali(Convert.ToDateTime(this.richesta.DataRichiesta).ToShortDateString()));

                regexText = new Regex("NDG_PROG");
                docText = regexText.Replace(docText, CaratteriSpeciali(this.richesta.NDG.ToString()));



                regexText = new Regex("COGNOME_NOME_PROG");
                docText = regexText.Replace(docText, CaratteriSpeciali(this.richesta.IntestazioneNDG));

                regexText = new Regex("INDIRIZZO_PROG");
                if (!string.IsNullOrEmpty(this.richesta.SedeLegale))
                    docText = regexText.Replace(docText, CaratteriSpeciali(this.richesta.SedeLegale));
                else
                    docText = regexText.Replace(docText, CaratteriSpeciali(this.richesta.Indirizzo));


                regexText = new Regex("RICHIEDENTE_PROG");
                string RICHIEDENTE_PROG = "";
                if (!string.IsNullOrEmpty(this.richesta.Richiedente))
                    RICHIEDENTE_PROG = CaratteriSpeciali(this.richesta.Richiedente);
                else
                {
                    if (this.richesta.Cointestatari == null || this.richesta.Cointestatari.Count <= 0)
                    {
                        RICHIEDENTE_PROG = CaratteriSpeciali(this.richesta.IntestazioneNDG);
                    }
                    else
                    {
                        foreach (var coint in this.richesta.Cointestatari.OrderBy(a => a.IntestazioneNDG))
                        {
                            if (!String.IsNullOrWhiteSpace(RICHIEDENTE_PROG)) RICHIEDENTE_PROG += "<w:br />";
                            RICHIEDENTE_PROG += coint.IntestazioneNDG;

                        }
                    }

                }
                docText = regexText.Replace(docText, RICHIEDENTE_PROG);
                regexText = new Regex("INDIRIZZO_RICH_PROG");

                if (!string.IsNullOrEmpty(this.richesta.IndirizzoRichiedente))
                    docText = regexText.Replace(docText, CaratteriSpeciali(this.richesta.IndirizzoRichiedente));
                else
                    docText = regexText.Replace(docText, CaratteriSpeciali(this.richesta.Indirizzo));



                //regexText = new Regex("COGNOME_NOME_PROG");
                //docText = regexText.Replace(docText, ValidaCampo( this.richesta.IntestazioneNDG));
                ////nel caso ci siano campi richiedente non compilati e sostituiamo con intestazione ndg
                //regexText = new Regex("RICHIEDENTE_PROG");
                //docText = regexText.Replace(docText, ValidaCampo(this.richesta.IntestazioneNDG));


                string cf = CaratteriSpeciali(this.richesta.CodiceFiscale);
                string piva = CaratteriSpeciali(this.richesta.PIva);
                cf = cf + (piva.Trim() != "" && cf.Trim() != "" ? "/" + piva : piva);
                regexText = new Regex("CF_PI_PROG");
                docText = regexText.Replace(docText, CaratteriSpeciali(cf));

                regexText = new Regex("LUOGO_NASCITA_PROG");
                docText = regexText.Replace(docText, CaratteriSpeciali(this.richesta.LuogoNascita));

                regexText = new Regex("DATA_NASCITA_PROG");

                datnas_sede_cciaa_albo = "";
                if (this.richesta.DataNascita != null)
                    datnas_sede_cciaa_albo = datnas_sede_cciaa_albo + Convert.ToDateTime(this.richesta.DataNascita).ToShortDateString();
                if (!string.IsNullOrEmpty(this.richesta.IscrizioneCCIAA))
                    datnas_sede_cciaa_albo = datnas_sede_cciaa_albo + " " + this.richesta.IscrizioneCCIAA;
                if (!string.IsNullOrEmpty(this.richesta.AlboProfessionale))
                    datnas_sede_cciaa_albo = datnas_sede_cciaa_albo + " " + this.richesta.AlboProfessionale;

                docText = regexText.Replace(docText, CaratteriSpeciali(datnas_sede_cciaa_albo));





                //if (string.IsNullOrEmpty( this.richesta.IndirizzoRichiedente))
                //    docText = regexText.Replace(docText, ValidaCampo( this.richesta.Indirizzo));
                //else
                //    docText = regexText.Replace(docText, ValidaCampo(this.richesta.IndirizzoRichiedente));

                regexText = new Regex("TOTALE_DOCUMENTI");
                docText = regexText.Replace(docText,
                    CaratteriSpeciali(String.Format("{0:N2}", this.richesta.ImportoTotale)));

                string pagamento = (this.richesta.ModalitaPagamento == null) ? "" : this.richesta.ModalitaPagamento.Nome.ToUpper();
                regexText = new Regex("CC_PROG");
                docText = regexText.Replace(docText, pagamento == "CONTO" ? "X" : "");

                regexText = new Regex("CONTOCORRENTE_PROG");
                docText = regexText.Replace(docText, CaratteriSpeciali(this.richesta.NumeroConto));

                regexText = new Regex("CAS_PROG");
                docText = regexText.Replace(docText, pagamento == "CASSA" ? "X" : "");


                regexText = new Regex("M_01");
                docText = regexText.Replace(docText, this.richesta.CanaleConsegna == null ? " " : this.richesta.CanaleConsegna.Nome.ToUpper() == "EMAIL" ? "X" : "  ");

                regexText = new Regex("MAIL_CLIENTE_PROG");
                docText = regexText.Replace(docText, CaratteriSpeciali(this.richesta.email));

                regexText = new Regex("M_02");
                docText = regexText.Replace(docText, this.richesta.CanaleConsegna == null ? " " : this.richesta.CanaleConsegna.Nome.ToUpper() == "PEC" ? "X" : "  ");

                regexText = new Regex("PEC_CLIENTE_PROG");
                string pecmail = CaratteriSpeciali(this.richesta.pec).Trim();
                docText = regexText.Replace(docText, pecmail != "" ? "PEC " + CaratteriSpeciali(pecmail) : " ");

                regexText = new Regex("M_03");
                docText = regexText.Replace(docText, this.richesta.CanaleConsegna == null ? " " : this.richesta.CanaleConsegna.Nome.ToUpper() == "FILIALE" ? "X" : "  ");

                //string luogo = string.IsNullOrEmpty(this.richesta.Luogo) ? "" : this.richesta.Luogo;
                regexText = new Regex("LUOGO_PIEPAGINA");
                docText = regexText.Replace(docText, CaratteriSpeciali(luogo));

                regexText = new Regex("DATAOGGI");
                docText = regexText.Replace(docText, CaratteriSpeciali(DateTime.Now.ToShortDateString()));

                string canalerichiesta = this.richesta.CanaleRicezione == null ? "" : this.richesta.CanaleRicezione.Nome.ToUpper();
                string m_04 = "";
                string m_05 = "";
                string m_06 = "";
                string data1 = "";
                string data2 = "";
                string data3 = "";
                switch (canalerichiesta)
                {
                    case "FILIALE":
                        {
                            m_04 = "X";
                            m_05 = "";
                            m_06 = "";
                            data1 = Convert.ToDateTime(this.richesta.DataRichiesta).ToShortDateString();
                            data2 = "";
                            data3 = "";

                            break;
                        }

                    case "PEC":
                        {
                            m_04 = "";
                            m_05 = "X";
                            m_06 = "";
                            data1 = "";
                            data2 = Convert.ToDateTime(this.richesta.DataRichiesta).ToShortDateString();
                            data3 = "";

                            break;
                        }
                    case "RACCOMANDATA":
                        {
                            m_04 = "";
                            m_05 = "";
                            m_06 = "X";
                            data1 = "";
                            data2 = "";
                            data3 = Convert.ToDateTime(this.richesta.DataRichiesta).ToShortDateString();

                            break;
                        }
                }

                regexText = new Regex("M_04");
                docText = regexText.Replace(docText, m_04);
                regexText = new Regex("M_05");
                docText = regexText.Replace(docText, m_05);
                regexText = new Regex("M_06");
                docText = regexText.Replace(docText, m_06);
                regexText = new Regex("DATA_RICHIESTA_1");
                docText = regexText.Replace(docText, data1);
                regexText = new Regex("DATA_RICHIESTA_2");
                docText = regexText.Replace(docText, data2);
                regexText = new Regex("DATA_RICHIESTA_3");
                docText = regexText.Replace(docText, data3);

                regexText = new Regex("DATA_RICEVUTA_DOC");
                docText = regexText.Replace(docText, this.richesta.DataConsegna == null ? " " : CaratteriSpeciali(Convert.ToDateTime(this.richesta.DataConsegna).ToShortDateString()));

                regexText = new Regex("Data_Avviso");
                docText = regexText.Replace(docText, this.richesta.DataAvviso == null ? " " : CaratteriSpeciali(Convert.ToDateTime(this.richesta.DataAvviso).ToShortDateString()));


                //string canaleconsegna = this.richesta.CanaleConsegna.Nome.ToUpper();
                if (canaleconsegna == "FILIALE" &&
                    this.richesta.Filiale == this.richesta.Filiale)
                    canaleconsegna = "in FILIALE";
                if (canaleconsegna == "FILIALE" &&
                    this.richesta.Filiale != this.richesta.Filiale)
                    canaleconsegna = "in " + this.richesta.Filiale;

                if (canaleconsegna == "PEC" || canaleconsegna == "EMAIL")
                    canaleconsegna = "VIA " + canaleconsegna;
                if (canaleconsegna == "RACCOMANDATA")
                    canaleconsegna = "per RACCOMANDATA";
                regexText = new Regex("CANALE_CONSEGNA");
                docText = regexText.Replace(docText, CaratteriSpeciali(canaleconsegna));

                using (StreamWriter sw = new StreamWriter(wordDoc.MainDocumentPart.GetStream(FileMode.Create)))
                {
                    sw.Write(docText);
                }
                var result = CompilaTabellaRichiedente(wordDoc);
                result = CompilaTabellaIntestatari(wordDoc);
                result = CompilaTabellaDocumenti(wordDoc);
                result = CompilaTabellaDocumentiCartone(wordDoc);
                result = CompilaTabellaDocumentiEsteso(wordDoc);
            }
            catch (Exception ex)
            {
                //System.Diagnostics.Debug.WriteLine(ex.Message);
                ErrorSignal.FromCurrentContext().Raise(ex);
                return false;
            }
            return true;
        }
        public bool CompilaTabellaRichiedente(WordprocessingDocument wordDoc)
        {
            try
            {

                TableRow row;
                Body bod = wordDoc.MainDocumentPart.Document.Body;
                foreach (Table t in bod.Descendants<Table>().Where(t => t.InnerXml.Contains("TABELLA_RICHIEDENTE")))
                {
                    var numprimariga = t.Elements<TableRow>().Count() - 3;
                    var primarigavuota = (TableRow)t.Elements<TableRow>().ElementAt(numprimariga).Clone();

                    row = t.Elements<TableRow>().ElementAt(numprimariga);
                    if (!string.IsNullOrEmpty(this.richesta.Richiedente))
                        row.ElementAt(0).Elements<DocumentFormat.OpenXml.Wordprocessing.Paragraph>().First().Elements<Run>().First().Elements<Text>().First().Text = CaratteriSpeciali(this.richesta.Richiedente);
                    else
                    {
                        if (this.richesta.Cointestatari == null || this.richesta.Cointestatari.Count <= 0)
                        {
                            row.ElementAt(0).Elements<DocumentFormat.OpenXml.Wordprocessing.Paragraph>().First().Elements<Run>().First().Elements<Text>().First().Text = CaratteriSpeciali(this.richesta.IntestazioneNDG);
                        }
                        else
                        {
                            foreach (var coint in this.richesta.Cointestatari.OrderBy(a => a.IntestazioneNDG))
                            {
                                row = t.Elements<TableRow>().ElementAt(numprimariga);
                                row.ElementAt(0).Elements<DocumentFormat.OpenXml.Wordprocessing.Paragraph>().First().Elements<Run>().First().Elements<Text>().First().Text = CaratteriSpeciali(coint.IntestazioneNDG);
                                if (coint != this.richesta.Cointestatari.OrderBy(a => a.IntestazioneNDG).Last())
                                {
                                    t.InsertAfter(primarigavuota, row);
                                    numprimariga++;
                                }

                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                //System.Diagnostics.Debug.WriteLine(ex.Message);
                ErrorSignal.FromCurrentContext().Raise(ex);
                return false;
            }
            return true;
        }
        public bool CompilaTabellaIntestatari(WordprocessingDocument wordDoc)
        {
            try
            {
                TableRow row;
                Body bod = wordDoc.MainDocumentPart.Document.Body;
                foreach (Table t in bod.Descendants<Table>().Where(t => t.InnerXml.Contains("TABELLA_INTESTATARI")))
                {
                    var numprimariga = t.Elements<TableRow>().Count() - 3;

                    var primarigavuota = (TableRow)t.Elements<TableRow>().ElementAt(numprimariga).Clone();

                    if (this.richesta.Cointestatari == null || this.richesta.Cointestatari.Count <= 0)
                    {
                        row = t.Elements<TableRow>().ElementAt(numprimariga);
                        row.ElementAt(0).Elements<DocumentFormat.OpenXml.Wordprocessing.Paragraph>().First().Elements<Run>().First().Elements<Text>().First().Text = CaratteriSpeciali(this.richesta.IntestazioneNDG);
                        string cf = CaratteriSpeciali(this.richesta.CodiceFiscale);
                        string piva = CaratteriSpeciali(this.richesta.PIva);
                        cf = cf + (piva.Trim() != "" && cf.Trim() != "" ? "/" + piva : piva);
                        row.ElementAt(1).Elements<DocumentFormat.OpenXml.Wordprocessing.Paragraph>().First().Elements<Run>().First().Elements<Text>().First().Text = CaratteriSpeciali(cf);
                        string datnas_sede_cciaa_albo = "";
                        if (this.richesta.DataNascita != null)
                        {
                            if (!String.IsNullOrWhiteSpace(datnas_sede_cciaa_albo)) datnas_sede_cciaa_albo += " ";
                            datnas_sede_cciaa_albo = datnas_sede_cciaa_albo + CaratteriSpeciali(Convert.ToDateTime(this.richesta.DataNascita).ToShortDateString());
                        }
                        if (!string.IsNullOrEmpty(this.richesta.IscrizioneCCIAA))
                        {
                            if (!String.IsNullOrWhiteSpace(datnas_sede_cciaa_albo)) datnas_sede_cciaa_albo += " ";
                            datnas_sede_cciaa_albo = datnas_sede_cciaa_albo + CaratteriSpeciali(this.richesta.IscrizioneCCIAA);
                        }
                        if (!string.IsNullOrEmpty(this.richesta.AlboProfessionale))
                        {
                            if (!String.IsNullOrWhiteSpace(datnas_sede_cciaa_albo)) datnas_sede_cciaa_albo += " ";
                            datnas_sede_cciaa_albo = datnas_sede_cciaa_albo + CaratteriSpeciali(this.richesta.AlboProfessionale);
                        }
                        row.ElementAt(2).Elements<DocumentFormat.OpenXml.Wordprocessing.Paragraph>().First().Elements<Run>().First().Elements<Text>().First().Text = datnas_sede_cciaa_albo;
                    }
                    else
                    {
                        foreach (var coint in this.richesta.Cointestatari.OrderBy(a => a.IntestazioneNDG))
                        {
                            row = t.Elements<TableRow>().ElementAt(numprimariga);
                            row.ElementAt(0).Elements<DocumentFormat.OpenXml.Wordprocessing.Paragraph>().First().Elements<Run>().First().Elements<Text>().First().Text = CaratteriSpeciali(coint.IntestazioneNDG);
                            string cf = CaratteriSpeciali(coint.CodiceFiscale);
                            string piva = CaratteriSpeciali(coint.PIva);
                            cf = cf + (piva.Trim() != "" && cf.Trim() != "" ? "/" + piva : piva);
                            row.ElementAt(1).Elements<DocumentFormat.OpenXml.Wordprocessing.Paragraph>().First().Elements<Run>().First().Elements<Text>().First().Text = CaratteriSpeciali(cf);
                            string datnas_sede_cciaa_albo = "";
                            if (coint.DataNascita != null)
                            {
                                if (!String.IsNullOrWhiteSpace(datnas_sede_cciaa_albo)) datnas_sede_cciaa_albo += " ";
                                datnas_sede_cciaa_albo = datnas_sede_cciaa_albo + CaratteriSpeciali(Convert.ToDateTime(coint.DataNascita).ToShortDateString());
                            }
                            if (!string.IsNullOrEmpty(coint.IscrizioneCCIAA))
                            {
                                if (!String.IsNullOrWhiteSpace(datnas_sede_cciaa_albo)) datnas_sede_cciaa_albo += " ";
                                datnas_sede_cciaa_albo = datnas_sede_cciaa_albo + CaratteriSpeciali(coint.IscrizioneCCIAA);
                            }
                            if (!string.IsNullOrEmpty(coint.AlboProfessionale))
                            {
                                if (!String.IsNullOrWhiteSpace(datnas_sede_cciaa_albo)) datnas_sede_cciaa_albo += " ";
                                datnas_sede_cciaa_albo = datnas_sede_cciaa_albo + CaratteriSpeciali(coint.AlboProfessionale);
                            }
                            row.ElementAt(2).Elements<DocumentFormat.OpenXml.Wordprocessing.Paragraph>().First().Elements<Run>().First().Elements<Text>().First().Text = datnas_sede_cciaa_albo;
                            if (coint != this.richesta.Cointestatari.OrderBy(a => a.IntestazioneNDG).Last())
                            {
                                t.InsertAfter(primarigavuota, row);
                                numprimariga++;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //System.Diagnostics.Debug.WriteLine(ex.Message);
                ErrorSignal.FromCurrentContext().Raise(ex);
                return false;
            }
            return true;
        }
        public bool CompilaTabellaDocumenti(WordprocessingDocument wordDoc)
        {
            try
            {
                TableRow row;
                Body bod = wordDoc.MainDocumentPart.Document.Body;
                //aggiunge una riga
                int numriga = 0;

                TableRow newriga;
                string des_rap;

                foreach (Table t in bod.Descendants<Table>().Where(t => t.InnerXml.Contains("TABELLA_DOCUMENTI_SEMPLICE")))
                {
                    numriga = 0;
                    foreach (DocumentiRichiesti doc_ric in this.richesta.DocumentiRichiesti)
                    {
                        numriga += 1;
                        newriga = (TableRow)t.Elements<TableRow>().ElementAt(t.Elements<TableRow>().Count() - 1).Clone();

                        row = t.Elements<TableRow>().ElementAt(t.Elements<TableRow>().Count() - 1);//prima riga vuota
                        row.ElementAt(1).Elements<DocumentFormat.OpenXml.Wordprocessing.Paragraph>().First().Elements<Run>().First().Elements<Text>()
                            .First().Text = doc_ric.NumCopie.ToString();

                        //des_rap = string.IsNullOrEmpty(doc_ric.Rapporto) ? "" : " " + doc_ric.Rapporto;
                        //row.ElementAt(2).Elements<DocumentFormat.OpenXml.Wordprocessing.Paragraph>().First().Elements<Run>().First().Elements<Text>()
                        //    .First().Text = doc_ric.DescrizioneDocumento + des_rap;
                        row.ElementAt(2).Elements<DocumentFormat.OpenXml.Wordprocessing.Paragraph>().First().Elements<Run>().First().Elements<Text>()
                        .First().Text = CaratteriSpeciali(doc_ric.DescrizioneDocumento);

                        des_rap = string.IsNullOrEmpty(doc_ric.Rapporto) ? "" : " " + doc_ric.Rapporto;
                        row.ElementAt(2).Elements<DocumentFormat.OpenXml.Wordprocessing.Paragraph>().First().Elements<Run>().ElementAt(1).Elements<Text>()
                            .First().Text = "Rapporto n. " + CaratteriSpeciali(des_rap);

                        row.ElementAt(3).Elements<DocumentFormat.OpenXml.Wordprocessing.Paragraph>().First().Elements<Run>().First().Elements<Text>()
                                .First().Text = "€ " + String.Format("{0:#.00}", doc_ric.Costo);

                        if (numriga != this.richesta.DocumentiRichiesti.Count)
                            t.Append(newriga);
                    }

                }
            }
            catch (Exception ex)
            {
                //System.Diagnostics.Debug.WriteLine(ex.Message);
                ErrorSignal.FromCurrentContext().Raise(ex);
                return false;
            }
            return true;
        }
        public bool CompilaTabellaDocumentiCartone(WordprocessingDocument wordDoc)
        {
            try
            {
                TableRow row;
                Body bod = wordDoc.MainDocumentPart.Document.Body;
                //aggiunge una riga
                int numriga = 0;

                TableRow newriga;
                string des_rap, cartone;

                foreach (Table t in bod.Descendants<Table>().Where(t => t.InnerXml.Contains("TABELLA_DOCUMENTI_CARTONE")))
                {
                    foreach (DocumentiRichiesti doc_ric in this.richesta.DocumentiRichiesti)
                    {
                        numriga += 1;
                        newriga = (TableRow)t.Elements<TableRow>().ElementAt(t.Elements<TableRow>().Count() - 1).Clone();

                        row = t.Elements<TableRow>().ElementAt(t.Elements<TableRow>().Count() - 1);//prima riga vuota
                        row.ElementAt(1).Elements<DocumentFormat.OpenXml.Wordprocessing.Paragraph>().First().Elements<Run>().First().Elements<Text>()
                            .First().Text = doc_ric.NumCopie.ToString();

                        row.ElementAt(2).Elements<DocumentFormat.OpenXml.Wordprocessing.Paragraph>().First().Elements<Run>().First().Elements<Text>()
                            .First().Text = CaratteriSpeciali(doc_ric.DescrizioneDocumento);

                        des_rap = string.IsNullOrEmpty(doc_ric.Rapporto) ? "" : " " + doc_ric.Rapporto;
                        row.ElementAt(2).Elements<DocumentFormat.OpenXml.Wordprocessing.Paragraph>().First().Elements<Run>().ElementAt(1).Elements<Text>()
                            .First().Text = "Rapporto n. " + CaratteriSpeciali(des_rap);

                        row.ElementAt(3).Elements<DocumentFormat.OpenXml.Wordprocessing.Paragraph>().First().Elements<Run>().First().Elements<Text>()
                                .First().Text = doc_ric.DocOrig ? "X" : "";

                        row.ElementAt(4).Elements<DocumentFormat.OpenXml.Wordprocessing.Paragraph>().First().Elements<Run>().First().Elements<Text>()
                                .First().Text = !doc_ric.DocOrig ? "X" : "";

                        cartone = doc_ric.NumCartone == null ? "" : doc_ric.NumCartone.ToString();
                        row.ElementAt(6).Elements<DocumentFormat.OpenXml.Wordprocessing.Paragraph>().First().Elements<Run>().First().Elements<Text>()
                            .First().Text = CaratteriSpeciali(cartone);

                        //table.Rows.Add();
                        if (numriga != this.richesta.DocumentiRichiesti.Count)
                            t.Append(newriga);
                    }

                }
            }
            catch (Exception ex)
            {
                //System.Diagnostics.Debug.WriteLine(ex.Message);
                ErrorSignal.FromCurrentContext().Raise(ex);
                return false;
            }
            return true;
        }
        public bool CompilaTabellaDocumentiEsteso(WordprocessingDocument wordDoc)
        {
            try
            {
                TableRow row;
                Body bod = wordDoc.MainDocumentPart.Document.Body;
                //aggiunge una riga
                int numriga = 0;

                TableRow newriga;
                string des_rap;

                foreach (Table t in bod.Descendants<Table>().Where(t => t.InnerXml.Contains("TABELLA_DOCUMENTI_ESTESO")))
                {
                    foreach (DocumentiRichiesti doc_ric in this.richesta.DocumentiRichiesti)
                    {
                        numriga += 1;
                        newriga = (TableRow)t.Elements<TableRow>().ElementAt(t.Elements<TableRow>().Count() - 1).Clone();

                        row = t.Elements<TableRow>().ElementAt(t.Elements<TableRow>().Count() - 1);//prima riga vuota
                        row.ElementAt(1).Elements<DocumentFormat.OpenXml.Wordprocessing.Paragraph>().First().Elements<Run>().First().Elements<Text>()
                            .First().Text = doc_ric.NumCopie.ToString();

                        row.ElementAt(2).Elements<DocumentFormat.OpenXml.Wordprocessing.Paragraph>().First().Elements<Run>().First().Elements<Text>()
                            .First().Text = CaratteriSpeciali(doc_ric.DescrizioneDocumento);

                        des_rap = string.IsNullOrEmpty(doc_ric.Rapporto) ? "" : " " + doc_ric.Rapporto;
                        row.ElementAt(2).Elements<DocumentFormat.OpenXml.Wordprocessing.Paragraph>().First().Elements<Run>().ElementAt(1).Elements<Text>()
                            .First().Text = "Rapporto n. " + CaratteriSpeciali(des_rap);

                        row.ElementAt(3).Elements<DocumentFormat.OpenXml.Wordprocessing.Paragraph>().First().Elements<Run>().First().Elements<Text>()
                                .First().Text = doc_ric.DocOrig ? "X" : "";

                        row.ElementAt(4).Elements<DocumentFormat.OpenXml.Wordprocessing.Paragraph>().First().Elements<Run>().First().Elements<Text>()
                                .First().Text = !doc_ric.DocOrig ? "X" : "";

                        //table.Rows.Add();
                        if (numriga != this.richesta.DocumentiRichiesti.Count)
                            t.Append(newriga);
                    }

                }
            }
            catch (Exception ex)
            {
                //System.Diagnostics.Debug.WriteLine(ex.Message);
                ErrorSignal.FromCurrentContext().Raise(ex);
                return false;
            }
            return true;
        }


        public byte[] Compila_Richiesta()
        {
            //return new byte[0];
            var file_memoria = new MemoryStream();

            try
            {
                string nomefile = "Modulo_Richiesta_Documentazione.docx";
                object m = System.Reflection.Missing.Value;
                object oldFileName;


                oldFileName = fullpath + nomefile;
                Directory.CreateDirectory(filetemp);
                filetemp += DateTime.UtcNow.Ticks + @".docx";
                File.Copy(oldFileName.ToString(), filetemp);

                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(filetemp, true))
                {

                    var result = this.AssegnaCampi(wordDoc);

                    wordDoc.Save();
                    wordDoc.Close();
                }

                Stream stream = File.Open(filetemp, FileMode.Open);
                file_memoria = new MemoryStream();
                stream.CopyTo(file_memoria);
                stream.Close();
                File.Delete(filetemp);
            }
            catch (Exception ex)
            {
                //System.Diagnostics.Debug.WriteLine(ex.Message);
                ErrorSignal.FromCurrentContext().Raise(ex);
            }

            return file_memoria.ToArray();
        }
        public byte[] Compila_Consegna()
        {
            var file_memoria = new MemoryStream();
            try
            {
                string nomefile = "Ricevuta_consegna_copia_documentazione.docx";
                object m = System.Reflection.Missing.Value;
                object oldFileName;
                oldFileName = fullpath + nomefile;


                Directory.CreateDirectory(filetemp);
                filetemp += DateTime.UtcNow.Ticks + @".docx";
                File.Copy(oldFileName.ToString(), filetemp);

                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(filetemp, true))
                {
                    var result = this.AssegnaCampi(wordDoc);

                    wordDoc.Save();
                    wordDoc.Close();
                }


                Stream stream = File.Open(filetemp, FileMode.Open);
                file_memoria = new MemoryStream();
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
                object nomefile = "Lettera_Disposizione_Addebito_Spese.docx";
                object m = System.Reflection.Missing.Value;
                object oldFileName;
                oldFileName = fullpath + nomefile;


                Directory.CreateDirectory(filetemp);
                filetemp += DateTime.UtcNow.Ticks + @".docx";
                File.Copy(oldFileName.ToString(), filetemp);

                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(filetemp, true))
                {
                    var result = this.AssegnaCampi(wordDoc);

                    ////aggiunge una riga
                    //int numriga = 0;
                    //TableRow row;
                    //TableRow newriga;
                    //string des_rap;
                    //Body bod = wordDoc.MainDocumentPart.Document.Body;
                    //foreach (Table t in bod.Descendants<Table>().Where(t => t.InnerXml.Contains("TABELLA_ALLEGATI")))
                    //{
                    //    foreach (DocumentiRichiesti doc_ric in this.richesta.DocumentiRichiesti)
                    //    {
                    //        numriga += 1;
                    //        newriga = (TableRow)t.Elements<TableRow>().ElementAt(t.Elements<TableRow>().Count() - 1).Clone();

                    //        row = t.Elements<TableRow>().ElementAt(t.Elements<TableRow>().Count() - 1);//prima riga vuota
                    //        row.ElementAt(1).Elements<DocumentFormat.OpenXml.Wordprocessing.Paragraph>().First().Elements<Run>().First().Elements<Text>()
                    //            .First().Text = doc_ric.NumCopie.ToString();

                    //        //des_rap = string.IsNullOrEmpty(doc_ric.Rapporto) ? "" : " " + doc_ric.Rapporto;
                    //        //row.ElementAt(2).Elements<DocumentFormat.OpenXml.Wordprocessing.Paragraph>().First().Elements<Run>().First().Elements<Text>()
                    //        //    .First().Text = doc_ric.DescrizioneDocumento + des_rap;
                    //        row.ElementAt(2).Elements<DocumentFormat.OpenXml.Wordprocessing.Paragraph>().First().Elements<Run>().First().Elements<Text>()
                    //        .First().Text = CaratteriSpeciali(doc_ric.DescrizioneDocumento);

                    //        des_rap = string.IsNullOrEmpty(doc_ric.Rapporto) ? "" : " " + doc_ric.Rapporto;
                    //        row.ElementAt(2).Elements<DocumentFormat.OpenXml.Wordprocessing.Paragraph>().First().Elements<Run>().ElementAt(1).Elements<Text>()
                    //            .First().Text = "Rapporto n. " + CaratteriSpeciali(des_rap);


                    //        row.ElementAt(3).Elements<DocumentFormat.OpenXml.Wordprocessing.Paragraph>().First().Elements<Run>().First().Elements<Text>()
                    //                .First().Text = "€ " + String.Format("{0:#.00}", doc_ric.Costo);

                    //        if (numriga != this.richesta.DocumentiRichiesti.Count)
                    //            t.Append(newriga);
                    //    }

                    //}

                    wordDoc.Save();
                    wordDoc.Close();
                }

                Stream stream = File.Open(filetemp, FileMode.Open);
                file_memoria = new MemoryStream();
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
                object nomefile = "Avviso_Documenti_in_Giacenza_1.docx";
                object m = System.Reflection.Missing.Value;
                object oldFileName;
                oldFileName = fullpath + nomefile;


                Directory.CreateDirectory(filetemp);
                filetemp += DateTime.UtcNow.Ticks + @".docx";
                File.Copy(oldFileName.ToString(), filetemp);

                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(filetemp, true))
                {
                    var result = this.AssegnaCampi(wordDoc);

                    ////aggiunge una riga
                    //int numriga = 0;
                    //TableRow row;
                    //TableRow newriga;
                    //string des_rap;
                    //Body bod = wordDoc.MainDocumentPart.Document.Body;
                    //foreach (Table t in bod.Descendants<Table>().Where(t => t.InnerXml.Contains("TABELLA_ALLEGATI")))
                    //{
                    //    numriga = 0;
                    //    foreach (DocumentiRichiesti doc_ric in this.richesta.DocumentiRichiesti)
                    //    {
                    //        numriga += 1;
                    //        newriga = (TableRow)t.Elements<TableRow>().ElementAt(t.Elements<TableRow>().Count() - 1).Clone();

                    //        row = t.Elements<TableRow>().ElementAt(t.Elements<TableRow>().Count() - 1);//prima riga vuota
                    //        row.ElementAt(1).Elements<DocumentFormat.OpenXml.Wordprocessing.Paragraph>().First().Elements<Run>().First().Elements<Text>()
                    //            .First().Text = doc_ric.NumCopie.ToString();

                    //        //des_rap = string.IsNullOrEmpty(doc_ric.Rapporto) ? "" : " " + doc_ric.Rapporto;
                    //        //row.ElementAt(2).Elements<DocumentFormat.OpenXml.Wordprocessing.Paragraph>().First().Elements<Run>().First().Elements<Text>()
                    //        //    .First().Text = doc_ric.DescrizioneDocumento + des_rap;
                    //        row.ElementAt(2).Elements<DocumentFormat.OpenXml.Wordprocessing.Paragraph>().First().Elements<Run>().First().Elements<Text>()
                    //        .First().Text = CaratteriSpeciali(doc_ric.DescrizioneDocumento);

                    //        des_rap = string.IsNullOrEmpty(doc_ric.Rapporto) ? "" : " " + doc_ric.Rapporto;
                    //        row.ElementAt(2).Elements<DocumentFormat.OpenXml.Wordprocessing.Paragraph>().First().Elements<Run>().ElementAt(1).Elements<Text>()
                    //            .First().Text = "Rapporto n. " + CaratteriSpeciali(des_rap);

                    //        row.ElementAt(3).Elements<DocumentFormat.OpenXml.Wordprocessing.Paragraph>().First().Elements<Run>().First().Elements<Text>()
                    //                .First().Text = "€ " + String.Format("{0:#.00}", doc_ric.Costo);

                    //        if (numriga != this.richesta.DocumentiRichiesti.Count)
                    //            t.Append(newriga);
                    //    }

                    //}

                    wordDoc.Save();
                    wordDoc.Close();
                }

                Stream stream = File.Open(filetemp, FileMode.Open);
                file_memoria = new MemoryStream();
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
        public byte[] Compila_ModDocumentiGiacenza2()
        {
            var file_memoria = new MemoryStream();

            try
            {
                object nomefile = "Avviso_Documenti_in_Giacenza_2.docx";
                object m = System.Reflection.Missing.Value;
                object oldFileName;
                oldFileName = fullpath + nomefile;


                Directory.CreateDirectory(filetemp);
                filetemp += DateTime.UtcNow.Ticks + @".docx";
                File.Copy(oldFileName.ToString(), filetemp);

                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(filetemp, true))
                {
                    var result = this.AssegnaCampi(wordDoc);




                    wordDoc.Save();
                    wordDoc.Close();
                }

                Stream stream = File.Open(filetemp, FileMode.Open);
                file_memoria = new MemoryStream();
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
                object nomefile = "Modulo_Invio_Documentazione.docx";
                object m = System.Reflection.Missing.Value;
                object oldFileName;
                oldFileName = fullpath + nomefile;


                Directory.CreateDirectory(filetemp);
                filetemp += DateTime.UtcNow.Ticks + @".docx";
                File.Copy(oldFileName.ToString(), filetemp);

                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(filetemp, true))
                {
                    var result = this.AssegnaCampi(wordDoc);

                    ////aggiunge una riga
                    //int numriga = 0;
                    //TableRow row;
                    //TableRow newriga;
                    //string des_rap;
                    //Body bod = wordDoc.MainDocumentPart.Document.Body;
                    //foreach (Table t in bod.Descendants<Table>().Where(t => t.InnerXml.Contains("TABELLA_ALLEGATI")))
                    //{
                    //    foreach (DocumentiRichiesti doc_ric in this.richesta.DocumentiRichiesti)
                    //    {
                    //        numriga += 1;
                    //        newriga = (TableRow)t.Elements<TableRow>().ElementAt(t.Elements<TableRow>().Count() - 1).Clone();

                    //        row = t.Elements<TableRow>().ElementAt(t.Elements<TableRow>().Count() - 1);//prima riga vuota
                    //        row.ElementAt(1).Elements<DocumentFormat.OpenXml.Wordprocessing.Paragraph>().First().Elements<Run>().First().Elements<Text>()
                    //            .First().Text = doc_ric.NumCopie.ToString();

                    //        //des_rap = string.IsNullOrEmpty(doc_ric.Rapporto) ? "" : " " + doc_ric.Rapporto;
                    //        //row.ElementAt(2).Elements<DocumentFormat.OpenXml.Wordprocessing.Paragraph>().First().Elements<Run>().First().Elements<Text>()
                    //        //    .First().Text = doc_ric.DescrizioneDocumento + des_rap;
                    //        row.ElementAt(2).Elements<DocumentFormat.OpenXml.Wordprocessing.Paragraph>().First().Elements<Run>().First().Elements<Text>()
                    //        .First().Text = CaratteriSpeciali(doc_ric.DescrizioneDocumento);

                    //        des_rap = string.IsNullOrEmpty(doc_ric.Rapporto) ? "" : " " + doc_ric.Rapporto;
                    //        row.ElementAt(2).Elements<DocumentFormat.OpenXml.Wordprocessing.Paragraph>().First().Elements<Run>().ElementAt(1).Elements<Text>()
                    //            .First().Text = "Rapporto n. " + CaratteriSpeciali(des_rap);

                    //        row.ElementAt(3).Elements<DocumentFormat.OpenXml.Wordprocessing.Paragraph>().First().Elements<Run>().First().Elements<Text>()
                    //                .First().Text = "€ " + String.Format("{0:#.00}", doc_ric.Costo);

                    //        if (numriga != this.richesta.DocumentiRichiesti.Count)
                    //            t.Append(newriga);
                    //    }

                    //}

                    wordDoc.Save();
                    wordDoc.Close();
                }

                Stream stream = File.Open(filetemp, FileMode.Open);
                file_memoria = new MemoryStream();
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