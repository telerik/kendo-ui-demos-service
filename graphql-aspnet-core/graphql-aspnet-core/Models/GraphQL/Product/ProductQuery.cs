using GraphQL.Types;
using graphql_aspnet_core.Data.Contracts;

namespace graphql_aspnet_core.Models.GraphQL
{
    public class ProductQuery : ObjectGraphType
    {
        public ProductQuery(IProductRepository productRepository)
        {
            Field<ProductType>(
                "product",
                arguments: new QueryArguments(new QueryArgument<IntGraphType> { Name = "ProductID" }),
                resolve: context => productRepository.GetAsync(context.GetArgument<int>("productID")));

            Field<ListGraphType<ProductType>>(
                "products",
                resolve: context => productRepository.AllAsync());
        }
    }
}
