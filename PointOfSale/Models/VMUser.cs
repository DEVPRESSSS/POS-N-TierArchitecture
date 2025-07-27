namespace PointOfSale.Models
{
    public class VMUser
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string ProfilePath { get; set; }
        public int IsActive { get; set; }

    }
}
