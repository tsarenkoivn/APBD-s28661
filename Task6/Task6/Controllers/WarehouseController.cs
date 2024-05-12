using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Linq.Expressions;
using Task6.Models;

namespace Task6.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WarehouseController : ControllerBase
    {
        private bool ProductExists(SqlConnection mycon, int ProductID) 
        {
            string query = @"SELECT COUNT(*)
                             FROM Product 
                             WHERE IdProduct = @ProductId";
            using (SqlCommand myCommand = new SqlCommand(query, mycon))
            {
                myCommand.Parameters.AddWithValue("@ProductId", ProductID);
                int count = (int)myCommand.ExecuteScalar();
                return count > 0;
            }
        }

        private bool WarehouseExists(SqlConnection mycon, int IdWarehouse) 
        {
            string query = @"SELECT COUNT(*)
                           FROM Warehouse 
                           WHERE IdWarehouse = @WarehouseId";
            using (SqlCommand myCommand = new SqlCommand (query, mycon))
            {
                myCommand.Parameters.AddWithValue("@WarehouseId", IdWarehouse);
                int count = (int)myCommand.ExecuteScalar ();
                return count > 0;
            }
        }

        private bool OrderExists(SqlConnection mycon, int ProductId, int Amount, DateTime CreatedAt)
        {
            string query = @"SELECT COUNT(*) FROM [Order] 
                           WHERE IdProduct = @ProductId AND Amount = @Amount AND CreatedAt < @CreatedAt AND FulfilledAt IS NULL";
            using (SqlCommand myCommand = new SqlCommand(query, mycon))
            {
                myCommand.Parameters.AddWithValue("@ProductId", ProductId);
                myCommand.Parameters.AddWithValue("@Amount", Amount);
                myCommand.Parameters.AddWithValue("@CreatedAt", CreatedAt);
                int count = (int)myCommand.ExecuteScalar();
                return count > 0;
                
            }
        }


        private readonly string connectionString = "Data Source=db-mssql;Initial Catalog=2019SBD;Integrated Security=True";

        [HttpPost]
        public IActionResult AddProductToWarehouse([FromBody] ProductWarehouseRequest request)
        {
            if (request.Amount <= 0)
            {
                return BadRequest("Amount should be greater than zero");
            }

            using(SqlConnection mycon = new SqlConnection (connectionString))
            {
                mycon.Open();
                var transaction  = mycon.BeginTransaction();
                try
                {
                    if(!ProductExists(mycon, request.ProductId))
                    {
                        return BadRequest("Product doesn't exist");
                    }
                    if(!WarehouseExists(mycon, request.WarehouseId))
                    {
                        return BadRequest("Warehouse doesn't exist");
                    }
                    if(!OrderExists(mycon, request.ProductId, request.Amount, request.CreatedAt))
                    {
                        return BadRequest("Order doesn't exist or is already fulfilled");
                    }

                    int ProductWarehouseId;
                    string queryPrice = @"SELECT Price FROM Product WHERE IdProduct = @ProductId";
                    using (transaction)
                    {
                        var myCommand = mycon.CreateCommand();
                        myCommand.Transaction = transaction;
                        //Get the price for the later INSERT
                        myCommand.CommandText = @"SELECT Price FROM Product WHERE IdProduct = @ProductId";
                        myCommand.Parameters.AddWithValue("@ProductId", request.ProductId);
                        decimal price = Convert.ToDecimal(myCommand.ExecuteScalar()) * request.Amount;

                        //Upadte the order table
                        myCommand.CommandText = @"UPDATE [Order] SET FulfilledAt = GETDATE()
                                                  WHERE IdProduct = @ProductId AND Amount = @Amount AND CreatedAt < @CreatedAt AND FulfilledAt IS NULL";
                        myCommand.Parameters.AddWithValue("@ProductId", request.ProductId);
                        myCommand.Parameters.AddWithValue("@Amount", request.Amount);
                        myCommand.Parameters.AddWithValue("@CreatedAt", request.CreatedAt);
                        myCommand.ExecuteNonQuery();

                        //Get the OrderId
                        myCommand.CommandText = @"SELECT IdOrder
                                                WHERE IdProduct = @ProductId AND Amount = @Amount AND CreatedAt < @CreatedAt AND FulfilledAt IS NULL";
                        myCommand.Parameters.AddWithValue("@ProductId", request.ProductId);
                        myCommand.Parameters.AddWithValue("@Amount", request.Amount);
                        myCommand.Parameters.AddWithValue("@CreatedAt", request.CreatedAt);
                        int OrderId = (int)myCommand.ExecuteScalar();

                        //Insert into Product_Warehouse
                        myCommand.CommandText = @"INSERT INTO Product_Warehouse (IdWarehouse, IdProduct, IdOrder, Amount, Price, CreatedAt)
                                                  VALUES (@WarehouseId, @ProductId, @OrderId, @Amount, @Price, @CreatedAt)";
                        myCommand.Parameters.AddWithValue("@WarehouseId", request.WarehouseId);
                        myCommand.Parameters.AddWithValue("@ProductId", request.ProductId);
                        myCommand.Parameters.AddWithValue("@OrderId", OrderId);
                        myCommand.Parameters.AddWithValue("@Amount", request.Amount);
                        myCommand.Parameters.AddWithValue("@Price", price);
                        myCommand.Parameters.AddWithValue("@CreatedAt", request.CreatedAt);
                        myCommand.ExecuteNonQuery();

                        //Get the latest Product_Warehouse ID
                        myCommand.CommandText = "SELECT IdProductWarehouse FROM Product_Warehouse ORDER BY CreatedAt DESC";
                        ProductWarehouseId = (int)myCommand.ExecuteScalar();
                    }
                    transaction.Commit();
                    return Ok($"Product added Succesfully. Product_Warehouse ID: {ProductWarehouseId}");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return StatusCode(500, $"An error occured: {ex.Message}");
                }
            }

        }
    }
}
