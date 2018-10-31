using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class User
    {
        public int UserId { get; set; }
        [Column(TypeName = "NVARCHAR2")]
        [StringLength(100)]
        public string Username { get; set; }
        [Column(TypeName = "VARCHAR2")]
        [StringLength(100)]
        public string FirstName { get; set; }
        [Column(TypeName = "VARCHAR2")]
        [StringLength(50)]
        public string LastName { get; set; }
        [Column(TypeName = "VARCHAR2")]
        [StringLength(50)]
        public string Email { get; set; }
        [Column(TypeName = "VARCHAR2")]
        [StringLength(50)]
        public string Password { get; set; }
        
        public bool IsActive { get; set; }
        [Column(TypeName = "VARCHAR2")]
        [StringLength(256)]
        public string ActivationCode { get; set; }
        public virtual ICollection<Role> Roles { get; set; }
    }
}
