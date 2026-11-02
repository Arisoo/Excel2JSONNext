using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Excel2JSONCoreLib.Models
{
    public partial class LogModel
    {
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }

        public LogModel(string message)
        {
            Message = message;
            Timestamp = DateTime.Now;
        }
    }
}
