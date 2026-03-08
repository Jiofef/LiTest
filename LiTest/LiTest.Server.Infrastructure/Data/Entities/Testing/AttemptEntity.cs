using LiTest.Shared.Core.Testing;
using System;
using System.Collections.Generic;
using System.Text;

namespace LiTest.Server.Infrastructure.Data.Entities.Testing
{
    public class AttemptEntity
    {
        public Guid UserUid { get; }
        public Guid TestUid { get; }
        public List<QuestionStatus> QuestionStatuses { get; } = new();
        public int CurrentQuestionIndex { get; set; } = 0;
    }

    public record QuestionStatus(int QuestionId, AnswerDtoAbstract Answer, AnswerMatchingDetails AnswerCorrectnessDetails);
}
