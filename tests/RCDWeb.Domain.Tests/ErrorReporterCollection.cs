using Xunit;

namespace RCDWeb.Domain.Tests
{
    /// <summary>
    /// <see cref="RCDWeb.Domain.Abstractions.ErrorReporter"/> è statico: i test che lo
    /// sostituiscono devono essere eseguiti in serie, altrimenti si sovrascrivono a vicenda.
    /// </summary>
    [CollectionDefinition("ErrorReporter", DisableParallelization = true)]
    public sealed class ErrorReporterCollection
    {
    }
}
