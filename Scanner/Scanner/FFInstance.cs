using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics; // For Process

namespace Vivisection
{
    public class FFInstance
    {
        public Process MyProcess { get; private set; }
        public int Instance { get; private set; }

        #region Constructor
        public FFInstance(Process proc)
        {
            MyProcess = proc;
            Instance = FFACE.CreateInstance((uint)MyProcess.Id);
        }
        #endregion

        #region ToString
        public override string ToString()
        {
            if (!Valid) { return "DELETED"; }
            else { return MyProcess.MainWindowTitle; }
        }
        #endregion

        #region Valid Check
        public bool Valid
        {
            get
            {
                return (MyProcess == null ? false : !MyProcess.HasExited);
            }
        }
        #endregion

        #region Equals Override
        public override bool Equals(object obj)
        {
            if (obj is FFInstance)
            {
                return MyProcess.Id.Equals(((FFInstance)obj).MyProcess.Id);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
    }
}
