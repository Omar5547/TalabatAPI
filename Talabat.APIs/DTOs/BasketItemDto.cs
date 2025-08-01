﻿namespace Talabat.APIs.DTOs
{
    public class BasketItemDto
    {
        public int Id { get; set; }
        public string productName { get; set; }
        public string PictureUrl { get; set; }
        public string Brand { get; set; }
        public string Type { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}