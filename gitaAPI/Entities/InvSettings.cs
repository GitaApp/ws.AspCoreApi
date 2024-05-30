using System;
namespace gitaAPI.Entities

{
    public class InvSettings
    {
        public int id { get; set; }
        public int companyId { get; set; }
        public string szamlaagentkulcs { get; set; }
        public bool eszamla { get; set; }
        public bool szamlaLetoltes { get; set; }
        public int szamlaLetoltesPld { get; set; }
        public int valaszVerzio { get; set; }
        public string aggregator { get; set; }
        public string szamlaKulsoAzon { get; set; }
    }
}

