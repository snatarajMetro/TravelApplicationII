using System;
using System.Collections.Generic;

namespace TravelApplication.DAL.Repositories.Base
{
    /// <summary>
    /// IRepository interface
    /// It provides definition of basic CRUD operation method for repository class.
    /// </summary>
    /// <typeparam name="T">General entity class</typeparam>
    public interface IBaseRepository<T> : IDisposable where T : class
    {
        T GetById(object id);

        int GetAllCount(string query);

        ICollection<T> GetAll(string query);

        void Create(T entity);

        void Update(T entity);

        void Delete(IList<object> ids);
    }
}