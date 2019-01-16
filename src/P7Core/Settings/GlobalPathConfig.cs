using System.Collections.Generic;

namespace P7Core.Settings
{
    public class GlobalPathConfig
    {
        public List<GlobalPathRecord> OptOut { get; set; }
        public List<GlobalPathRecord> OptIn { get; set; }
    }
}