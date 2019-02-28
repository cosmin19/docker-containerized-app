namespace Enviroself.Areas.Admin.Features.User.Dto
{
    public class UserSmallAdminDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public bool EmailConfirmed { get; set; }
        public string CreatedOnUtc { get; set; }
        public string Role { get; set; }
    }
}
