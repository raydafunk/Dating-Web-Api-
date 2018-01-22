using System;
using System.Threading.Tasks;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class AuthRespository : IAuthRepository
    {
        private readonly DataContext _context;
        public AuthRespository(DataContext context)
        {
            _context = context;

        }
        public async Task<User> LoginAsync(string username, string password)
        {
           // store the user 
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);

            if (username == null)
                return null;
                
           if(!VerifyPasswordHasH(password, user.PasswordHash , user.PasswordSalt))
              return null;

              // aut is suscessfull 
              return user;
        }

        private bool VerifyPasswordHasH(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
          {
            
               var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
               for (int i =0; i < computedHash.Length; i++)
               {
                   if(computedHash[i] != passwordHash[i]) return false;
               }
               return true;
          }
        }

        public async Task<User> Register(User user, string password)
        {
           byte[] passwordHash, passwordSalt;
           CreatePaswordHash(password, out passwordHash, out passwordSalt);

           user.PasswordHash = passwordHash;
           user.PasswordSalt = passwordSalt;
           
           await _context.Users.AddAsync(user);
           await _context.SaveChangesAsync();

           return user;
        }

        private void CreatePaswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
          using (var hmac = new System.Security.Cryptography.HMACSHA512())
          {
              passwordSalt = hmac.Key;
              passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
          }
        }

        public async Task<bool> UserExists(string username)
        {
            if(await _context.Users.AnyAsync(x => x.Username == username))
               return true;

               return false;
        }
    }
}