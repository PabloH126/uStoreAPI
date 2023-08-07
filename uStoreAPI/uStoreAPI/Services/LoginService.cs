using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using uStoreAPI.Dtos;
using uStoreAPI.ModelsAzureDB;

namespace uStoreAPI.Services
{
    public class LoginService
    {
        private readonly UstoreContext context;
        public LoginService(UstoreContext _context)
        {
            context = _context;
        }

        public async Task<CuentaAdministrador?> GetAdmin(LoginDto loginData)
        {
            return await context.CuentaAdministradors.SingleOrDefaultAsync(p => p.Email == loginData.Email && p.Password == loginData.Password);
        }
    }
}
