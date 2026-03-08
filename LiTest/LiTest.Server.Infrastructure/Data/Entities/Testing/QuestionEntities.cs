using LiTest.Server.Infrastructure.Testing;

namespace LiTest.Shared.Core.Testing
{
    public class QuestionEntity
    {
        public QuestionTypeEnum QuestionType;
        public string? Text;
        public List<Guid> ImageUids = new();
        public IQuestionContentEntity Content = new QuestionOptionsContentEntity();
        public AnswerEntity CorrectAnswer = new OneAnswerEntity();
    }
    public interface IQuestionContentEntity { }
    public class QuestionOptionsContentEntity : IQuestionContentEntity
    {
        public List<QuestionOptionEntity> Options = new();
    }
    public class QuestionOptionEntity
    {
        public string Title = "Option";
        public int Id;
    }
    public class QuestionTextContentEntity : IQuestionContentEntity
    {
        public string DefaultText = "Your answer...";
    }
}
