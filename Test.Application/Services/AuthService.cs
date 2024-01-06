using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.DataAccess.Repositories;

namespace Test.Application.Services
{
    public class AuthService
    {
        private readonly AuthRepository _authRepository;

        public AuthService(AuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        public bool ValidateUser(string username, string password)
        {
            var user = _authRepository.GetUserByUsername(username);
            
            return user != null && user.Password == password;
        }
    }
}
