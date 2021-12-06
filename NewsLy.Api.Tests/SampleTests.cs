using Xunit;

namespace NewsLy.Api.Tests 
{
    public class SampleTests
    {
        [Theory]
        [InlineData(1, 1, 2)]
        [InlineData(100, 255, 355)]
        [InlineData(27, 3, 30)]
        public void SimpleAdditionComparisonTest(int first, int second, int expected)
        {
            var result = first + second;

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(2, 1, 1)]
        [InlineData(900, 400, 500)]
        [InlineData(100, 210, -110)]
        public void SimpleSubtractionComparisonTest(int first, int second, int expected)
        {
            var result = first - second;

            Assert.Equal(expected, result);
        }
    }
}