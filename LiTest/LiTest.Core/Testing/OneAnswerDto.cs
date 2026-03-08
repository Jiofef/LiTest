namespace LiTest.Shared.Core.Testing
{
    public record OneAnswerDto(int OptionId) : AnswerDtoAbstract
    {
        public override QuestionTypeEnum AnswerType => QuestionTypeEnum.OneAnswer;
        public override bool IsEmpty => OptionId == -1;

        public OneAnswerMatchingDetails CheckMatching(AnswerDtoAbstract otherAnswer)
        {
            var metaMatching = CheckMetaMatching(otherAnswer);

            if (!metaMatching.CompleteMatch)
                return new OneAnswerMatchingDetails(false);

            var otherOneAnswer = (OneAnswerDto)otherAnswer;
            var selectedOptionMatch = OptionId == otherOneAnswer.OptionId;

            var details = new OneAnswerMatchingDetails(selectedOptionMatch);
            return details;
        }

        public override bool IsMatchingWith(AnswerDtoAbstract otherAnswer)
            => CheckMatching(otherAnswer).SelectedOptionMatch;
    }
    public record OneAnswerMatchingDetails(bool SelectedOptionMatch) : AnswerMatchingDetails;

}
