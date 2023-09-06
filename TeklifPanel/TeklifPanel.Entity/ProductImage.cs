﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeklifPanel.Entity
{
    public class ProductImage
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public Product Product { get; set; }
        public int ProductId { get; set; }
    }
}
