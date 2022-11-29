using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Registration_Login.Models
{
    public class hospitals
    {
        public int Id { get; set; }
        public string hospitalname { get; set; }
        public string facilities { get; set; }
        public string department { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string UserName { get; set; }

        /*public int doctorlistId { get; set; }
        [ForeignKey("doctorlistId")]
        public doctorlist doctorlist { get; set; }*/
    }
}
