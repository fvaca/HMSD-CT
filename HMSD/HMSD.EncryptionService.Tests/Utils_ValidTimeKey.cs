using System;
using System.Runtime.CompilerServices;
using HMSD.EncryptionService.Utils;
using Xunit;


namespace HMSD.EncryptionService.Tests
{
    public class Utils_ValidTimeKey
    {
       

        [Fact]
        public void IsValidTimeKey_LastThreeDigitsFibbonaci()
        {
            var mykey = KeyUtils.GetTimeKey();

            var digits = mykey % 100;

            Assert.True(KeyUtils.IsFibonacci(digits));
        }
    }
}
