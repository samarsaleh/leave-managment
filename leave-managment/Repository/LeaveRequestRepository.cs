using leave_managment.Contracts;
using leave_managment.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_managment.Repository
{
    public class LeaveRequestRepository : ILeaveRequestRepository
    {
        private readonly ApplicationDbContext _db;

        public LeaveRequestRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public bool create(LeaveRequest entity)
        {
            _db.LeaveRequests.Add(entity);
            return Save();
            //save
        }

        public bool Delete(LeaveRequest entity)
        {
            _db.LeaveRequests.Remove(entity);
            //need to be saved after change using save()
            return Save();
        }
        //returns all the recored in the table  which is here a class
        public ICollection<LeaveRequest> FindAll()
        {

            var LeaveHistorys = _db.LeaveRequests
                .Include(q => q.RequestingEmployee)
                .Include(q => q.ApprovedBy)
                .Include(q => q.LeaveType).ToList();
            //from database so we need _db we can access any thing in the context 
            return LeaveHistorys;
            //var leavetypes=_db.LeaveTypes.ToList();
        }

        public LeaveRequest FindById(int id)
        {

            var LeaveHistorys = _db.LeaveRequests
                .Include(q => q.RequestingEmployee)
                .Include(q => q.ApprovedBy)
                .Include(q => q.LeaveType).FirstOrDefault(q=>q.Id==id);
            //from database so we need _db we can access any thing in the context 
            return LeaveHistorys;
            
        }

        public ICollection<LeaveRequest> GetLeaveRequestsByEmployee(string employeeid)
        {
            var leaveRequests = FindAll().Where(q => q.RequestingEmployeeId == employeeid).ToList();
            return leaveRequests;
        }

        public bool isExist(int id)
        {
            var exists = _db.LeaveTypes.Any(q => q.Id == id);// is there any object in leavetypes has this id?
            return exists;
        }

        public bool Save()
        {
            //if there is a  change if no changes then 
            var changes = _db.SaveChanges();
            return changes > 0;
        }

        public bool update(LeaveRequest entity)
        {
            // we do the update then call the save , so it will find a change 
            _db.LeaveRequests.Update(entity);
            return Save();
        }
   
}
}
