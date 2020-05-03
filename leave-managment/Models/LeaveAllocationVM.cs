using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace leave_managment.Models
{
    public class LeaveAllocationVM
    {
        public int Id { get; set; }
        public int NumberOfDays { get; set; }
        public DateTime DateCreated { get; set; }
        public int Period { get; set; }
       
        public EmployeeVM Employee { get; set; }

        public string EmployeeId { get; set; }
        
        public LeaveTypeVM LeaveType { get; set; }
        public int LeaveTypeId { get; set; }
        //make a list of employee and leave types

        //we delete it for details in Employee manage 
        //public IEnumerable<SelectListItem> Employees { get; set; }
       // public IEnumerable<SelectListItem> LeaveTypes { get; set; }

    }
    //VM
    public class CreateLeaveAllocationVM
    {
        public int NumberUpdated { get; set; }
        public List <LeaveTypeVM> Leavetypes { get; set; }
    }
    // a new VM to simple the view from the extra hidden things from the original 
    //this is to be specific
    public class EditLeaveAllocationVM
    {
        public int Id { get; set; }
        [Display(Name = "Number Of Days")]
        [Range(1, 50, ErrorMessage = "Enter Valid Number")]
        public int NumberOfDays { get; set; }
        public EmployeeVM Employee { get; set; }
        public string EmployeeId { get; set; }

        public LeaveTypeVM LeaveTypes { get; set; }
        
    }
    public class ViewAllocationVM
    {
        public EmployeeVM Employee { get; set; }
        public string EmployeeId { get; set; }
        public List<LeaveAllocationVM> LeaveAllocations { get; set; }
    }
}
