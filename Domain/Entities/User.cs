using System.Text.Json.Serialization;

namespace Domain.Entities
{
    public class User
    {
        public int Id { get; set; }

        public string Login { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        [JsonIgnore]
        public string Hashed_Password { get; set; }

        public Boolean IsActive { get; set; }
    }
}
