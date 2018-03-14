using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OnlineTimeClockMTSImportFileCreator
{
    public class LogEventArgs : EventArgs
    {
        private string sMessage = string.Empty;
        private bool bNoNewLine = false;

        public LogEventArgs(string pMessage, bool pNoNewLine)
        {
            this.Message = pMessage;
            this.NoNewLine = pNoNewLine;
        }
        public LogEventArgs(string pMessage)
        {
            this.Message = pMessage;

        }
        public string Message
        {
            get { return sMessage; }
            set { this.sMessage = value; }
        }

        public bool NoNewLine
        {
            get { return bNoNewLine; }
            set { this.bNoNewLine = value; }
        }
    }
}
