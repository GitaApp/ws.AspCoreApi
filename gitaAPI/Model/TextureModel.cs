using System.ComponentModel.DataAnnotations;

namespace gitaAPI.Model
{
    public class TextureModel
    {
        [Key]
      
        public string ImageName { get; set; }
        public string ImagePath { get; set; }
        public List<IFormFile> imageList { get; set; }
    }
}
