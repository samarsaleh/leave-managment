﻿using leave_managment.Contracts;
using leave_managment.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_managment.Repository
{
    public class LeaveHistoryRepository : ILeaveHistoryRepository
    {
        private readonly ApplicationDbContext _db;

        public LeaveHistoryRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public bool create(LeaveHistory entity)
        {
            _db.LeaveHistories.Add(entity);
            return Save();
            //save
        }

        public bool Delete(LeaveHistory entity)
        {
            _db.LeaveHistories.Remove(entity);
            //need to be saved after change using save()
            return Save();
        }
        //returns all the recored in the table  which is here a class
        public ICollection<LeaveHistory> FindAll()
        {
            //from database so we need _db we can access any thing in the context 
            return _db.LeaveHistories.ToList();
            //var leavetypes=_db.LeaveTypes.ToList();
        }

        public LeaveHistory FindById(int id)
        {
            var LeaveHistory = _db.LeaveHistories.Find(id);
            return LeaveHistory;
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

        public bool update(LeaveHistory entity)
        {
            // we do the update then call the save , so it will find a change 
            _db.LeaveHistories.Update(entity);
            return Save();
        }
   
}
}
