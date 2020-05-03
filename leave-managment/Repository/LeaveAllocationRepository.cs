using leave_managment.Contracts;
using leave_managment.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_managment.Repository
{
    public class LeaveAllocationRepository : ILeaveAllocationRepository
    {
        private readonly ApplicationDbContext _db;

        public LeaveAllocationRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public bool CheckAllocation(int leavetypeid, string employeeid)
        {
            //check the id
            var period = DateTime.Now.Year;
            //any is used because this expressin is returning a collection while the func is return a bool 
            return FindAll().Where(q => q.EmployeeId == employeeid && q.LeaveTypeId == leavetypeid && q.Period == period).Any();
        }

        public bool create(LeaveAllocation entity)
        {
            _db.LeaveAllocations.Add(entity);
            return Save();
            //save
        }

        public bool Delete(LeaveAllocation entity)
        {
            _db.LeaveAllocations.Remove(entity);
            //need to be saved after change using save()
            return Save();
        }
        //returns all the recored in the table  which is here a class
        public ICollection<LeaveAllocation> FindAll()
        {
            //from database so we need _db we can access any thing in the context 
           var leaveallocations= _db.LeaveAllocations
                .Include(q=>q.LeaveType)
                .Include(q=>q.Employee)
                .ToList(); //cuz in details employee view we use leavetype
            //var leavetypes=_db.LeaveTypes.ToList();
            return leaveallocations;
        }

        public LeaveAllocation FindById(int id)
        {
            var LeaveAllocation = _db.LeaveAllocations
                 .Include(q => q.LeaveType)
                .Include(q => q.Employee)
                .FirstOrDefault(q=>q.Id==id);
            return LeaveAllocation;
        }
        //instead of find all 
        public ICollection<LeaveAllocation> GetLeaveAllocationsByEmployee(string id)
        {
            //allocation matching the period 
            var period = DateTime.Now.Year;
            return FindAll().Where(q => q.EmployeeId == id &&q.Period==period).ToList();
        }

        public LeaveAllocation GetLeaveAllocationsByEmployeeAndType(string id, int leaveTypeid)
        {
            var period = DateTime.Now.Year;
            return FindAll().FirstOrDefault(q => q.EmployeeId == id && q.Period == period && q.LeaveTypeId == leaveTypeid);
                
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

        public bool update(LeaveAllocation entity)
        {
            // we do the update then call the save , so it will find a change 
            _db.LeaveAllocations.Update(entity);
            return Save();
        }
    }
}

