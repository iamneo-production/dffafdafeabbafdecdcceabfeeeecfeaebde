using System;
using System.Data;
using System.Data.SqlClient;
using DisconnectedArchitecture.Models;

namespace DisconnectArchitecture
{

    public static class ConnectionStringProvider
    {
        // Replace with your actual connection string
        public static string ConnectionString { get; } = "Replace your connectionstring here";
    }

    public class Program
    {
        // Replace with your actual connection string
        static string connectionString = ConnectionStringProvider.ConnectionString;

        static void Main(string[] args) 
        {

            // Write your Disconnected Architecture console app here
            // 1. Add Product
            // 2. List Products
            // 3. Update Stock Quantity
            // 4. Delete Product
            // 5. Exit
        }
        // Change your method names (xyz) to appropriate name
        public static void xyz(Product product)
        {
            // Write your code to add new product
        }

        public static void xyz()
        {
            // Write your code to ListProducts
        }

        public static void xyz(string productName, int newStockQuantity)
        {
            // Write your code to update the StockQuantity by using ProductName
        }

        public static void xyz(int productID)
        {
            // Write your code to delete the Product using ProductID
        }

    }
}
