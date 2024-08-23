using GraphQL;
using GraphQL.Conversion;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace graphql_aspnet_core.Models.GraphQL.Product
{
    public class ProductsSchema : Schema
    {
        public ProductsSchema(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            Query = serviceProvider.GetRequiredService<ProductQuery>();
            Mutation = serviceProvider.GetRequiredService<ProductMutation>();
        }
    }
}
