using Microsoft.AspNetCore.Mvc;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq.Expressions;
using Task6.Models;

namespace Task6.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WarehouseController : ControllerBase
    {
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
                    var ProductExists = mycon.CreateCommand();
                    ProductExists.Transaction = transaction;
                    ProductExists.CommandText = @"SELECT 1 FROM Product WHERE IdProduct = @ProductId";
                    ProductExists.Parameters.AddWithValue("@ProductId", request.ProductId);

                    object result = ProductExists.ExecuteScalar(); 

                    if(result == null || result == DBNull.Value)
                    {
                        return BadRequest("Product doesn't exist");
                    }

                    var WarehouseExists = mycon.CreateCommand();
                    WarehouseExists.Transaction = transaction;
                    WarehouseExists.CommandText = @"SELECT 1 FROM Warehouse WHERE IdWarehouse = @WarehouseId";
                    WarehouseExists.Parameters.AddWithValue("@WarehouseId", request.WarehouseId);

                    result = WarehouseExists.ExecuteScalar();

                    if(result == null || result == DBNull.Value)
                    {
                        return BadRequest("Warehouse doesn't exist");
                    }

                    var OrderExists = mycon.CreateCommand();
                    OrderExists.Transaction = transaction;
                    OrderExists.CommandText = @"SELECT IdOrder FROM [Order] WHERE IdProduct = @ProductId AND Amount = @Amount AND CreatedAt<@CreatedAt AND FulfilledAt = NULL";
                    OrderExists.Parameters.AddWithValue("@ProductId", request.ProductId);
                    OrderExists.Parameters.AddWithValue("@Amount", request.Amount);
                    OrderExists.Parameters.AddWithValue("@CreatedAt", request.CreatedAt);

                    result = OrderExists.ExecuteScalar();

                    if (result == null || result == DBNull.Value);
                    {
                        return BadRequest("Order doesn't exist or is already fulfilled");
                    }

                    int orderId = Convert.ToInt32(result);
                    

                    var UpdateOrder = mycon.CreateCommand();
                    UpdateOrder.Transaction = transaction;
                    UpdateOrder.CommandText = @"UPDATE [Order] SET FulfilledAt = @CreatedAt WHERE IdProduct = @ProductId";
                    UpdateOrder.Parameters.AddWithValue("@CreatedAt", request.CreatedAt);
                    UpdateOrder.Parameters.AddWithValue("@IdProduct", request.ProductId);
                    UpdateOrder.ExecuteNonQuery();

                    var GetPrice = mycon.CreateCommand();
                    GetPrice.Transaction = transaction;
                    GetPrice.CommandText = @"SELECT Price FROM Product WHERE IdProduct = @ProductId";

                    result = GetPrice.ExecuteScalar();
                    decimal price = Convert.ToDecimal(result);

                    decimal priceTotal = price * request.Amount;

                    SqlCommand insertWarehouse = mycon.CreateCommand();
                    insertWarehouse.Transaction = transaction;
                    insertWarehouse.CommandText = @"INSERT INTO Product_Warehouse (IdProduct, IdWarehouse, IdOrder, Amount, Price, CreatedAt) "" +
                                                 ""VALUES (@ProductId, @WarehouseId, @OrderId, @Amount, @PriceTotal, @CreatedAt)";
                    insertWarehouse.Parameters.AddWithValue("@ProductId", request.ProductId);
                    insertWarehouse.Parameters.AddWithValue("@WarehouseId", request.WarehouseId);
                    insertWarehouse.Parameters.AddWithValue("@Amount", request.Amount);
                    insertWarehouse.Parameters.AddWithValue("@Price", priceTotal);
                    insertWarehouse.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                    insertWarehouse.Parameters.AddWithValue("@OrderId", orderId);

                    insertWarehouse.ExecuteNonQuery();

                    SqlCommand SelectId = mycon.CreateCommand();
                    SelectId.Transaction = transaction;
                    SelectId.CommandText = @"SELECT IdProductWarehouse FROM Product_Warehouse WHERE IdOrder = @OrderId";
                    SelectId.Parameters.AddWithValue("@OrderId", orderId);
                    result = SelectId.ExecuteScalar();

                    int ProductWarehouseId = Convert.ToInt32(result);
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
