using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IUserRepository
    {
        IQueryable<User> GetAll();
        User GetById(int id);
        public User GetByLogin(string login);
        void Add(User uuser);
        void Update(User uuser);
        void Deactivate(int id);
    }
}
