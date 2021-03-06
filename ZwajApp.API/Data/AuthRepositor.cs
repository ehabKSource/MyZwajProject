using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ZwajApp.API.Models;

namespace ZwajApp.API.Data
{
    public class AuthRepositor : IAuthRepository

    {
        private readonly DataContext _context;

        public AuthRepositor(DataContext context)
        {
            _context = context;

        }
        public async Task<User> Login(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x=>x.Username==username.ToLower());
            if(user==null)return null;

            if(!VerifyPasswordHash(password, user.PasswordSalt ,  user.PasswordHash )) return null; 
           
            return user;
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordhash , passwordSalt;
            CreatePasswordHash(password,out passwordhash,out passwordSalt);
            user.PasswordHash=passwordhash;
            user.PasswordSalt=passwordSalt;
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<bool> UserExists(string username)
        {
            if(await _context.Users.AnyAsync(x=>x.Username==username))return true;
            return false;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordSalt, byte[] passwordHash)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt)){
                
                var ComputedHash=hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < ComputedHash.Length; i++)
                {
                    if(ComputedHash[i]!=passwordHash[i]){
                        return false;
                    }
                    
                }
                return true;

            }
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512()){
                passwordSalt = hmac.Key;
                passwordHash=hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

            }
            
        }
    }
}