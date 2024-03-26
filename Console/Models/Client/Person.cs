using System.ComponentModel;

namespace Console.Models.Client
{
    [Description("Request model")]
    public class Person
    {
        [Description("Name of the person")]
        public string Name { get; set; }

        [Description("Email of the person")]
        public string Email { get; set; }

        [Description("Phone number of the person")]
        public string PhoneNumber { get; set; }
    }
}
