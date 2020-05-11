using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_managment.Contracts
{
    public interface IRepositoryBase<T> where T : class
        // i can pass any class and will act in any operation in the interface 
        // CRUL , create-update-delete-view from database 
    {
        //new modification using Task for Async is to add Task<> and Async in the functions and methods

        Task <ICollection<T>> FindAll();

        Task<T> FindById(int id);

        Task<bool> isExist(int id);

        Task<bool> create(T entity);
        Task<bool> update(T entity);
        Task<bool> Delete(T entity);
        Task<bool> Save();


        //ICollection<T> FindAll();

        //T FindById(int id);

        //bool isExist(int id);

        //bool create(T entity);
        //bool update(T entity);
        //bool Delete(T entity);
        //bool Save();


    }
}
