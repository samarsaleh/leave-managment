using leave_managment.Contracts;
using leave_managment.Data;
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
        public bool create(LeaveType entity)
        {
            _db.LeaveTypes.Add(entity);
            return Save();
            //save
        }

        public bool Delete(LeaveType entity)
        {
            _db.LeaveTypes.Remove(entity);
            //need to be saved after change using save()
            return Save();
        }
        //returns all the recored in the table  which is here a class
        public ICollection<LeaveType> FindAll()
        {
            //from database so we need _db we can access any thing in the context 
            return _db.LeaveTypes.ToList();
            //var leavetypes=_db.LeaveTypes.ToList();
        }

        public LeaveType FindById(int id)
        {
            var leavetype = _db.LeaveTypes.Find(id);            
            return leavetype;
        }

        public ICollection<LeaveType> GetEmployeesByLeaveType(int id)
        {
            throw new NotImplementedException();
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

        public bool update(LeaveType entity)
        {
            // we do the update then call the save , so it will find a change 
            _db.LeaveTypes.Update(entity);
            return Save();
        }
    }
}
