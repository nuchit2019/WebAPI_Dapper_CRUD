using APIDapperCRUD.Models;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data.Common;

namespace APIDapperCRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly string connectionString;
        //ctor
        public CustomersController(IConfiguration configuration )
        {
            connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        [HttpPost]
        public IActionResult Create(CustomerDto  customerDto)
        {
            try
            {
                using (var con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string sql = @"
                        INSERT INTO [Customers]
                                    ([CustomerID], [CompanyName], [ContactName], [ContactTitle], [Address], 
                                     [City], [Region], [PostalCode], [Country])
                        VALUES
                                    (@CustomerID, @CompanyName, @ContactName, @ContactTitle, @Address, 
                                     @City, @Region, @PostalCode, @Country);                                     
                                ";

                    var customer = new Customer()
                    {
                       CustomerID = customerDto.CustomerID,
                       CompanyName = customerDto.CompanyName,
                       ContactName = customerDto.ContactName,   
                       Address = customerDto.Address,
                       City = customerDto.City,
                       Region = customerDto.Region,
                       PostalCode = customerDto.PostalCode, 
                       Country = customerDto.Country,

                    };


                    var rowsAffected = con.Execute(sql, customer);
                    if (rowsAffected > 0)
                    {
                        return Ok(customerDto); // ส่งคืนข้อมูลลูกค้าที่ถูกสร้างใหม่
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("We have an exception:\n"+ex.Message);
            }

            return BadRequest();
        }

        [HttpGet]
        public IActionResult GetCustomers()
        {
            List<Customer> customers = new List<Customer>();

            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = "SELECT *  FROM [Customers]";
                    var data = conn.Query<Customer>(sql);
                    customers = data.ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("We have exception: \n" + ex.Message);
                return BadRequest();
            }
            return Ok(customers);
        }

        [HttpGet("{id}")]
        public IActionResult GetCustomer(string id)
        {
            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = "SELECT *  FROM [Customers] where [CustomerID] = @CustomerID";
                  var customer=  conn.QuerySingleOrDefault<Customer>(sql,new { CustomerID = id });
                    if (customer != null)
                    {
                        return Ok(customer);
                    }

                }
            }
            catch (Exception ex) {
                Console.WriteLine("We have exception: \n" + ex.Message);
                return BadRequest();
            }
            return BadRequest();
        }

        [HttpPut("{id}")]
        public IActionResult PutCustomer(string id,CustomerDto customerDto) {
            try
            {

                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = @"
                                UPDATE [Northwind].[dbo].[Customers]
                                SET
                                    [CompanyName] = @CompanyName,
                                    [ContactName] = @ContactName, 
                                    [Address] = @Address,
                                    [City] = @City,
                                    [Region] = @Region,
                                    [PostalCode] = @PostalCode,
                                    [Country] = @Country
                                WHERE
                                    [CustomerID] = @CustomerID;
                                 ";

                    var customer = new Customer
                    {
                        CustomerID = id,
                        CompanyName = customerDto.CompanyName,
                        ContactName = customerDto.ContactName,
                        Address = customerDto.Address,
                        City = customerDto.City,
                        PostalCode = customerDto.PostalCode,
                        Country = customerDto.Country

                    };
                    int count = conn.Execute(sql, customer);
                    if (count < 1)
                    {
                        return NotFound();// หากไม่มีแถวใดถูกอัปเดต แสดงว่าไม่พบลูกค้า
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("We have exception: \n" + ex.Message);
                return BadRequest(); // แสดงว่าเกิดข้อผิดพลาดบางอย่าง
            }

            return GetCustomer(id); // หากการอัปเดตสำเร็จ ... ไปดึง Customer ตาม id แล้วส่งกลับ
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCustomer(string id)
        {
            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = @"
                                DELETE FROM [Northwind].[dbo].[Customers]
                                WHERE [CustomerID] = @CustomerID;
                            ";
                  int count =   conn.Execute(sql, new { CustomerID = id });
                    if (count < 1)
                    {
                        return NotFound(); // หากไม่มีแถวใดถูกลบ แสดงว่าไม่พบลูกค้า
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("We have exception: \n" + ex.Message);
                return BadRequest();
            }

            return Ok();// หากลบสำเร็จ ส่งคืนสถานะ 200 OK

        }

    }
}
