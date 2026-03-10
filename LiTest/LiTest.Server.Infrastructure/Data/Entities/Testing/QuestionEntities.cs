using LiTest.Server.Infrastructure.Testing;

namespace LiTest.Shared.Core.Testing
{
    public class QuestionEntity
    {
        public QuestionTypeEnum QuestionType;
        public string? Text;
        public List<Guid> ImageUids = new();
        public QuestionContentEntityAbstract Content = new QuestionOptionsContentEntity();
        public AnswerEntityAbstract CorrectAnswer = new OneAnswerEntity();
    }
    public abstract class QuestionContentEntityAbstract { }
    public class QuestionOptionsContentEntity : QuestionContentEntityAbstract
    {
        public List<QuestionOptionEntity> Options = new();
    }
    public class QuestionOptionEntity
    {
        public string Title = "Option";
        public int Id;
    }
    public class QuestionTextContentEntity : QuestionContentEntityAbstract
    {
        public string DefaultText = "Your answer...";
    }
}
