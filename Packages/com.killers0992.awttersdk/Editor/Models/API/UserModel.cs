namespace AwtterSDK.Editor.Models.API
{
    public class UserModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public bool Admin { get; set; }
        public DiscordInfoModel Discord { get; set; }
    }
}
