using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.Exeptions
{
    public class NotAcceptableStatusException : Exception
    {
        public NotAcceptableStatusException(string message) : base(message)
        {

        }
    }
}
