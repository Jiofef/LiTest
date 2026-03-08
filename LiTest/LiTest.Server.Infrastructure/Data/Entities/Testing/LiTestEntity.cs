using LiTest.Shared.Core.Community;
using LiTest.Shared.Core.Testing;

namespace LiTest.Server.Infrastructure.Testing
{
    public class LiTestEntity
    {
        // Metadata
        public Guid AuthorId;
        public PublicityEnum Publicity = PublicityEnum.Private;

        // Content
        public string Name = string.Empty;
        public string Description = string.Empty;
        public Guid ThumbnailUid;
        public List<QuestionEntity> Questions = new();
    }
}
