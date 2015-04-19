using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZScanLib;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZScanLibTests
{
    [TestClass]
    public class ScannerTests
    {
        /// <summary>
        /// Tests the functionality of the pattern matcher. 
        /// </summary>
        [TestMethod]
        public void TestExactPattern()
        {
            // Create the matcher
            PatternMatcher matcher = new PatternMatcher();

            // Create a new predictable pattern. 
            var pattern = string.Join("", Enumerable.Repeat("0", 10000));           

            // Create the signature object. 
            var signature = new Signature(pattern, 0);

            // Create the memory buffer to scan a pattern for. 
            byte[] bytes = Enumerable.Repeat<byte>(0x00, 5000).ToArray();

            // Get the starting address for the matched signature. 
            var index = matcher.FindMatch(signature, bytes);

            // Test to see if the signature was found. 
            Assert.IsTrue(index == 0);
        }

        /// <summary>
        /// Tests if the default matcher can determine the address of 
        /// a signature in the middle of memory. 
        /// </summary>
        [TestMethod]
        public void TestPatternInMiddle()
        {
            // Create a pattern like: 
            // 00000111111111100000

            // The memory in which to search for the pattern. 
            var buffer = new byte[] { 0, 0, 0, 0, 17, 17, 0, 0, 0, 0 };

            // Create the new signature with the pattern data. 
            Signature signature = new Signature("1111", 0);

            // Create and find the pattern. 
            PatternMatcher matcher = new PatternMatcher();
            var index = matcher.FindMatch(signature, buffer);

            // Index should be at position four. 
            Assert.IsTrue(index == 4);
        }
    }
}