using NUnit.Framework;
using System;
using System.IO;
using System.Data.SqlClient;
using DisconnectedArchitecture;
//using MusicalInstrumentsCRUD.Services;
using DisconnectedArchitecture.Models;
using System.Reflection;
using DisconnectArchitecture;
using System.Diagnostics.Metrics;

namespace DisconnectedArchitecture.Tests
{
    [TestFixture]
    public class ProgramTests
    {
        private Type programType = typeof(Program);

        private Assembly assembly;
        private Assembly assembly1;
        //private InstrumentService instrumentService;

        private string connectionString;
        private string databaseName;
        private string tableName;
        private Type? _instrumentType;
        private int lastInsertedProductId; // Store the ID of the last inserted product

        [OneTimeSetUp]
        public void LoadAssembly()
        {
            string assemblyPath = "DisconnectedArchitecture.dll"; // Adjust the path if needed
            assembly = Assembly.LoadFrom(assemblyPath);
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            // Load the assembly containing the Program class (DisconnectArchitecture assembly).
            assembly1 = Assembly.LoadFrom("DisconnectedArchitecture.dll");
        }

        [OneTimeSetUp]
        public void SetUp()
        {
            // Set up the connection string and other variables
            connectionString = ConnectionStringProvider.ConnectionString;
            databaseName = "ProductDB";
            tableName = "Products";


            // Create the database
            // CreateDatabase();

            // Create the Products table
            // CreateTable();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            // Delete the product added during the test
            //DeleteProduct(lastInsertedProductId);
            //DeleteTestData();
            DeleteProduct1(lastInsertedProductId);


            // Drop the Products table
            // DropTable();

            // Drop the database
            // DropDatabase();
        }

        [Test]
        public void ConnectToDatabase_ConnectionSuccessful()
        {
            bool connectionSuccess = false;
            string errorMessage = "";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    connectionSuccess = true;
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            Assert.IsTrue(connectionSuccess, "Failed to connect to the database. Error message: " + errorMessage);
        }

        [Test]
        public void Test_ProductClassExists()
        {
            string className = "DisconnectedArchitecture.Models.Product";
            Type movieType = assembly.GetType(className);
            Assert.NotNull(movieType, $"The class '{className}' does not exist in the assembly.");
        }

        [Test]
        public void Test_ProductIDPropertyDataType_Int()
        {
            Type movieType = assembly.GetType("DisconnectedArchitecture.Models.Product");
            PropertyInfo property = movieType.GetProperty("ProductID");
            Assert.AreEqual("System.Int32", property.PropertyType.FullName, "The 'ProductId' property should be of type int.");
        }

        [Test]
        public void Test_ProductNamePropertyDataType_string()
        {
            Type movieType = assembly.GetType("DisconnectedArchitecture.Models.Product");
            PropertyInfo property = movieType.GetProperty("ProductName");
            Assert.AreEqual("System.String", property.PropertyType.FullName, "The 'ProductName' property should be of type int.");
        }

        [Test]
        public void Test_StockQuantityPropertyDataType_Int()
        {
            Type movieType = assembly.GetType("DisconnectedArchitecture.Models.Product");
            PropertyInfo property = movieType.GetProperty("StockQuantity");
            Assert.AreEqual("System.Int32", property.PropertyType.FullName, "The 'StockQuantity' property should be of type int.");
        }

        [Test]
        public void Test_PricePropertyDataType_decimal()
        {
            Type movieType = assembly.GetType("DisconnectedArchitecture.Models.Product");
            PropertyInfo property = movieType.GetProperty("Price");
            Assert.AreEqual("System.Decimal", property.PropertyType.FullName, "The 'Price' property should be of type int.");
        }

        [Test]
        public void Test_ProductID_StockQuantity_PropertyGetterSetter()
        {
            Type movieType = assembly.GetType("DisconnectedArchitecture.Models.Product");
            PropertyInfo property = movieType.GetProperty("ProductID");
            PropertyInfo property1 = movieType.GetProperty("StockQuantity");
            Assert.IsTrue(property.CanRead, "The 'ProductID' property should have a getter.");
            Assert.IsTrue(property1.CanRead, "The 'StockQuantity' property should have a getter.");
            Assert.IsTrue(property.CanWrite, "The 'ProductID' property should have a setter.");
            Assert.IsTrue(property1.CanWrite, "The 'StockQuantity' property should have a setter.");
        }

        [Test]
        public void Test_ProductName_PropertyGetterSetter()
        {
            Type movieType = assembly.GetType("DisconnectedArchitecture.Models.Product");
            PropertyInfo property = movieType.GetProperty("ProductName");
            Assert.IsTrue(property.CanRead, "The 'Type' property should have a getter.");
            Assert.IsTrue(property.CanWrite, "The 'Type' property should have a setter.");
        }

        [Test]
        public void Test_PricePropertyGetterSetter()
        {
            Type movieType = assembly.GetType("DisconnectedArchitecture.Models.Product");
            PropertyInfo property = movieType.GetProperty("Price");
            Assert.IsTrue(property.CanRead, "The 'Price' property should have a getter.");
            Assert.IsTrue(property.CanWrite, "The 'Price' property should have a setter.");
        }


