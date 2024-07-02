namespace RekvalifikaceApp.Models
{
    public class RoleModifications
    {
        public string RoleName { get; set; } = string.Empty;
        public string RoleId { get; set; } = string.Empty;
        public string[] AddIds { get; set; } = Array.Empty<string>();
        public string[] DeleteIds { get; set; } = Array.Empty<string>();
    }
}
