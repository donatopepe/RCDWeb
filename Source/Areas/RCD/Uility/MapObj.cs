using Elmah;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace RCDWeb.Areas.RCD.Uility
{
    public static class MapObj
    {
        public static T ObjectTo<T>(object Sorgente, T Destinazione = default)
        {
            if (Destinazione == null) Destinazione = Activator.CreateInstance<T>();

            var propertiesSorgente = Sorgente.GetType().GetProperties();
            if (propertiesSorgente == null)
                return Destinazione;

            foreach (var entry in propertiesSorgente)
            {
                var propertySorgente = Sorgente.GetType().GetProperty(entry.Name);

                PropertyInfo propertyDestinazione = null;
                foreach (var prop in Destinazione.GetType().GetProperties())
                {
                    if (entry.Name.ToLower() == prop.Name.ToLower())
                    {
                        propertyDestinazione = prop;
                        break;
                    }
                }
                //var propertyDestinazione = Destinazione.GetType().GetProperty(entry.Name);
                if ((propertySorgente != null) && (propertyDestinazione != null))
                {
                    var valoreSorgente = propertySorgente.GetValue(Sorgente);

                    try
                    {
                        propertyDestinazione.SetValue(Destinazione, valoreSorgente, null);
                    }
                    catch
                    {
                        try
                        {
                            if (valoreSorgente != null)
                            {
                                var tipoDestinazione = propertyDestinazione.GetMethod.ReturnType;
                                if (Nullable.GetUnderlyingType(tipoDestinazione) != null)
                                {
                                    tipoDestinazione = Nullable.GetUnderlyingType(tipoDestinazione);
                                }
                                var valoreConvertito = Convert.ChangeType(valoreSorgente, tipoDestinazione);
                                propertyDestinazione.SetValue(Destinazione, valoreConvertito, null);
                            }
                            else
                            {
                                propertyDestinazione.SetValue(Destinazione, valoreSorgente, null);
                            }
                        }
                        catch (Exception ex) {
                            ErrorSignal.FromCurrentContext().Raise(ex);
                        }
                    }

                }

            }
            return Destinazione;
        }
    }
}