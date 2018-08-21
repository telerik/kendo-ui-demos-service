using GraphQL;
using GraphQL.Conversion;
using GraphQL.Types;

namespace graphql_aspnet_core.Models.GraphQL.Product
{
    public class ProductsSchema : Schema
    {
        public ProductsSchema(IDependencyResolver resolver): base(resolver)
        {
            Query = resolver.Resolve<ProductQuery>();
            Mutation = resolver.Resolve<ProductMutation>();
        }
    }
}
