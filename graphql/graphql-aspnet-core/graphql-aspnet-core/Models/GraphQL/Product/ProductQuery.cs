using GraphQL;
using GraphQL.Types;
using graphql_aspnet_core.Data.Contracts;

namespace graphql_aspnet_core.Models.GraphQL
{
    public class ProductQuery : ObjectGraphType
    {
        public ProductQuery(IProductRepository productRepository)
        {
            Field<ProductType>("product")
                .Argument<NonNullGraphType<IntGraphType>>("ProductID", "<Description goes here>")
                .ResolveAsync(async ctx => await productRepository.GetAsync(ctx.GetArgument<int>("productID")));

            Field<ListGraphType<ProductType>>("products")
                .ResolveAsync(async context => await productRepository.AllAsync());
        }
    }
}
