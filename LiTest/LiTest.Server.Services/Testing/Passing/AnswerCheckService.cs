using LiTest.Shared.Core.Testing;

namespace LiTest.Server.Services.Testing.Passing
{
    public class AnswerCheckService
    {
        public bool IsAnswerCorrect(AnswerDtoAbstract answer, QuestionDto question)
        {
            var qType = question.QuestionType;

            var correctAnswer = question.CorrectAnswer;
            return answer.IsMatchingWith(correctAnswer);
        }

        public AnswerMatchingDetails GetAnswerCorrectnessDetails(AnswerDtoAbstract answer, QuestionDto question)
        {
            var qType = question.QuestionType;

            if (qType != answer.AnswerType)
                throw new InvalidOperationException("The answer and question types do not match");

            var correctAnswer = question.CorrectAnswer;

            switch (answer)
            {
                case OneAnswerDto oneA:
                    return oneA.CheckMatching(correctAnswer);
                case MultiAnswerDto multiA:
                    return multiA.CheckMatching(correctAnswer);
                case TextAnswerDto textA:
                    return textA.CheckMatching(correctAnswer);
                default:
                    throw new InvalidOperationException("An unknown answer type was used");
            }    
        }
    }
}
