using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityLens.Data.Data
{
    public class PostDto
    {
        public int PostId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime DateTime { get; set; }
        public bool IsOpen { get; set; }
        public bool IsResolved { get; set; }
        public string Description { get; set; } = string.Empty;

        public List<string> Images { get; set; } = new List<string>();
        public int Votes { get; set; }
    }
   
    public class CreatePostDto
    {
        public string Title { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime DateTime { get; set; } = DateTime.UtcNow;
        public bool IsOpen { get; set; } = true;
        public bool IsResolved { get; set; } = false;
        public string Description { get; set; } = string.Empty;
        public List<string> Images { get; set; } = new();

        public string NotificationEmail { get; set; } = string.Empty;
    }

    public class UpdatePostDto
    {
        public string Title { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime DateTime { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool IsOpen { get; set; }
        public bool IsResolved { get; set; }
    }
}
