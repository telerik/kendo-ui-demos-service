using GraphQL.Types;
using graphql_aspnet_core.Data.Contracts;

namespace graphql_aspnet_core.Models.GraphQL
{
    public class ProductMutation : ObjectGraphType
    {
        public ProductMutation(IProductRepository productRepository)
        {
            Name = "CreateProductMutation";
            Field<ProductType>(
                "createProduct",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<ProductInputType>> { Name = "product" }
                ),
                resolve: context =>
                {
                    var product = context.GetArgument<Data.Product>("product");
                    var totalProducts = productRepository.GetTotalRecords().Result;
                    product.ProductID = ++totalProducts;

                    return productRepository.AddAsync(product);
                });

            Name = "UpdateProductMutation";
            Field<ProductType>(
                "updateProduct",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<ProductInputType>> { Name = "product" }
                ),
                resolve: context =>
                {
                    var product = context.GetArgument<Data.Product>("product");

                    return productRepository.UpdateAsync(product);
                });

            Name = "DeleteProductMutation";
            Field<ProductType>(
                "deleteProduct",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<ProductInputType>> { Name = "product" }
                ),
                resolve: context =>
                {
                    var product = context.GetArgument<Data.Product>("product");

                    return productRepository.Delete(product);
                });
        }
    }
}
