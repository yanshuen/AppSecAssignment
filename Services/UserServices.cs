using AppSecAssignment.Model;

namespace AppSecAssignment.Services
{
	public class UserServices
	{
        private AuthDbContext _context;

        public UserServices(AuthDbContext context)
        {
            _context = context;
        }

        public ApplicationUser? GetUserDetails(string email)
        {
            ApplicationUser? user = _context.Users.FirstOrDefault(x => x.Email.Equals(email));
            return user;
        }

        public void UpdateUser(ApplicationUser user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();
        }
    }
}
