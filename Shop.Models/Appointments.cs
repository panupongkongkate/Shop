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

        [Display(Name ="วัน")]
        public DateTime AppointmentDate { get; set; }

        [Display(Name = "เวลา")]
        [NotMapped]
        public DateTime AppointmentTime { get; set; }

        [Display(Name = "ชื่อ-นามสกุล.")]
        [Required(ErrorMessage = "กรุณากรอกชื่อ-นามสกุล.")]
        public string CustomerName { get; set; }

        [Display(Name = "เบอร์โทรศัพท")]
        [Required(ErrorMessage = "กรุณากรอกเบอร์โทรศัพท.")]
        public string CustomerPhoneNumber { get; set; }

        [Display(Name = "อีเมล์")]
        [Required(ErrorMessage = "กรุณากรอกอีเมล์")]
        public string CustomerEmail { get; set; }
        public bool isConfirmed { get; set; }
        [Display(Name = "ใบเสร็จ")]
        public int Receiptnumber { get; set; }
    }
}
