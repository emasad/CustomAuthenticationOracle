using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class TSubject : BaseEntity
    {
        [Column(TypeName = "VARCHAR2")]
        [StringLength(50)]
        public string SubjectName { get; set; }

        [Column(TypeName = "VARCHAR2")]
        [StringLength(50)]
        public string Author { get; set; }
    }
}
