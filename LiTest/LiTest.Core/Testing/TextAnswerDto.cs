namespace LiTest.Shared.Core.Testing
{
    public record TextAnswerDto(string AnswerText) : AnswerDtoAbstract
    {
        public override QuestionTypeEnum AnswerType => QuestionTypeEnum.TextAnswer;
        public override bool IsEmpty => string.IsNullOrEmpty(AnswerText);

        public TextAnswerMatchingDetails CheckMatching(AnswerDtoAbstract otherAnswer)
        {
            var metaMatching = CheckMetaMatching(otherAnswer);

            if (!metaMatching.CompleteMatch)
                return new TextAnswerMatchingDetails(false);

            var otherTextAnswer = (TextAnswerDto)otherAnswer;
            var otherAnswerText = otherTextAnswer.AnswerText;

            bool stringMatching = 
                AnswerText.ToLower()
                .Equals(otherAnswerText.ToLower());

            var details = new TextAnswerMatchingDetails(stringMatching);
            return details;
        }

        public override bool IsMatchingWith(AnswerDtoAbstract otherAnswer)
            => CheckMatching(otherAnswer).IsStringMatching;
    }

    public record TextAnswerMatchingDetails(bool IsStringMatching) : AnswerMatchingDetails;

}
