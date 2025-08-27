using CityLens.Services.Abstract;
using CityLens.Web.Data;
using CityLens.Data.Data.Entities;
using Microsoft.EntityFrameworkCore;
using CityLens.Data.Data;
using Microsoft.AspNetCore.Hosting;

namespace CityLens.Services.Concrete
{
    public class PostService : IPostService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public PostService(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

    

        public async Task<IEnumerable<PostDto>> GetAllPostsAsync()
        {
            var posts = await _context.Posts
                .Include(p => p.Images)
                .Include(p => p.Votes)
                .ToListAsync();

            return posts.Select(MapToDto);
        }

        public async Task<PostDto?> GetPostByIdAsync(int id)
        {
            var post = await _context.Posts
                .Include(p => p.Images)
                .Include(p => p.Votes)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null) return null;
            return MapToDto(post);
        }
        private string SaveBase64Image(string base64)
        {
            if (string.IsNullOrEmpty(base64)) return null;

            // Папка для хранения
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "images");
            Directory.CreateDirectory(uploadPath);

            // Имя файла
            var fileName = $"{Guid.NewGuid()}.png";
            var filePath = Path.Combine(uploadPath, fileName);

            // Сохраняем файл
            var bytes = Convert.FromBase64String(base64);
            File.WriteAllBytes(filePath, bytes);

            // Формируем полный URL
            //var baseUrl = "https://localhost:7203"; // замените на ваш API URL
            var ngrokBaseUrl = "https://0aaeaf8dc1cf.ngrok-free.app";

            return $"{ngrokBaseUrl}/uploads/images/{fileName}";
        }

        public async Task<PostDto> CreatePostAsync(CreatePostDto createDto)
        {
            var post = new Post
            {
                Title = createDto.Title,
                Region = createDto.Region,
                Address = createDto.Address,
                IsOpen = createDto.IsOpen,
                IsResolved = createDto.IsResolved,
                DateTime = createDto.DateTime
            };

            post.Images = createDto.Images
                .Select(b64 => new PostImage { ImageUrl = SaveBase64Image(b64) })
                .ToList();

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return MapToDto(post);
        }

        public async Task<bool> UpdatePostAsync(Post post)
        {
            var existing = await _context.Posts.FindAsync(post.Id);
            if (existing == null) return false;

            existing.Title = post.Title;
            existing.Region = post.Region;
            existing.Address = post.Address;
            existing.IsOpen = post.IsOpen;
            existing.IsResolved = post.IsResolved;
            existing.DateTime = post.DateTime;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeletePostAsync(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null) return false;

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return true;
        }

        private PostDto MapToDto(Post post)
        {
            return new PostDto
            {
                PostId = post.Id,
                Title = post.Title,
                Region = post.Region,
                Address = post.Address,
                DateTime = post.DateTime,
                IsOpen = post.IsOpen,
                IsResolved = post.IsResolved,
                Description = post.Description,
                Images = post.Images.Select(i => i.ImageUrl).ToList(), 
                Votes = post.Votes.Sum(v => v.VoteValue) 
            };
        }
    }
}
