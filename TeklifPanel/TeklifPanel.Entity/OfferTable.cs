using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeklifPanel.Entity
{
    public class OfferTable : BaseEntity
    {
        public string Name { get; set; }
        public string MenuName { get; set; }
        public bool IsShow { get; set; }
        public int SiraNo { get; set; }

    }
}
