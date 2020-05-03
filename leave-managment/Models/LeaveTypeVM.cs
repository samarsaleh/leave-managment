using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace leave_managment.Models
{
    public class LeaveTypeVM
    {//showing data
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [Display(Name="Default Number Of Days")]
        [Range(1,25,ErrorMessage ="please enter A valid number")]
        public int DefaultDays { get; set; }
       

        [Display(Name="Date Created")] // changes the label 
        public DateTime? DateCreated { get; set; }// ? is like required
    }

}
