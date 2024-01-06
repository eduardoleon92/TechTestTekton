using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.Core.Entities;

namespace Test.DataAccess.Repositories
{
    public class AuthRepository
    {
        public User GetUserByUsername(string username)
        {
            return new User { UserId = 1, Username = "user", Password = "123456" };
        }
    }
}
