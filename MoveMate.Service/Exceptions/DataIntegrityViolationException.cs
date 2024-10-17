using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.Exeptions
{
    public class DataIntegrityViolationException : Exception
    {
        public DataIntegrityViolationException(string message) : base(message)
        {
        }
    }
}