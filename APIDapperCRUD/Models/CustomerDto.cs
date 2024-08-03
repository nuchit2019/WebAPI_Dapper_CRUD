using System.ComponentModel.DataAnnotations;

namespace APIDapperCRUD.Models
{
    public class CustomerDto
    {
        [Required]
        public string CustomerID { get; set; }

        [Required,MaxLength(100)]
        public string CompanyName { get; set; }
        public string ContactName { get; set; } = ""; 
        public string Address { get; set; } = "";
        public string City { get; set; } = "";
        public string Region { get; set; } = "";
        public string PostalCode { get; set; } = "";
        public string Country { get; set; } = "";
         
    }
}
