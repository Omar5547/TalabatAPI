﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Repositories
{
    public interface IBasketRepository
    {
        Task<CustomerBasket?> GetBasketAsync(string BasketId);
        Task<CustomerBasket> UpdateBasketAsync(CustomerBasket customerBasket);
        Task <bool> DeleteBasketAsync(string BasketId);
    }
}
