using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Humans.Web.Models
{
    public class Result
    {
        public int Id { get; set; }
        public bool Success { get; set; }
        public string MessageForLog { get; set; }
        public string MessageForUser { get; set; }
        public dynamic DynObject { get; set; }

        public Result()
        {
            Success = false;
            Id = 0;
            MessageForLog = string.Empty;
            MessageForUser = string.Empty;
            DynObject = null;
        }
    }
}