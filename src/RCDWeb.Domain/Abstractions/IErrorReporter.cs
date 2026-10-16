using System;

namespace RCDWeb.Domain.Abstractions
{
    /// <summary>
    /// Astrazione del segnalatore di errori.
    /// Nel progetto web legacy questa interfaccia è implementata da ELMAH
    /// (<c>Elmah.ErrorSignal.FromCurrentContext().Raise</c>); nella libreria di dominio
    /// non esiste alcuna dipendenza da ELMAH o da <c>System.Web</c>.
    /// </summary>
    public interface IErrorReporter
    {
        /// <summary>Segnala un errore non fatale.</summary>
        void Report(Exception exception);
    }

    /// <summary>Implementazione che scarta le segnalazioni (usata come default).</summary>
    public sealed class NullErrorReporter : IErrorReporter
    {
        /// <summary>Istanza condivisa.</summary>
        public static readonly NullErrorReporter Instance = new NullErrorReporter();

        private NullErrorReporter() { }

        /// <inheritdoc />
        public void Report(Exception exception) { }
    }

    /// <summary>
    /// Punto di accesso statico al segnalatore di errori corrente.
    /// Essendo statico, l'host applicativo deve registrare la propria implementazione
    /// una sola volta all'avvio; se non viene registrata nulla, le segnalazioni vengono scartate.
    /// </summary>
    public static class ErrorReporter
    {
        private static volatile IErrorReporter _current = NullErrorReporter.Instance;

        /// <summary>Segnalatore corrente. Impostare <c>null</c> ripristina <see cref="NullErrorReporter"/>.</summary>
        public static IErrorReporter Current
        {
            get { return _current; }
            set { _current = value ?? NullErrorReporter.Instance; }
        }

        /// <summary>Segnala un errore con il segnalatore corrente.</summary>
        public static void Report(Exception exception)
        {
            if (exception == null) return;
            _current.Report(exception);
        }

        /// <summary>Ripristina il segnalatore predefinito (utile nei test).</summary>
        public static void Reset()
        {
            _current = NullErrorReporter.Instance;
        }
    }
}
