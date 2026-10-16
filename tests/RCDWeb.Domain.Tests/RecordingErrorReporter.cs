using System;
using System.Collections.Generic;
using RCDWeb.Domain.Abstractions;

namespace RCDWeb.Domain.Tests
{
    /// <summary>
    /// Segnalatore di errori di test: registra le eccezioni segnalate invece di scartarle.
    /// </summary>
    public sealed class RecordingErrorReporter : IErrorReporter
    {
        public List<Exception> Reported { get; } = new List<Exception>();

        public int Count { get { return Reported.Count; } }

        public void Report(Exception exception)
        {
            Reported.Add(exception);
        }
    }
}
