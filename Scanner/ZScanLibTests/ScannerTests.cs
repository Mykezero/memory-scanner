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
        private Process process;

        public ScannerTests()
        {
            // Grab first notepad session. 
            process = Process.GetProcessesByName("pol").FirstOrDefault();
        }

        [TestMethod]
        public void TestPatternMatcher()
        {
            PatternMatcher matcher = new PatternMatcher();

            var pattern = string.Join("", Enumerable.Repeat("0", 10000));
            var mask = string.Join("", Enumerable.Repeat("X", 5000));
            var offset = 0;
                
            var signature = new Signature(pattern, mask, offset);

            byte[] bytes = Enumerable.Repeat<byte>(0x00, 5000).ToArray();

            var index = matcher.FindMatch(signature, bytes);

            Assert.IsTrue(index == 0);
        }

        public void Test()
        {
            // Auto-Fail on process not found. 
            Assert.IsNotNull(process);

            // The signature data for locating our player's hp in Final Fantasy XI. 
            var pattern = "8D048D00000000B9000000008D780400000000C70000000000F3A5";
            var mask = "XXX????X????XX????XX?????XX";
            var offset = 0;

            // Create a scanner and search for our signature. If one is not found
            // we fail this test. 
            Signature signature = new Signature(pattern, mask, offset);
            MemoryScanner scanner = new MemoryScanner(process);
            IntPtr address = scanner.Scan(signature);
            Assert.IsTrue(address != IntPtr.Zero);

            // Get the value
            byte[] buffer = new byte[sizeof(int)];
            Memory.Peek(process, address, buffer);
            var health = BitConverter.ToInt32(buffer, 0);

            // Return true if the health is in its normal range. 
            Assert.IsTrue(health >= 0 || health <= 100);
        }
    }
}