using WebStoreApp.DTOs;
using WebStoreApp.GraphQL.Resolvers;

namespace WebStoreApp.GraphQL.Types
{
    public class ProductType : ObjectType<ProductDTO>
    {
        protected override void Configure(IObjectTypeDescriptor<ProductDTO> descriptor)
        {
            descriptor.Field(t => t.Id)
                        .Type<NonNullType<IdType>>();

            descriptor.Field(t => t.Name)
                      .Type<NonNullType<StringType>>();

            descriptor.Field(t => t.Quantity)
                      .Name("stockLevel")
                      .Description("Current stock available for this product.").ResolveWith<ProductResolver>(r => r.GetStockLevel(default!));

        

           
        }
    }

}
