using leave_managment.Contracts;
using leave_managment.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace leave_managment.Repository
{
    public class LeaveTypeRepository : ILeaveTypeRepository
    {
        //we need to conect it with database soo...called dependancy ingection
        private readonly ApplicationDbContext _db;

        public LeaveTypeRepository(ApplicationDbContext db)
        {
            _db = db;    
        }
        public async Task< bool> create(LeaveType entity) // modified using async and await ..
        {
          await  _db.LeaveTypes.AddAsync(entity);
            return await Save();
            //save
        }

        public async Task<bool> Delete(LeaveType entity)
        {
           // _db.LeaveTypes.Remove(entity);
            _db.LeaveTypes.Remove(entity);
            //need to be saved after change using save()
            return await Save();
        }
        //returns all the recored in the table  which is here a class
        public async Task<ICollection<LeaveType>> FindAll()
        {
            var leavetypes= await _db.LeaveTypes.ToListAsync(); ;
            //from database so we need _db we can access any thing in the context 
            return leavetypes;
                                
            //var leavetypes=_db.LeaveTypes.ToList();
        }

        public async Task<LeaveType> FindById(int id)
        {
            var leavetype = _db.LeaveTypes.FindAsync(id);
            return await leavetype;
        }

        public async Task<ICollection<LeaveType>> GetEmployeesByLeaveType(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<bool >isExist(int id)
        {
            var exists = _db.LeaveTypes.AnyAsync(q => q.Id == id);// is there any object in leavetypes has this id?
            return await exists;
        }

        public async Task<bool> Save()
        {
            //if there is a  change if no changes then 
          var changes = _db.SaveChangesAsync();
          return await  changes > 0;
        }

        public async Task<bool> update(LeaveType entity)
        {
            // we do the update then call the save , so it will find a change 
            _db.LeaveTypes.Update(entity);
            return await Save();
        }
    }
}
