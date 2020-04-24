using leave_managment.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_managment.Contracts
{
    public interface ILeaveTypeRepository : IRepositoryBase<LeaveType>
   // this interface will inherit the base interface using its CLASS
    {
        ICollection<LeaveType> GetEmployeesByLeaveType(int id);
    }
}
