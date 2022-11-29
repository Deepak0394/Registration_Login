using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Registration_Login.Models
{
    public class Clinic
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        [Display(Name = "Phone Number")]
        public string PhoneNo { get; set; }
        [Required]
        [Display(Name = "Emergency Number")]
        public string EmergencyNo { get; set; }
        [Required]
        public string Facilities { get; set; }
        public int doctorlistId { get; set; }
        [ForeignKey("doctorlistId")]
        public doctorlist doctorlist { get; set; }
    }
}
