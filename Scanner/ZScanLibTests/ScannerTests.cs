using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZScanLib;
using System.Diagnostics;
using System.Linq;

namespace ZScanLibTests
{
    [TestClass]
    public class ScannerTests
    {
        /// <summary>
        /// Tests the functionality of the pattern matcher. 
        /// </summary>
        [TestMethod]
        public void TestPatternMatcher()
        {
            // Create the matcher
            PatternMatcher matcher = new PatternMatcher();

            // Create a new predictable pattern. 
            var pattern = string.Join("", Enumerable.Repeat("0", 10000));

            // Create a mask that matches the new pattern. 
            var mask = string.Join("", Enumerable.Repeat("X", 5000));
            
            var offset = 0;

            // Create the signature object. 
            var signature = new Signature(pattern, mask, offset);

            // Create the memory buffer to scan a pattern for. 
            byte[] bytes = Enumerable.Repeat<byte>(0x00, 5000).ToArray();

            // Get the starting address for the matched signature. 
            var index = matcher.FindMatch(signature, bytes);

            // Test to see if the signature was found. 
            Assert.IsTrue(index == 0);
        }
    }
}