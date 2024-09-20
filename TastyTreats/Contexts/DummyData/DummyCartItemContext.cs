﻿using TastyTreats.Models;

namespace TastyTreats.Contexts.DummyData
{
    public class DummyCartItemContext
    {
        public static List<CartItem> GetCartItems()
        {
            return new List<CartItem>
            {
                new CartItem
                {
                    CartItemId = 1,
                    Quantity = 2,
                    Price = 1.99m,
                    CartId = 1,
                    ItemId = 1
                },
                new CartItem
                {
                    CartItemId = 2,
                    Quantity = 1,
                    Price = 2.49m,
                    CartId = 2,
                    ItemId = 2
                }
            };
        }
    }
}