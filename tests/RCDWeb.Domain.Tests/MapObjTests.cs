using RCDWeb.Domain.Abstractions;
using RCDWeb.Domain.Utility;
using Xunit;

namespace RCDWeb.Domain.Tests
{
    [Collection("ErrorReporter")]
    public class MapObjTests
    {
        private class Source
        {
            public string Nome { get; set; }
            public int Eta { get; set; }
            public string Solo { get; set; }
        }

        private class Destination
        {
            public string nome { get; set; }
            public int Eta { get; set; }
            public string Altro { get; set; }
        }

        private class TypedSource
        {
            public string Numero { get; set; }
        }

        private class TypedDestination
        {
            public int Numero { get; set; }
        }

        [Fact]
        public void ObjectTo_CopiesMatchingProperties_CaseInsensitive()
        {
            var source = new Source { Nome = "Mario", Eta = 42, Solo = "x" };

            var result = MapObj.ObjectTo(source, new Destination());

            Assert.Equal("Mario", result.nome);
            Assert.Equal(42, result.Eta);
            Assert.Null(result.Altro);
        }

        [Fact]
        public void ObjectTo_CreatesDestinationWhenNull()
        {
            var result = MapObj.ObjectTo<Source>(new Source { Nome = "Mario" });

            Assert.NotNull(result);
            Assert.Equal("Mario", result.Nome);
        }

        [Fact]
        public void ObjectTo_NullSource_ReturnsDestination()
        {
            var destination = new Destination { nome = "invariato" };

            var result = MapObj.ObjectTo<object>(null, destination);

            Assert.Same(destination, result);
        }

        [Fact]
        public void ObjectTo_ConvertsValueWhenTypesDiffer()
        {
            var result = MapObj.ObjectTo(new TypedSource { Numero = "123" }, new TypedDestination());

            Assert.Equal(123, result.Numero);
        }

        [Fact]
        public void ObjectTo_UnconvertibleValue_IsReportedAndDoesNotThrow()
        {
            var reporter = new RecordingErrorReporter();
            ErrorReporter.Current = reporter;

            try
            {
                var result = MapObj.ObjectTo(new TypedSource { Numero = "not-a-number" }, new TypedDestination());

                Assert.Equal(0, result.Numero);
                Assert.True(reporter.Count >= 1);
            }
            finally
            {
                ErrorReporter.Reset();
            }
        }

        [Fact]
        public void ObjectTo_DefaultDestination_IsNewInstance()
        {
            var result = MapObj.ObjectTo<Destination>(new Destination());

            Assert.NotNull(result);
        }
    }
}
