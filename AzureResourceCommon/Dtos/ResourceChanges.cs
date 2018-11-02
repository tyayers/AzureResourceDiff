using System;
using System.Collections.Generic;
using System.Text;

namespace AzureResourceCommon.Dtos
{
    public class DailyResourceChanges
    {
        public int Id;
        public DateTime Timestamp;
        
        public int Additions = 0;
        public int Others = 0;
        public List<ResourceChanges> Changes = new List<ResourceChanges>();
    }

    public class ResourceChanges
    {
        public string Namespace { get; set; }
        public string Differences { get; set; }
    }
}
