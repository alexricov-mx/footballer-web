using System.Text.Json;

namespace FootballerWeb.Services
{
    public interface IContentService
    {
        Task<HomepageContent?> GetHomepageContentAsync();
        Task<CarouselData?> GetCarouselImagesAsync();
    }

    public class ContentService : IContentService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ContentService> _logger;

        public ContentService(HttpClient httpClient, ILogger<ContentService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<HomepageContent?> GetHomepageContentAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/content/homepage");
                
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    return JsonSerializer.Deserialize<HomepageContent>(jsonString, options);
                }
                
                _logger.LogWarning("Failed to fetch homepage content. Status: {StatusCode}", response.StatusCode);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching homepage content");
                return null;
            }
        }

        public async Task<CarouselData?> GetCarouselImagesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/content/carousel-images");
                
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    return JsonSerializer.Deserialize<CarouselData>(jsonString, options);
                }
                
                _logger.LogWarning("Failed to fetch carousel images. Status: {StatusCode}", response.StatusCode);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching carousel images");
                return null;
            }
        }
    }

    // DTOs para el contenido
    public class HomepageContent
    {
        public HeroSection Hero { get; set; } = new();
        public CarouselSection Carousel { get; set; } = new();
        public List<Feature> Features { get; set; } = new();
        public List<Testimonial> Testimonials { get; set; } = new();
        public List<Stat> Stats { get; set; } = new();
        public ContactInfo Contact { get; set; } = new();
        public DateTime LastUpdated { get; set; }
    }

    public class HeroSection
    {
        public string Title { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public CallToAction CallToAction { get; set; } = new();
    }

    public class CallToAction
    {
        public string Text { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
    }

    public class CarouselSection
    {
        public int AutoSlideInterval { get; set; } = 4000;
        public List<CarouselImage> Images { get; set; } = new();
    }

    public class CarouselData
    {
        public List<CarouselImage> Images { get; set; } = new();
        public int AutoSlideInterval { get; set; } = 4000;
    }

    public class CarouselImage
    {
        public string Url { get; set; } = string.Empty;
        public string Alt { get; set; } = string.Empty;
        public string Caption { get; set; } = string.Empty;
    }

    public class Feature
    {
        public string Icon { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool Highlight { get; set; }
    }

    public class Testimonial
    {
        public string Name { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Organization { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
    }

    public class Stat
    {
        public string Number { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
    }

    public class ContactInfo
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }
}