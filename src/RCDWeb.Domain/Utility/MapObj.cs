using System;
using System.Reflection;
using RCDWeb.Domain.Abstractions;

namespace RCDWeb.Domain.Utility
{
    /// <summary>
    /// Copia le proprietà di un oggetto sorgente su un oggetto destinazione confrontando
    /// i nomi senza distinguere maiuscole/minuscole.
    /// Porting cross-platform di <c>RCDWeb.Areas.RCD.Uility.MapObj</c>, con la dipendenza
    /// da ELMAH sostituita da <see cref="IErrorReporter"/>.
    /// </summary>
    public static class MapObj
    {
        /// <summary>
        /// Copia su <paramref name="Destinazione"/> i valori delle proprietà di
        /// <paramref name="Sorgente"/> che hanno lo stesso nome (case-insensitive).
        /// Se la destinazione è <c>null</c> ne viene creata una nuova istanza.
        /// Le proprietà non convertibili restano invariate e l'eccezione viene segnalata
        /// al reporter senza interrompere la copia.
        /// </summary>
        public static T ObjectTo<T>(object Sorgente, T Destinazione = default(T))
        {
            if (Destinazione == null) Destinazione = Activator.CreateInstance<T>();
            if (Sorgente == null) return Destinazione;

            var sourceType = Sorgente.GetType();
            var destinationType = Destinazione.GetType();
            var destinationProperties = destinationType.GetProperties();

            foreach (var entry in sourceType.GetProperties())
            {
                var propertySorgente = sourceType.GetProperty(entry.Name);

                PropertyInfo propertyDestinazione = null;
                foreach (var prop in destinationProperties)
                {
                    if (String.Equals(entry.Name, prop.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        propertyDestinazione = prop;
                        break;
                    }
                }

                if (propertySorgente == null || propertyDestinazione == null) continue;
                if (!propertyDestinazione.CanWrite) continue;

                object valoreSorgente;
                try
                {
                    valoreSorgente = propertySorgente.GetValue(Sorgente, null);
                }
                catch (Exception ex)
                {
                    ErrorReporter.Report(ex);
                    continue;
                }

                try
                {
                    propertyDestinazione.SetValue(Destinazione, valoreSorgente, null);
                }
                catch (Exception ex)
                {
                    ErrorReporter.Report(ex);

                    try
                    {
                        if (valoreSorgente != null)
                        {
                            var tipoDestinazione = propertyDestinazione.PropertyType;
                            var underlying = Nullable.GetUnderlyingType(tipoDestinazione);
                            if (underlying != null) tipoDestinazione = underlying;

                            var valoreConvertito = Convert.ChangeType(valoreSorgente, tipoDestinazione);
                            propertyDestinazione.SetValue(Destinazione, valoreConvertito, null);
                        }
                        else
                        {
                            propertyDestinazione.SetValue(Destinazione, null, null);
                        }
                    }
                    catch (Exception conversionException)
                    {
                        ErrorReporter.Report(conversionException);
                    }
                }
            }

            return Destinazione;
        }
    }
}
