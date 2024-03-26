using System.ComponentModel;

namespace Console.Models.Client
{
    [Description("Credentials of Person")]
    public class Credentials
    {
        [Description("Name of the Person")]
        public string Name { get; set; }

        [Description("Username of the Person")]
        public string Username { get; set; }

        [Description("Password of the Person")]
        public string Password { get; set; }
    }
}
