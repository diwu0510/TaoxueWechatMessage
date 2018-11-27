namespace HZC.Core
{
    public partial class AppUser : IBaseModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Role { get; set; }
    }
}
