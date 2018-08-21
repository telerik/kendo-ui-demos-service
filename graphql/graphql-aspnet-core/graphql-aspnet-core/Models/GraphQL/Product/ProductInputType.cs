using GraphQL.Types;

namespace graphql_aspnet_core.Models.GraphQL
{
    public class ProductInputType : InputObjectGraphType<Data.Product>
    {
        public ProductInputType()
        {
            Name = "ProductInput";
            Field<NonNullGraphType<IntGraphType>>("ProductID");
            Field<NonNullGraphType<StringGraphType>>("ProductName");
            Field<DecimalGraphType>("UnitPrice");
            Field<IntGraphType>("UnitsInStock");
        }
    }
}
