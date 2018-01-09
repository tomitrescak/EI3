
namespace Ei.Persistence
{
    using System.Collections.Generic;

    public class AuthorisationDao
    {
        public string Organisation { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public List<GroupDao> Groups { get; set; }
    }
}
