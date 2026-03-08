using System.Text.Json.Serialization;

namespace LiTest.Shared.Core.Testing
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "$answerKind")]
    [JsonDerivedType(typeof(OneAnswerDto), "one")]
    [JsonDerivedType(typeof(MultiAnswerDto), "multi")]
    [JsonDerivedType(typeof(TextAnswerDto), "text")]
    public abstract record class AnswerDtoAbstract
    {
        public abstract QuestionTypeEnum AnswerType { get; }
        public abstract bool IsEmpty { get; }
        public int QuestionId { get; }
        public Guid TestUid { get; }

        public AnswerMetaMatchingDetails CheckMetaMatching(AnswerDtoAbstract otherAnswer)
        {
            bool notNull = otherAnswer != null;
            if (!notNull)
                return new AnswerMetaMatchingDetails(false, false, false, false, false);

            bool answerTypeMatch = AnswerType == otherAnswer!.AnswerType;
            bool testUidMatch = TestUid == otherAnswer.TestUid;
            bool questionUidMatch = QuestionId == otherAnswer.QuestionId;

            var details = new AnswerMetaMatchingDetails(
                notNull,
                answerTypeMatch,
                testUidMatch,
                questionUidMatch,
                answerTypeMatch && testUidMatch && questionUidMatch);

            return details;
        }

        public virtual bool IsMatchingWith(AnswerDtoAbstract otherAnswer)
            => CheckMetaMatching(otherAnswer).CompleteMatch;
    }
    public record AnswerMetaMatchingDetails(bool NotNull, bool AnswerTypeMatch, bool TestUidMatch, bool QuestionIdMatch, bool CompleteMatch) : AnswerMatchingDetails;

    public abstract record AnswerMatchingDetails;

}
