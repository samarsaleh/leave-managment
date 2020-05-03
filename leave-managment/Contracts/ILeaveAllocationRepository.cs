using leave_managment.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_managment.Contracts
{
    public interface ILeaveAllocationRepository :IRepositoryBase<LeaveAllocation>
    {
        bool CheckAllocation(int leavetypeid, string employeeid);
        ICollection<LeaveAllocation> GetLeaveAllocationsByEmployee(string id);
       LeaveAllocation GetLeaveAllocationsByEmployeeAndType(string id,int leaveTypeId);
    }
}
