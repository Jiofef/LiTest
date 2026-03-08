using System.Text.Json.Serialization;

namespace LiTest.Shared.Core.Testing
{
    public enum QuestionTypeEnum { OneAnswer, MultiAnswer, TextAnswer }
    public record QuestionDto(QuestionTypeEnum QuestionType, string? Text, List<Guid> ImageUids, IQuestionContentDto Content, AnswerDtos CorrectAnswer);

    [JsonPolymorphic(TypeDiscriminatorPropertyName = "$contentKind")]
    [JsonDerivedType(typeof(QuestionOptionsContentDto), "options")]
    [JsonDerivedType(typeof(QuestionTextContentDto), "text")]
    public interface IQuestionContentDto { }
    public record QuestionOptionsContentDto(List<QuestionOptionDto> Options) : IQuestionContentDto;
    public record QuestionOptionDto(int Id, string Text);
    public record QuestionTextContentDto(string DefaultText) : IQuestionContentDto;
}
