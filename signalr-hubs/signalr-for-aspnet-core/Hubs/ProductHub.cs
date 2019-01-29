using Microsoft.AspNetCore.SignalR;
using signalr_for_aspnet_core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;

namespace signalr_for_aspnet_core.Hubs
{
    public class ProductHub : Hub
    {
        public override System.Threading.Tasks.Task OnConnectedAsync()
        {
            Groups.AddToGroupAsync(Context.ConnectionId, GetGroupName());
            return base.OnConnectedAsync();
        }

        public override System.Threading.Tasks.Task OnDisconnectedAsync(Exception e)
        {
            Groups.RemoveFromGroupAsync(Context.ConnectionId, GetGroupName());
            return base.OnDisconnectedAsync(e);
        }

        public IEnumerable<ProductSignalR> Read()
        {
            using (var context = new SampleEntitiesDataContext())
            {

                var products = context.Products.Select(p => new ProductSignalR
                    {
                        ID = p.ProductID,
                        ProductName = p.ProductName,
                        UnitPrice = (double)p.UnitPrice.GetValueOrDefault(),
                        UnitsInStock = p.UnitsInStock.GetValueOrDefault(),
                        CreatedAt = DateTime.Now.AddMilliseconds(1),
                        Category = new CategorySignalR()
                        {
                            CategoryID = p.Category.CategoryID,
                            CategoryName = p.Category.CategoryName
                        }
                    })
                    .ToList();

                return products;
            }
        }

        public ProductSignalR Create(ProductSignalR product)
        {
            product.ID = DateTime.Now.Ticks;
            product.CreatedAt = DateTime.Now;
            product.Category = product.Category ?? new CategorySignalR() { CategoryID = 1 };

            Clients.OthersInGroup(GetGroupName()).SendAsync("create", product);

            return product;
        }

        public void Update(ProductSignalR product)
        {
            Clients.OthersInGroup(GetGroupName()).SendAsync("update", product);
        }

        public void Destroy(ProductSignalR product)
        {
            Clients.OthersInGroup(GetGroupName()).SendAsync("destroy", product);
        }

        public string GetGroupName()
        {
            return GetRemoteIpAddress();
        }

        public string GetRemoteIpAddress()
        {
            return Context.GetHttpContext()?.Connection.RemoteIpAddress.ToString();
        }
    }
}
