using System;
using RCDWeb.Domain.Abstractions;
using RCDWeb.Domain.Utility;
using Xunit;

namespace RCDWeb.Domain.Tests
{
    [Collection("ErrorReporter")]
    public class DateUtilTests : IDisposable
    {
        private readonly RecordingErrorReporter _reporter = new RecordingErrorReporter();

        public DateUtilTests()
        {
            ErrorReporter.Current = _reporter;
        }

        public void Dispose()
        {
            ErrorReporter.Reset();
        }

        [Fact]
        public void DataToText_Null_ReturnsEmpty()
        {
            Assert.Equal(String.Empty, DateUtil.DataToText(null));
        }

        [Fact]
        public void DataToText_ValidDate_UsesInvariantFormat()
        {
            var value = new DateTime(2026, 10, 16, 13, 45, 0, DateTimeKind.Unspecified);
            Assert.Equal("16/10/2026", DateUtil.DataToText(value));
        }

        [Fact]
        public void DataToText_CustomFormat_IsRespected()
        {
            var value = new DateTime(2026, 1, 2);
            Assert.Equal("2026-01-02", DateUtil.DataToText(value, true, "yyyy-MM-dd"));
        }

        [Fact]
        public void DataToText_TooOldDate_ReturnsEmptyAndReports()
        {
            var tooOld = DateTime.Now.AddYears(-(DateUtil.MaxYearsBack + 1));

            Assert.Equal(String.Empty, DateUtil.DataToText(tooOld));
            Assert.Equal(1, _reporter.Count);
        }

        [Fact]
        public void DataToText_TooOldDate_WithViewErrorFalse_DoesNotReport()
        {
            var tooOld = DateTime.Now.AddYears(-(DateUtil.MaxYearsBack + 1));

            Assert.Equal(String.Empty, DateUtil.DataToText(tooOld, viewerror: false));
            Assert.Equal(0, _reporter.Count);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void TextToDate_EmptyInput_ReturnsNullWithoutReporting(string input)
        {
            Assert.Null(DateUtil.TextToDate(input));
            Assert.Equal(0, _reporter.Count);
        }

        [Theory]
        [InlineData("16/10/2026", 2026, 10, 16)]
        [InlineData("16102026", 2026, 10, 16)]
        [InlineData("161026", 2026, 10, 16)]
        [InlineData("16/10/26", 2026, 10, 16)]
        public void TextToDate_AcceptedFormats_AreParsed(string input, int y, int m, int d)
        {
            var result = DateUtil.TextToDate(input);

            Assert.NotNull(result);
            Assert.Equal(new DateTime(y, m, d), result.Value.Date);
            Assert.Equal(0, _reporter.Count);
        }

        [Fact]
        public void TextToDate_InvalidFormat_ReturnsNullAndReports()
        {
            Assert.Null(DateUtil.TextToDate("non-una-data"));
            Assert.Equal(1, _reporter.Count);
        }

        [Fact]
        public void TextToDate_InvalidFormat_WithViewErrorFalse_DoesNotReport()
        {
            Assert.Null(DateUtil.TextToDate("non-una-data", viewerror: false));
            Assert.Equal(0, _reporter.Count);
        }

        [Fact]
        public void TextToDate_TooOldDate_ReturnsNullAndReports()
        {
            var tooOld = DateTime.Now.AddYears(-(DateUtil.MaxYearsBack + 2)).ToString("dd/MM/yyyy");

            Assert.Null(DateUtil.TextToDate(tooOld));
            Assert.Equal(1, _reporter.Count);
        }

        [Fact]
        public void RoundTrip_DataToText_TextToDate()
        {
            var original = new DateTime(2020, 3, 9);

            var text = DateUtil.DataToText(original);
            var parsed = DateUtil.TextToDate(text);

            Assert.Equal(original, parsed);
        }

        [Fact]
        public void AcceptedFormats_AreStable()
        {
            Assert.Equal(6, DateUtil.AcceptedFormats.Count);
            Assert.Contains("dd/MM/yyyy", DateUtil.AcceptedFormats);
        }
    }
}
