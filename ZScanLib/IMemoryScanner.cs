using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZScanLib
{
    /// <summary>
    /// Interface that defines how scanner objects should search
    /// for addresses. 
    /// </summary>
    public interface IMemoryScanner
    {
        IntPtr Scan(Signature signature);
    }
}