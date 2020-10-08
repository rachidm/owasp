using System;
using Xunit;

namespace UnitTestProject1
{
    public class UnitTest1
    {
        [Theory]
        [InlineData(1)]
        public void Test1(int value)
        {
            Assert.Equal(1, value);
        }
    }
}
