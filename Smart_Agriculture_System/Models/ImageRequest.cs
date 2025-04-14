namespace Smart_Agriculture_System.Models
{
    public class ImageRequest
    {
        public string TextPrompt { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
}
