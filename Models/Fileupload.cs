using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models
{
    
    public class Fileupload
    {
        [Key]
        [ScaffoldColumn(false)]
        [Exclude]
        public int Id { get; set; }   
        [NotMapped]
        public IFormFile File { get; set; }    
        public string Name { get; set; }    
        public string Description { get; set; }
       
        public string filepath { get; set; }

        private class ExcludeAttribute : Attribute
        {
        }
    }
}
