namespace OnlinePropertyBookingPlatform.Models.DataTransferObjects
{
    public class UserDto
    {
        public int Id { get; set; }

        public string Username { get; set; } 

        public short PhoneNumber { get; set; }

        public string Email { get; set; } 

        public string Password { get; set; } 

        public string Role { get; set; } 

        public int? ReviewCount { get; set; }
    }
}
