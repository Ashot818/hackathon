using CityLens.Data.Data;
using CityLens.Data.Data.Entities;
using CityLens.Services.Abstract;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MimeKit.Utils;

namespace CityLens.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostsController(IPostService postService)
        {
            _postService = postService;
        }

        // GET: api/Posts
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var posts = await _postService.GetAllPostsAsync();
            return Ok(posts);
        }

        // GET: api/Posts/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var post = await _postService.GetPostByIdAsync(id);
            if (post == null) return NotFound();
            return Ok(post);
        }

        // POST: api/Posts
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePostDto createDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsPath))
                Directory.CreateDirectory(uploadsPath);

            var savedFilePaths = new List<string>();

            // Сохраняем картинки локально и собираем пути
            foreach (var base64 in createDto.Images)
            {
                var fileName = $"{Guid.NewGuid()}.png";
                var filePath = Path.Combine(uploadsPath, fileName);

                var bytes = Convert.FromBase64String(base64);
                await System.IO.File.WriteAllBytesAsync(filePath, bytes);

                savedFilePaths.Add(filePath);
            }

            var created = await _postService.CreatePostAsync(createDto);

            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("CityLens App", "steven.shelbycargollc@gmail.com"));
                message.To.Add(new MailboxAddress("", createDto.NotificationEmail));
                message.Subject = $"Նոր գրառում․ {created.Title}";

                // Создаем альтернативное HTML-представление письма с вложенными картинками
                var builder = new BodyBuilder();

                // HTML часть с cid-ссылками на изображения
                var imagesHtmlParts = new List<string>();

                for (int i = 0; i < savedFilePaths.Count; i++)
                {
                    var path = savedFilePaths[i];
                    var image = builder.LinkedResources.Add(path);
                    image.ContentId = MimeUtils.GenerateMessageId();

                    // Вставляем картинку с cid
                    imagesHtmlParts.Add($"<img src=\"cid:{image.ContentId}\" style='max-width:400px;' />");
                }

                var imagesHtml = string.Join("<br/>", imagesHtmlParts);

                builder.HtmlBody = $@"
<h2>{created.Title}</h2>
<p>Տարածաշրջան: {created.Region}</p>
<p>Հասցե: {created.Address}</p>
<p>Ամսաթիվ: {created.DateTime}</p>
<p>Բաց է: {(created.IsOpen ? "Այո" : "Ոչ")}</p>
<p>Լուծված է: {(created.IsResolved ? "Այո" : "Ոչ")}</p>
<p>Նկարներ:</p>
{imagesHtml}
";

                message.Body = builder.ToMessageBody();

                using var client = new MailKit.Net.Smtp.SmtpClient();
                client.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                client.Authenticate("steven.shelbycargollc@gmail.com", "kimt bnnq cezo seqi");
                client.Send(message);
                client.Disconnect(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при отправке письма: {ex.Message}");
            }

            return CreatedAtAction(nameof(Get), new { id = created.PostId }, created);
        }


        // PUT: api/Posts/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePostDto updateDto)
        {
            var post = new Post
            {
                Id = id,
                Title = updateDto.Title,
                Region = updateDto.Region,
                Address = updateDto.Address,
                Description = updateDto.Description,
                IsOpen = updateDto.IsOpen,
                IsResolved = updateDto.IsResolved,
                DateTime = updateDto.DateTime

            };

            var updated = await _postService.UpdatePostAsync(post);
            if (!updated) return NotFound();

            return NoContent();
        }

        // DELETE: api/Posts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _postService.DeletePostAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }

}
