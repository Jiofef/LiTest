using LiTest.Shared.Core.Community;
using LiTest.Shared.Core.Testing;

namespace LiTest.Server.Infrastructure.Testing
{
    public class LiTestEntity
    {
        // Metadata
        public Guid Id { get; set; }
        public Guid AuthorId { get; set; }
        public PublicityEnum Publicity { get; set; } = PublicityEnum.Private;
        public DateTimeOffset PublishedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? DeletedAt = null;

        public int PassersCount { get; set; } = 0;
        public int LikesCount { get; set; } = 0;
        public int DislikesCount { get; set; } = 0;

        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new();
        public Guid? ThumbnailUid { get; set; }
        
        
        // Content
        public List<QuestionEntity> Questions { get; set; } = new();
    }
}
