using LiTest.Shared.Core.Testing;

namespace LiTest.Server.Infrastructure.Testing
{
    public abstract class AnswerEntityAbstract
    {
        public abstract QuestionTypeEnum AnswerType { get; }
        public abstract bool IsEmpty { get; }
    }
    public class OneAnswerEntity : AnswerEntityAbstract
    {
        public override QuestionTypeEnum AnswerType => QuestionTypeEnum.OneAnswer;

        public int OptionId = -1;
        public override bool IsEmpty => OptionId == -1;

    }
    public class MultiAnswerEntity : AnswerEntityAbstract
    {
        public override QuestionTypeEnum AnswerType => QuestionTypeEnum.MultiAnswer;

        public List<int> OptionIds = new();
        public override bool IsEmpty => OptionIds.Count == 0;
    }
    public class TextAnswerEntity : AnswerEntityAbstract
    {
        public override QuestionTypeEnum AnswerType => QuestionTypeEnum.TextAnswer;

        public string Answer = string.Empty;
        public override bool IsEmpty => string.IsNullOrEmpty(Answer);
    }
}
