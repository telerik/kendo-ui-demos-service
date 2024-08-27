using GraphQL;
using GraphQL.Types;
using graphql_aspnet_core.Data.Contracts;

namespace graphql_aspnet_core.Models.GraphQL
{
    public class ProductMutation : ObjectGraphType
    {
        public ProductMutation(IProductRepository productRepository)
        {
            Name = "CreateProductMutation";
            Field<ProductType>("createProduct")
                .Argument<NonNullGraphType<ProductInputType>>("product")
                .ResolveAsync(async context =>
                {
                    var product = context.GetArgument<Data.Product>("product");
                    var totalProducts = productRepository.GetTotalRecords().Result;
                    product.ProductID = ++totalProducts;

                    return await productRepository.AddAsync(product);
                });

            Name = "UpdateProductMutation";
            Field<ProductType>("updateProduct")
                .Argument<NonNullGraphType<ProductInputType>>("product")
                .ResolveAsync(async context => 
                {
                    var product = context.GetArgument<Data.Product>("product");

                    return await productRepository.UpdateAsync(product);
                });

            Name = "DeleteProductMutation";
            Field<ProductType>("deleteProduct")
                .Argument<NonNullGraphType<ProductInputType>>("product")
                .ResolveAsync(async context =>
                {
                    var product = context.GetArgument<Data.Product>("product");

                    return await productRepository.Delete(product);
                });
        }
    }
}
