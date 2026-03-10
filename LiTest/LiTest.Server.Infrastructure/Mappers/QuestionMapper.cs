using LiTest.Server.Infrastructure.Testing;
using LiTest.Shared.Core.Testing;
using Riok.Mapperly.Abstractions;

namespace LiTest.Server.Infrastructure.Mappers
{
    [Mapper]
    public partial class QuestionMapper
    {
        public partial QuestionDto EntityToDto(QuestionEntity entity);
        public partial QuestionEntity DtoToEntity(QuestionDto dto);

        // --- CONTENT MAPPING (IQuestionContent) ---

        protected IQuestionContentDto MapContentToDto(QuestionContentEntityAbstract entity) => entity switch
        {
            QuestionOptionsContentEntity e => MapOptionsToDto(e),
            QuestionTextContentEntity e => MapTextToDto(e),
            _ => throw new NotSupportedException($"Unknown content entity type: {entity.GetType()}")
        };

        protected QuestionContentEntityAbstract MapContentToEntity(IQuestionContentDto dto) => dto switch
        {
            QuestionOptionsContentDto d => MapOptionsToEntity(d),
            QuestionTextContentDto d => MapTextToEntity(d),
            _ => throw new NotSupportedException($"Unknown content dto type: {dto.GetType()}")
        };

        // --- ANSWER MAPPING (Answer) ---

        protected AnswerDtos MapAnswerToDto(AnswerEntityAbstract entity) => entity switch
        {
            OneAnswerEntity e => MapOneToDto(e),
            MultiAnswerEntity e => MapMultiToDto(e),
            TextAnswerEntity e => MapTextAnswerToDto(e),
            _ => throw new NotSupportedException($"Unknown answer entity type: {entity.GetType()}")
        };

        protected AnswerEntityAbstract MapAnswerToEntity(AnswerDtos dto) => dto switch
        {
            OneAnswerDto d => MapOneToEntity(d),
            MultiAnswerDto d => MapMultiToEntity(d),
            TextAnswerDto d => MapTextAnswerToEntity(d),
            _ => throw new NotSupportedException($"Unknown answer dto type: {dto.GetType()}")
        };

        // --- HELPER METHODS ---

        private partial QuestionOptionsContentDto MapOptionsToDto(QuestionOptionsContentEntity entity);
        private partial QuestionOptionsContentEntity MapOptionsToEntity(QuestionOptionsContentDto dto);

        private partial QuestionTextContentDto MapTextToDto(QuestionTextContentEntity entity);
        private partial QuestionTextContentEntity MapTextToEntity(QuestionTextContentDto dto);

        private partial OneAnswerDto MapOneToDto(OneAnswerEntity entity);
        private partial OneAnswerEntity MapOneToEntity(OneAnswerDto dto);

        private partial MultiAnswerDto MapMultiToDto(MultiAnswerEntity entity);
        private partial MultiAnswerEntity MapMultiToEntity(MultiAnswerDto dto);

        private partial TextAnswerDto MapTextAnswerToDto(TextAnswerEntity entity);
        private partial TextAnswerEntity MapTextAnswerToEntity(TextAnswerDto dto);

        // --- OPTION MAPPING ---
        private partial QuestionOptionDto MapOptionToDto(QuestionOptionEntity entity);
        private partial QuestionOptionEntity MapOptionToEntity(QuestionOptionDto dto);
    }
}