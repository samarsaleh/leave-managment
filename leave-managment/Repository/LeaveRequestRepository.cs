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
        public async Task<bool> create(LeaveRequest entity)
        {
           await _db.LeaveRequests.AddAsync(entity);
            return await Save();
            //save
        }

        public async Task<bool> Delete(LeaveRequest entity)
        {
            _db.LeaveRequests.Remove(entity);
            //need to be saved after change using save()
            return await Save();
        }
        //returns all the recored in the table  which is here a class
        public async Task<ICollection<LeaveRequest>> FindAll()
        {

            var LeaveHistorys = await _db.LeaveRequests
                .Include(q => q.RequestingEmployee)
                .Include(q => q.ApprovedBy)
                .Include(q => q.LeaveType).ToListAsync();
            //from database so we need _db we can access any thing in the context 
            return LeaveHistorys;
            //var leavetypes=_db.LeaveTypes.ToList();
        }

        public async Task<LeaveRequest> FindById(int id)
        {

            var LeaveHistorys =await  _db.LeaveRequests
                .Include(q => q.RequestingEmployee)
                .Include(q => q.ApprovedBy)
                .Include(q => q.LeaveType).FirstOrDefaultAsync(q=>q.Id==id);
            //from database so we need _db we can access any thing in the context 
            return LeaveHistorys;
            
        }

        public async Task<ICollection<LeaveRequest>> GetLeaveRequestsByEmployee(string employeeid)
        {
            var leaveRequests = await FindAll();
            return leaveRequests.Where(q => q.RequestingEmployeeId == employeeid).ToList();
            
        }

        public async Task<bool> isExist(int id)
        {
            var exists = await _db.LeaveTypes.AnyAsync(q => q.Id == id);// is there any object in leavetypes has this id?
            return exists;
        }

        public async Task<bool> Save()
        {
            //if there is a  change if no changes then 
            var changes =await  _db.SaveChangesAsync();
            return changes > 0;
        }

        public async Task<bool> update(LeaveRequest entity)
        {
            // we do the update then call the save , so it will find a change 
            _db.LeaveRequests.Update(entity);
            return await Save();
        }
   
}
}
