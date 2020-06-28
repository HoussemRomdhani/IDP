using System.Collections.Generic;

namespace IDP.Modes
{
    public class ApiResource
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string ClaimTypes { get; set; }
        public string Secret { get; set; }
    }
}
