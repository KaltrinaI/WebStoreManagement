﻿namespace WebStoreApp.DTOs
{
    public class OrderResponseDTO
    {
        public string OrderStatus { get; set; }
        public DateTime OrderDate { get; set; }
        public string UserEmail { get; set; }
        public  ICollection<OrderItemsResponseDTO> OrderItems { get; set; }
    }
}
