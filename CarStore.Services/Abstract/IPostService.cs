using CityLens.Data.Data;
using CityLens.Data.Data.Entities;

namespace CityLens.Services.Abstract;

public interface IPostService
{
    Task<IEnumerable<PostDto>> GetAllPostsAsync();
    Task<PostDto?> GetPostByIdAsync(int id);
    Task<PostDto> CreatePostAsync(CreatePostDto createDto); 
    Task<bool> UpdatePostAsync(Post post);
    Task<bool> DeletePostAsync(int id);
}
