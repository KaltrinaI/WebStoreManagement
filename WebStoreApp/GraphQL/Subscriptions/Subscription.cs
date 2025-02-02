using HotChocolate;
using HotChocolate.Types;
using HotChocolate.Subscriptions;
using WebStoreApp.DTOs;

namespace WebStoreApp.GraphQL.Subscriptions
{
    public class Subscription
    {
        [Subscribe]
        [Topic("ProductStockUpdated")]
        public ProductDTO OnProductStockUpdated([EventMessage] ProductDTO product)
        {
            return product;
        }
    }
}
