using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Role
    {
        public int RoleId { get; set; }
        [Column(TypeName = "NVARCHAR2")]
        [StringLength(100)]
        public string RoleName { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
