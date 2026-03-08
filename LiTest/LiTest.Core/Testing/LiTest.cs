using LiTest.Shared.Core.Community;

namespace LiTest.Shared.Core.Testing
{
    public record LiTestDto(Guid AuthorId, PublicityEnum Publicity, string Name, string Description, Guid ThumbnailUid, List<QuestionDto> Questions);
}
