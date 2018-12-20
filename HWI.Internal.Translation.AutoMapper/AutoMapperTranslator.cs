using AutoMapper;

namespace HWI.Internal.Translation.AutoMapper
{
    public class AutoMapperTranslator<TIn, TOut> : IObjectTranslator<TIn, TOut> where TOut : new()
    {
        public TOut Translate(TIn input)
        {
            return Mapper.Map<TIn, TOut>(input);
        }

        public TOut Translate(TIn input, TOut existing)
        {
            return Mapper.Map<TIn, TOut>(input, existing);
        }
    }
}