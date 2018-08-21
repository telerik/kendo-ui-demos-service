using GraphQL.Types;

namespace graphql_aspnet_core.Models.GraphQL
{
    public class ProductType : ObjectGraphType<Data.Product>
    {
        public ProductType()
        {
            Field(x => x.ProductID);
            Field(x => x.ProductName);
            Field(x => x.UnitPrice, true);
            Field(x => x.UnitsInStock, true);
        }
    }
}
