using System;
using System.Collections.Generic;
using System.Text;

namespace AzureResourceCommon.Dtos
{
    public class Resources
    {
        public int Id { get; set; }
        public string ResourcesJson { get; set; }
        public string Differences { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
