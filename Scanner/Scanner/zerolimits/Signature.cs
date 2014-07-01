
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scanner
{
    public class Signature
    {
        public String Pattern = String.Empty;
        public String Mask = String.Empty;
        public int Offset = 0;

        public Signature(String pattern, String mask, int offset)
        {
            this.Pattern = pattern;
            this.Mask = mask;
            this.Offset = offset;
        }

        public static Signature Create(String pattern, String mask, int offset)
        {
            return new Signature(pattern, mask, offset);
        }
    }
}
