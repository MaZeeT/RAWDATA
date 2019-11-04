using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebService.Models
{
    public class CategoryForCreation
    {
        [Required]
        [MaxLength(15)]
        public string Id { get; set; }
        [MaxLength(300)]
        public string Title { get; set; }
    }
}
