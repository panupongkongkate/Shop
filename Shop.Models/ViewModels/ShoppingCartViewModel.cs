using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Models.ViewModels
{
    public class ShoppingCartViewModel
    {
        public List<Product> Product { get; set; }
        public Appointments Appointments { get; set; }
    }
}
