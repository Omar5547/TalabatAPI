using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.Core.Entities
{
    public class BasketItem
    {
        [Required]
        public int Id { get; set; }
        [Required]

        public string productName { get; set; }
        [Required]

        public string PictureUrl { get; set; }
        [Required]

        public string Brand {  get; set; }
        [Required]

        public string Type { get; set; }
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage ="Price can not be Zero")]
        public decimal Price { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage ="Quantity Must Be one item at least")]
        public int Quantity { get; set; }


    }
}
