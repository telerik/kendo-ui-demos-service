﻿using KendoCRUDService.Data.Models;
using KendoCRUDService.Extensions;
using Microsoft.EntityFrameworkCore;

namespace KendoCRUDService.Data.Repositories
{
    public class OrderRepository
    {
        private readonly ISession _session;
        private readonly IDbContextFactory<DemoDbContext> _contextFactory;

        public OrderRepository(IHttpContextAccessor httpContextAccessor, IDbContextFactory<DemoDbContext> contextFactory)
        {
            _session = httpContextAccessor.HttpContext.Session;
            _contextFactory = contextFactory;
        }

        public IList<Order> All()
        {
            IList<Order> result = _session.GetObjectFromJson<IList<Order>>("Orders");

            if (result == null)
            {
                result = _contextFactory.CreateDbContext().Orders.Select(c => new Order
                {
                    OrderID = c.OrderID,
                    ShipName = c.ShipName,
                    ShipCity = c.ShipCity,
                    ShipCountry = c.ShipCountry,
                    ShipAddress = c.ShipAddress,
                    ShipPostalCode = c.ShipPostalCode,
                    ShipRegion = c.ShipRegion,
                    ShipVia = c.ShipVia,
                    CustomerID = c.CustomerID,
                    EmployeeID = c.EmployeeID,
                    Freight = c.Freight,
                    OrderDate = c.OrderDate,
                    RequiredDate = c.RequiredDate,
                    ShippedDate = c.ShippedDate
                }).ToList();

                _session.SetObjectAsJson("Orders", result);
            }

            return result;
        }
    }
}
