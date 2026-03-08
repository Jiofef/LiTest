namespace LiTest.Shared.Core.Testing
{
    public record MultiAnswerDto(IReadOnlyList<int> OptionIds) : AnswerDtoAbstract
    {
        public override QuestionTypeEnum AnswerType => QuestionTypeEnum.MultiAnswer;
        public override bool IsEmpty => OptionIds.Count == 0;

        public MultiAnswerMatchingDetails CheckMatching(AnswerDtoAbstract otherAnswer)
        {
            var metaMatching = CheckMetaMatching(otherAnswer);

            if (!metaMatching.CompleteMatch)
                return new MultiAnswerMatchingDetails(false, new List<int>(), new List<int>(), new List<int>());

            var otherMultiAnswer = (MultiAnswerDto)otherAnswer;
            List<int> matchingOptions = new(), unmatchingOptions = new(), missedOptions = new();
            bool isAllMatching = true;

            var otherOptionsIds = otherMultiAnswer.OptionIds;

            int selectedCount1 = OptionIds.Count;
            int selectedCount2 = otherOptionsIds.Count;

            for (int i = 0; i < selectedCount1; i++)
            {
                var value = OptionIds[i];
                if (otherOptionsIds.Contains(value))
                {
                    matchingOptions.Add(value);
                }
                else
                {
                    unmatchingOptions.Add(value);
                    isAllMatching = false;
                }
            }

            for (int i = 0; i < selectedCount2; i++)
            {
                var value = otherOptionsIds[i];
                {
                    if (matchingOptions.Contains(value))
                        continue;
                    else
                    {
                        missedOptions.Add(value);
                        isAllMatching = false;
                    }
                }
            }

            var details = new MultiAnswerMatchingDetails(isAllMatching, matchingOptions, unmatchingOptions, missedOptions);
            return details;
        }

        public override bool IsMatchingWith(AnswerDtoAbstract otherAnswer)
            => CheckMatching(otherAnswer).IsAllMatching;
    }
    public record MultiAnswerMatchingDetails(bool IsAllMatching, IReadOnlyList<int> MatchingOptions, IReadOnlyList<int> UnmatchingOptions, IReadOnlyList<int> MissedOptions) : AnswerMatchingDetails;

}
