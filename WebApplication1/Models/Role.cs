namespace WebApplication1.Models
{
    public class Role
    {
        public int Id { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public ICollection<ApplicationUser> Users { get; set; }
    }
}
