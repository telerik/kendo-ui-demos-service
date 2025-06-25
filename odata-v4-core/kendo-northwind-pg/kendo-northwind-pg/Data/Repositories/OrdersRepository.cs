﻿using kendo_northwind_pg.Data.Models;
using kendo_northwind_pg.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Net;

namespace kendo_northwind_pg.Data.Repositories
{
    public class OrdersRepository
    {
        private readonly ISession _session;
        private readonly IServiceScopeFactory _scopeFactory;
        private IHttpContextAccessor _contextAccessor;
        private ConcurrentDictionary<string, IList<Order>> _orders;


        public OrdersRepository(IHttpContextAccessor httpContextAccessor, IServiceScopeFactory scopeFactory)
        {
            _session = httpContextAccessor.HttpContext.Session;
            _contextAccessor = httpContextAccessor;
            _scopeFactory = scopeFactory;
            _orders = new ConcurrentDictionary<string, IList<Order>>();
        }

        public IList<Order> All()
        {
            var userKey = SessionUtils.GetUserKey(_contextAccessor);

            return _orders.GetOrAdd(userKey, key =>
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<DemoDbContext>();
                    return context.Orders
                    .Include(p => p.OrderDetails).Select(p => new Order
                    {
                        OrderID = p.OrderID,
                        CustomerID = p.CustomerID,
                        EmployeeID = p.EmployeeID.GetValueOrDefault(),
                        Employee = p.Employee,
                        Freight = p.Freight.GetValueOrDefault(),
                        OrderDate = p.OrderDate,
                        RequiredDate = p.RequiredDate,
                        ShipAddress = p.ShipAddress,
                        ShipCity = p.ShipCity,
                        ShipCountry = p.ShipCountry,
                        ShipName = p.ShipName,
                        ShippedDate = p.ShippedDate,
                        ShipPostalCode = p.ShipPostalCode,
                        ShipRegion = p.ShipRegion,
                        ShipVia = p.ShipVia.GetValueOrDefault(),
                        ShipViaNavigation = p.ShipViaNavigation,
                        OrderDetails = p.OrderDetails
                    }).ToList();
                }

            });
        }

        public Order One(Func<Order, bool> predicate)
        {
            return All().FirstOrDefault(predicate);
        }

        public IEnumerable<Order> Where(Func<Order, bool> predicate)
        {
            return All().Where(predicate);
        }

        public void Insert(Order Order)
        {
            var first = All().OrderByDescending(p => p.OrderID).FirstOrDefault();
            if (first != null)
            {
                Order.OrderID = first.OrderID + 1;
            }
            else
            {
                Order.OrderID = 0;
            }

            All().Insert(0, Order);
        }

        public void Insert(IEnumerable<Order> Orders)
        {
            foreach (var Order in Orders)
            {
                Insert(Order);
            }
        }

        public void Update(Order Order)
        {
            var target = One(p => p.OrderID == Order.OrderID);
            if (target != null)
            {
                target.Freight = Order.Freight;
                target.OrderDate = Order.OrderDate;
                target.RequiredDate = Order.RequiredDate;
                target.ShipAddress = Order.ShipAddress;
                target.ShipCity = Order.ShipCity;
                target.ShipCountry = Order.ShipCountry;
                target.ShipName = Order.ShipName;
                target.ShippedDate = Order.ShippedDate;
                target.ShipPostalCode = Order.ShipPostalCode;
                target.ShipRegion = Order.ShipRegion;
                target.ShipVia = Order.ShipVia;
            }
        }

        public void Update(IEnumerable<Order> Orders)
        {
            foreach (var Order in Orders)
            {
                Update(Order);
            }
        }

        public void Delete(Order Order)
        {
            var target = One(p => p.OrderID == Order.OrderID);
            if (target != null)
            {
                All().Remove(target);
            }
        }

        public void Delete(IEnumerable<Order> Orders)
        {
            foreach (var Order in Orders)
            {
                Delete(Order);
            }
        }
    }
}
