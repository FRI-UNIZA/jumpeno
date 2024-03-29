﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JumpenoWebassembly.Shared.ErrorHandling
{
    public class Error
    {
        public long Id { get; set; }
        public string Type { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public DateTime Date { get; set; }
        public string Notes { get; set; }
    }
}
