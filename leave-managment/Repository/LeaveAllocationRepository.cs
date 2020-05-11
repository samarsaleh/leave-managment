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

        public async Task<bool> CheckAllocation(int leavetypeid, string employeeid)
        {
            //check the id
            var period = DateTime.Now.Year;
            var allocations = await FindAll();
            return allocations.Where(q => q.EmployeeId == employeeid && q.LeaveTypeId == leavetypeid && q.Period == period).Any();
            //any is used because this expressin is returning a collection while the func is return a bool 
           // return FindAll().Where(q => q.EmployeeId == employeeid && q.LeaveTypeId == leavetypeid && q.Period == period).Any();
        }

        public async Task<bool> create(LeaveAllocation entity)
        {
           await _db.LeaveAllocations.AddAsync(entity);
            return await Save();
            //save
        }

        public async Task<bool> Delete(LeaveAllocation entity)
        {
            _db.LeaveAllocations.Remove(entity);
            //need to be saved after change using save()
            return await Save();
        }
        //returns all the recored in the table  which is here a class
        public async Task<ICollection<LeaveAllocation>> FindAll()
        {
            //from database so we need _db we can access any thing in the context 
           var leaveallocations= await _db.LeaveAllocations
                .Include(q=>q.LeaveType)
                .Include(q=>q.Employee)
                .ToListAsync(); //cuz in details employee view we use leavetype
            //var leavetypes=_db.LeaveTypes.ToList();
            return leaveallocations;
        }

        public async Task<LeaveAllocation> FindById(int id)
        {
            var LeaveAllocation = await _db.LeaveAllocations
                 .Include(q => q.LeaveType)
                .Include(q => q.Employee)
                .FirstOrDefaultAsync(q=>q.Id==id);
            return LeaveAllocation;
        }
        //instead of find all 
        public async Task<ICollection<LeaveAllocation>> GetLeaveAllocationsByEmployee(string id)
        {
            //allocation matching the period 
            var period = DateTime.Now.Year;
            var alloc = await FindAll();
            return alloc.Where(q => q.EmployeeId == id &&q.Period==period).ToList();
        }

        public async Task<LeaveAllocation> GetLeaveAllocationsByEmployeeAndType(string id, int leaveTypeid)
        {
            var period = DateTime.Now.Year;
            var alloc = await FindAll();
            return alloc.FirstOrDefault(q => q.EmployeeId == id && q.Period == period && q.LeaveTypeId == leaveTypeid);
                
        }

        public async Task<bool> isExist(int id)
        {
            var exists = _db.LeaveTypes.AnyAsync(q => q.Id == id);// is there any object in leavetypes has this id?
            return await exists;
        }

        public async Task<bool> Save()
        {
            //if there is a  change if no changes then 
            var changes = await _db.SaveChangesAsync();
            return changes > 0;
        }

        public async Task< bool> update(LeaveAllocation entity)
        {
            // we do the update then call the save , so it will find a change 
            _db.LeaveAllocations.Update(entity);
            return await Save();
        }
    }
}

