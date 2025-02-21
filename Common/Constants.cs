namespace AshishGeneralStore.Common
{
    public class Constants
    {
        public static class Elasticsearch
        {
            public const string ProductsIndex = "products";
            public const string OrdersIndex = "orders";
            public const string CustomersIndex = "customers";
        }

        // You can add other constants here, e.g., for SignalR hubs, API routes, etc.
        public static class ApiRoutes
        {
            public const string AdminInventoryBase = "api/admin/inventory";
            public const string CustomerBase = "api/customer";
            public const string AuthBase = "api/auth";
            public const string AuthLogin = $"{AuthBase}/login";
            public const string AuthRegister = $"{AuthBase}/register";
            public const string InventoryProducts = $"{AdminInventoryBase}/products";
            public const string InventoryLowStock = $"{AdminInventoryBase}/low-stock";
        }

        public static class SignalR
        {
            public const string InventoryHub = "/hubs/inventory";
        }

        public static class ML
        {
            public const string RecommendationModelPath = "MLModels/Recommendations.zip";
        }

        public static class Jwt
        {
            public const string Key = "d36b1dd8-2cc9-4e47-9721-329d42a24378"; 
            public const string Issuer = "AshishGeneralStore";
            public const string Audience = "AshishGeneralStore";
        }
    }
}
