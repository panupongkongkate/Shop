using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Shop.Models
{
   public class Appointments
    {
        public int Id { get; set; }

        public DateTime AppointmentDate { get; set; }

        [NotMapped]
        public DateTime AppointmentTime { get; set; }

        [Required(ErrorMessage = "กรุณากรอกชื่อ-นามสกุล.")]
        public string CustomerName { get; set; }
        [Required(ErrorMessage = "กรุณากรอกเบอร์โทรศัพท.")]
        public string CustomerPhoneNumber { get; set; }
        [Required(ErrorMessage = "กรุณากรอกอีเมล์")]
        public string CustomerEmail { get; set; }
        public bool isConfirmed { get; set; }
        public int Receiptnumber { get; set; }
    }
}
