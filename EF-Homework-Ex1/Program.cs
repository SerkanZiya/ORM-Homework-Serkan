using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EF_HomeworkEx1
{
    class Program
    {
        static string _ConnectionSql = "Server=DESKTOP-3M44063\\SQLEXPRESS;Trusted_Connection=true;TrustServerCertificate=true;";

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            DatabaseExistance();
            Table();
            InsertProduct("Laptop");
            InsertProduct("Smartphone");
            InsertBuyer("Ivan");
            InsertBuyer("Georgi");
            BuyProduct(1, 1);
            BuyProduct(1, 2);
            BuyProduct(2, 2);
            Query();
            DeleteMainTable(2);
            //Delete();


        }

        static void DatabaseExistance()
        {
            string checkDataBase = "Select * From sys.databases Where name = 'Warehouse'";
            string createDataBase = "CREATE DATABASE Warehouse";

            using (SqlConnection newdb = new SqlConnection(_ConnectionSql))
            {
                newdb.Open();
                using (SqlCommand Command = new SqlCommand(checkDataBase, newdb))
                {
                    var result = Command.ExecuteScalar();
                    if (result == null)
                    {
                        using (SqlCommand CreateDatabase = new SqlCommand(createDataBase, newdb))
                        {
                            var CreatingResult = CreateDatabase.ExecuteNonQuery();
                            Console.WriteLine("Database Warehouse created.");
                        }
                    }
                }
                ;
                _ConnectionSql = _ConnectionSql + "Database=Warehouse";
                newdb.Close();
            }

        }
        static void Table()
        {
            using (SqlConnection newdb = new SqlConnection(_ConnectionSql))
            {
                newdb.Open();

                string Products = @"
                    IF Not Exists (SELECT * FROM sys.tables WHERE name = 'Products')
                    Create Table Products ( Id INT IDENTITY(1,1) Primary Key,Name Varchar(50) Not Null )";

                string Buyers = @"
                    IF Not Exists (SELECT * FROM sys.tables WHERE name = 'Buyers')
                    Create Table Buyers (Id INT IDENTITY(1,1) Primary Key,Name Varchar(50) Not Null )";

                string Purchases = @"
                 IF Not Exists (SELECT * FROM sys.tables WHERE name = 'Purchases')
                CREATE TABLE Purchases ( Id INT IDENTITY(1,1) Primary Key,
                    BuyerId Int Foreign Key References Buyers(Id),
                    ProductId Int Foreign Key References Products(Id) )";

                using (SqlCommand Addproducts = new SqlCommand(Products, newdb))
                {
                    Addproducts.ExecuteNonQuery();
                    Console.WriteLine("You added table Products");
                }

                using (SqlCommand AddBuyers = new SqlCommand(Buyers, newdb))
                {
                    AddBuyers.ExecuteNonQuery();
                    Console.WriteLine("You added table Buyers");
                }
                using (SqlCommand AddPurchase = new SqlCommand(Purchases, newdb))
                {
                    AddPurchase.ExecuteNonQuery();
                    Console.WriteLine("You added table Purchases");
                }

                newdb.Close();
            }
        }

        public static void InsertProduct(string Product_Name)
        {
            try
            {
                using (SqlConnection newdb = new SqlConnection(_ConnectionSql))
                {
                    newdb.Open();
                    string insert = "INSERT INTO Products (Name) " +
                        "Values(@Name )";

                    using (SqlCommand Inserting = new SqlCommand(insert, newdb))
                    {
                        Inserting.Parameters.AddWithValue("@Name", Product_Name);
                        Inserting.ExecuteNonQuery();

                    }
                    newdb.Close();
                }
            }
            catch (Exception ex)
            {
                 Console.WriteLine($"Грешка:{ex.Message}"); 
            }

        }
        public static void InsertBuyer(string BuyerName)
        {
            try
            {
                using (SqlConnection newdb = new SqlConnection(_ConnectionSql))
                {

                    newdb.Open();
                    string InsertBuyer = "INSERT INTO Buyers(Name) " +
                        "Values(@Name)";
                    using (SqlCommand Inserting = new SqlCommand(InsertBuyer, newdb))
                    {

                        Inserting.Parameters.AddWithValue("@Name", BuyerName);
                        Inserting.ExecuteNonQuery();
                    }
                    newdb.Close();
                }
            }
            catch (Exception ex) { Console.WriteLine($"Грешка:{ex.Message}"); }
        }
        public static void BuyProduct(int buyerId, int productId)
        {
            try
            {
                using (SqlConnection newdb = new SqlConnection(_ConnectionSql))
                {
                    newdb.Open();
                    string insertPurchase = "INSERT INTO Purchases (BuyerId, ProductId) " +
                        "Values (@BuyerId, @ProductId)";

                    using (SqlCommand cmd = new SqlCommand(insertPurchase, newdb))
                    {
                        cmd.Parameters.AddWithValue("@BuyerId", buyerId);
                        cmd.Parameters.AddWithValue("@ProductId", productId);
                        cmd.ExecuteNonQuery();
                    }

                    Console.WriteLine($"Купувач {buyerId} купи продукт номер {productId}.");
                    newdb.Close();
                }
            }
            catch (Exception ex) { Console.WriteLine($"Грешка:{ex.Message}"); }
        }

        public static void Query()
        {
            try
            {
                using (SqlConnection newdb = new SqlConnection(_ConnectionSql))
                {
                    newdb.Open();
                    string query = "Select P.Name AS ProductName, COUNT(B.Id) AS BuyerCount " +
                                   "From Products P " +
                                   "Left Outer Join Purchases B ON P.Id = B.ProductId " +
                                   "Group By P.Name";

                    using (SqlCommand sqlCommand = new SqlCommand(query, newdb))
                    {
                        SqlDataReader reader = sqlCommand.ExecuteReader();

                        while (reader.Read())
                        {
                            Console.WriteLine($"Продукт: {reader["ProductName"]}, Купувачи: {reader["BuyerCount"]}");
                        }

                    }
                    newdb.Close();
                }

            }
            catch (Exception ex)
            {
                 Console.WriteLine($"Грешка:{ex.Message}"); 
            }
        }



        public static void Delete()
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(_ConnectionSql))
                {
                    sqlConnection.Open();

                    string Drop = "DROP DATABASE Warehouse;";

                    using (SqlCommand sqlCommand = new SqlCommand(Drop, sqlConnection))
                    {
                        sqlCommand.ExecuteNonQuery();
                        Console.WriteLine("You deleted database Warehouse");
                    }
                    sqlConnection.Close();
                }
            }
            catch (Exception ex) { Console.WriteLine($"Грешка:{ex.Message}"); }
        }


        public static void DeleteMainTable(int deleteID)
        {
            try
            {

                using (SqlConnection sqlConnection = new SqlConnection(_ConnectionSql))
                {
                    sqlConnection.Open();
                    string deleterecord = "DELETE FROM Purchases WHERE Id = @DeleteId;";


                    using (SqlCommand sqlCommand = new SqlCommand(deleterecord, sqlConnection))
                    {
                        sqlCommand.Parameters.AddWithValue("@DeleteId", deleteID);
                        sqlCommand.ExecuteNonQuery();
                        Console.WriteLine($"You delete row {deleteID} ");
                    }

                    sqlConnection.Close();
                }
            }
            catch (Exception ex) { Console.WriteLine($"Грешка:{ex.Message}"); }
        }
    }
}

