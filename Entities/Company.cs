using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Company
    {

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string DataBaseName { get; set; }

        public string AdminId { get; set; }

        public virtual AppUser Admin { get; set; }

    }
}
