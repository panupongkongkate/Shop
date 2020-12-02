using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shop.Models
{
    public class ProductType
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
    }
}
