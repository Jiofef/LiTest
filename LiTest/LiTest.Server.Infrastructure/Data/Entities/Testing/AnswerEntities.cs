using LiTest.Shared.Core.Testing;

namespace LiTest.Server.Infrastructure.Testing
{
    public interface AnswerEntity
    {
        public QuestionTypeEnum AnswerType { get; }
        public bool IsEmpty { get; }
    }
    public class OneAnswerEntity : AnswerEntity
    {
        public QuestionTypeEnum AnswerType => QuestionTypeEnum.OneAnswer;

        public int OptionId = -1;
        public bool IsEmpty => OptionId == -1;

    }
    public class MultiAnswerEntity : AnswerEntity
    {
        public QuestionTypeEnum AnswerType => QuestionTypeEnum.MultiAnswer;

        public List<int> OptionIds = new();
        public bool IsEmpty => OptionIds.Count == 0;
    }
    public class TextAnswerEntity : AnswerEntity
    {
        public QuestionTypeEnum AnswerType => QuestionTypeEnum.TextAnswer;

        public string Answer = string.Empty;
        public bool IsEmpty => string.IsNullOrEmpty(Answer);
    }
}
