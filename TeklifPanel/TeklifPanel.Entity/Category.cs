﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeklifPanel.Entity
{
    public class Category : BaseEntity
    {
        public Category()
        {
            Products = new List<Product>();
        }
        public string Name { get; set; }
        public string Details { get; set; }
        public string Url { get; set; }
        public List<Product> Products { get; set; }
        public decimal KDV { get; set; }
    }
}
