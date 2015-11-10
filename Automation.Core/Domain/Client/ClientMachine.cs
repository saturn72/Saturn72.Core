using System;

namespace Automation.Core.Domain.Client
{
    public class ClientMachine : BaseEntity
    {
        public string IpAddress { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public ClientSettings Settings { get; set; }
        public DateTime? LastConnectionOn { get; set; }
    }
}