using DAL.Entities;
using DAL.Interface;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Implementations
{
    public class ProductRepository : IProductRepository
    {
       public IConfiguration _configuration { get; }
       public  string ConnectionString;
        public ProductRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            ConnectionString = _configuration["ConnectionStrings:DbConnection"];
        }

        public int DeleteProduct(int productId)
        {
            using(SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlTransaction sqltrans = connection.BeginTransaction();

                var param = new DynamicParameters();
                param.Add("@ProductId", productId);

                var result = connection.Execute("DeleteProductById",param,sqltrans,0,System.Data.CommandType.StoredProcedure);
                if(result != 0)
                {
                    sqltrans.Commit();
                }
                else
                {
                    sqltrans.Rollback();
                }
                return result;
            }
        }

        public IEnumerable<Product> GetAllProducts()
        {
            string sqlquery = "SELECT * FROM Products";
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {

                var products = connection.Query<Product>(sqlquery); 
                return products.ToList();
            }
        }

        public IEnumerable<Category> GetCategories()
        {
            string sqlQuery = "SELECT * FROM Categories"; 
            using (SqlConnection con = new SqlConnection(ConnectionString)) 
            { 
                var categories = con.Query<Category>(sqlQuery); 
                return categories.ToList(); 
            }
        }

        public Product GetSingleProduct(int productId)
        {
            using(SqlConnection connection = new SqlConnection(ConnectionString))
            {
                var param = new DynamicParameters();    
                param.Add("@ProductId", productId);

                return connection.Query<Product>("usp_getproduct", param, null, true, 0, System.Data.CommandType.StoredProcedure).SingleOrDefault();

            }
        }

        public int InsertProduct(Product Product)
        {
            using(SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlTransaction sqltrans = connection.BeginTransaction();

                var param = new DynamicParameters();
                param.Add("@Name", Product.Name);
                param.Add("@UnitPrice", Product.UnitPrice);
                param.Add("@Description", Product.Description);
                param.Add("@CategoryId", Product.CategoryId);

                var result = connection.Execute("AddNewProductDetails", param, sqltrans, 0, System.Data.CommandType.StoredProcedure);

                if(result > 0)
                {
                    sqltrans.Commit();
                }
                else
                {
                    sqltrans.Rollback();
                } 
                return result;
            }
        }

        public int UpdateProduct(Product Product)
        {
            using(SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlTransaction sqltrans = connection.BeginTransaction();

                var param = new DynamicParameters();
                param.Add("@ProductId", Product.ProductId);
                param.Add("@Name", Product.Name);
                param.Add("@UnitPrice", Product.UnitPrice);
                param.Add("@Description", Product.Description);
                param.Add("@CategoryId", Product.CategoryId);

                var result = connection.Execute("UpdateProductDetails", param, sqltrans, 0, System.Data.CommandType.StoredProcedure);

                if(result>0)
                {
                    sqltrans.Commit();
                }
                else
                {
                    sqltrans.Rollback();
                } 
                return result;
            }
        }
    }
}
