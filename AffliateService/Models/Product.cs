using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AffliateService.Models
{
    public class ProductDetailsBasic
    {
        public int Id { get; set; }
        public string MedicineName { get; set; }
        public string ImagePath { get; set; }
    }

    public class ProductDiscountPayload
    {
        //public int Id { get; set; }
        public int ProductId { get; set; }
        public decimal DiscountPercent { get; set; }
    }
}
