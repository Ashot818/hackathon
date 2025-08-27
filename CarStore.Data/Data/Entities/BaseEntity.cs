using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityLens.Data.Data.Entities
{

    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DateTime { get; set; } = DateTime.UtcNow;
        public bool IsOpen { get; set; } = true;
        public bool IsResolved { get; set; } = false;

        public ICollection<PostImage> Images { get; set; } = new List<PostImage>();

        public ICollection<PostVote> Votes { get; set; } = new List<PostVote>();
    }

    public class PostImage
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;

        public int PostId { get; set; }
        public Post Post { get; set; }
    }

    public class PostVote
    {
        public int Id { get; set; }
        public int VoteValue { get; set; }

        public int PostId { get; set; }
        public Post Post { get; set; }

        public int UserId { get; set; }
    }


}