        [Test]
        public void Test_AddProduct_ShouldAddProduct_ToDatabase()
        {
            MethodInfo addProductMethod = programType.GetMethod("AddProduct", BindingFlags.Public | BindingFlags.Static);
            if (addProductMethod == null)
            {
                Assert.Fail("AddProduct method not found. Skipping the test.");
            }
            else
            {

                // Arrange
                //InstrumentService instrumentService = new InstrumentService(connectionString);
                Product testInstrument = new Product
                {
                    ProductID = 999, // Replace with a unique ID that doesn't already exist in the database
                    ProductName = "Test Product",
                    Price = 1500,
                    StockQuantity = 50
                };
                lastInsertedProductId = testInstrument.ProductID;
                // Act
                //Program.AddProduct(testInstrument);
                addProductMethod.Invoke(null, new object[] { testInstrument });

                //Product addedInstrument =  (testInstrument.InstrumentId);

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM Products where ProductID = @InstrumentId";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@InstrumentId", testInstrument.ProductID);
                    int inserted = (int)command.ExecuteScalar();
                    Console.WriteLine(inserted);
                    Assert.AreEqual(999, inserted);
                }

                DeleteProduct1(testInstrument.ProductID);
            }
        }

        [Test]
        public void Test_UpdateQuantityStock_By_ProductName_ShouldUpdateProductInDatabase()
        {
            MethodInfo addProductMethod = programType.GetMethod("AddProduct", BindingFlags.Public | BindingFlags.Static);
            MethodInfo updateProductMethod1 = programType.GetMethod("UpdateProductStock", BindingFlags.Public | BindingFlags.Static);
            if (addProductMethod == null && updateProductMethod1 == null)
            {
                Assert.Fail("AddProduct & UpdateProductStock methods not found.");
            }
            else
            {
                // Arrange
                //InstrumentService instrumentService = new InstrumentService(connectionString);
                Product testInstrument = new Product
                {
                    ProductID = 9993, // Replace with a unique ID that doesn't already exist in the database
                    ProductName = "Test Product",
                    Price = 1500,
                    StockQuantity = 50
                };
                lastInsertedProductId = testInstrument.ProductID;


                // Add the instrument to the database initially
                //Program.AddProduct(testInstrument);
                addProductMethod.Invoke(null, new object[] { testInstrument });


                // Modify the testInstrument with updated values
                testInstrument.ProductName = "Test Product";
                testInstrument.StockQuantity = 25;

                // Act
                //Program.UpdateProductStock(testInstrument.ProductName, testInstrument.StockQuantity);
                updateProductMethod1.Invoke(null, new object[] { testInstrument.ProductName, testInstrument.StockQuantity });

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM Products where ProductID = @ProductID";
                    string query1 = "SELECT StockQuantity FROM Products where ProductID = @ProductID";

                    SqlCommand command = new SqlCommand(query, connection);
                    SqlCommand command1 = new SqlCommand(query1, connection);

                    command.Parameters.AddWithValue("@ProductID", testInstrument.ProductID);
                    command1.Parameters.AddWithValue("@ProductID", testInstrument.ProductID);
                    int inserted = (int)command.ExecuteScalar();
                    var nameupdated = command1.ExecuteScalar();
                    Console.WriteLine(inserted);
                    Console.WriteLine(nameupdated);
                    Assert.AreEqual(25, nameupdated);
                }

                DeleteProduct1(testInstrument.ProductID);
            }
        }

        [Test]
        public void Test_DeleteProduct_ShouldDeleteProductFromDatabase()
        {

            MethodInfo addProductMethod = programType.GetMethod("AddProduct", BindingFlags.Public | BindingFlags.Static);
            MethodInfo deleteProductMethod1 = programType.GetMethod("DeleteProduct", BindingFlags.Public | BindingFlags.Static);
            if (addProductMethod == null && deleteProductMethod1 == null)
            {
                Assert.Fail("AddProduct & UpdateProductStock methods not found.");
            }
            else
            {
                // Arrange
                //InstrumentService instrumentService = new InstrumentService(connectionString);
                Product testInstrument = new Product
                {
                    ProductID = 9993, // Replace with a unique ID that doesn't already exist in the database
                    ProductName = "Test Product",
                    Price = 1500,
                    StockQuantity = 50
                };
                lastInsertedProductId = testInstrument.ProductID;

                // Add the instrument to the database initially
                //Program.AddProduct(testInstrument);
                addProductMethod.Invoke(null, new object[] { testInstrument });


                // Act
                //Program.DeleteProduct(testInstrument.ProductID);
                deleteProductMethod1.Invoke(null,new object[] { testInstrument.ProductID });
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM Products where ProductID = @InstrumentId";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@InstrumentId", testInstrument.ProductID);
                    if (command.ExecuteScalar() == null)
                    {
                        Assert.Pass();
                    }
                    else { Assert.Fail(); }
                }
            }
        }

        [Test]
        public void TestAddProductMethod()
        {
            AssertMethodExists("AddProduct");
        }

        [Test]
        public void TestListProductsMethod()
        {
            AssertMethodExists("ListProducts");
        }

        [Test]
        public void TestUpdateProductStockMethod()
        {
            AssertMethodExists("UpdateProductStock");
        }

        [Test]
        public void TestDeleteProductMethod()
        {
            AssertMethodExists("DeleteProduct");
        }

        private void AssertMethodExists(string methodName)
        {
            MethodInfo method = programType.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static);
            Assert.NotNull(method, $"Method '{methodName}' not found.");
        }


        private void DeleteProduct1(int productId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    connection.ChangeDatabase(databaseName);

                    SqlCommand command = connection.CreateCommand();
                    command.CommandText = $"DELETE FROM {tableName} WHERE ProductID = @ID";
                    command.Parameters.AddWithValue("@ID", productId);

                    command.ExecuteNonQuery();
                }
            }
            catch (Exception)
            {
                // Handle any exception if necessary
            }
        }




    }



}
