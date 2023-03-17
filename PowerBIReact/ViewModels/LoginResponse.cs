namespace PowerBIReact.ViewModels
{
    public class LoginResponse
    {
        public string Id { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string Token { get; set; }

        public string[] UserRoles { get; set; }

        public bool IsActive { get; set; }
    }
}
