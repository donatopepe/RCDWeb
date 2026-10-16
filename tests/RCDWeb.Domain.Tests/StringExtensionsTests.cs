using RCDWeb.Domain.Utility;
using Xunit;

namespace RCDWeb.Domain.Tests
{
    public class StringExtensionsTests
    {
        [Theory]
        [InlineData("AB 123", "ab123", true)]
        [InlineData("ab123", "AB 123", true)]
        [InlineData("ab123", "ab124", false)]
        [InlineData("", "", true)]
        public void EqualsNormalized_IgnoresSpacesAndCase(string a, string b, bool expected)
        {
            Assert.Equal(expected, a.EqualsNormalized(b));
        }

        [Fact]
        public void EqualsNormalized_NullAndEmpty_AreNotEqual()
        {
            Assert.False(((string)null).EqualsNormalized(""));
            Assert.False("".EqualsNormalized(null));
        }

        [Fact]
        public void EqualsNormalized_BothNull_IsTrue()
        {
            string a = null;
            string b = null;
            Assert.True(a.EqualsNormalized(b));
        }

        [Theory]
        [InlineData("Via Roma 1", "via roma", true)]
        [InlineData("VIA ROMA 1", "roma1", true)]
        [InlineData("Via Roma", "Milano", false)]
        public void ContainsNormalized_IgnoresSpacesAndCase(string a, string b, bool expected)
        {
            Assert.Equal(expected, a.ContainsNormalized(b));
        }

        [Fact]
        public void ContainsNormalized_NullSource_ReturnsFalse()
        {
            Assert.False(((string)null).ContainsNormalized("x"));
        }

        [Fact]
        public void ContainsNormalized_NullNeedle_ReturnsTrue()
        {
            // Comportamento storico di String.Contains("") => true, reso null-safe.
            Assert.True("qualsiasi".ContainsNormalized(null));
        }
    }
}
