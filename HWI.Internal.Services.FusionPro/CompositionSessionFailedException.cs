using System;

namespace HWI.Internal.Services.FusionPro
{
    public class CompositionSessionFailedException : ApplicationException
    {
        public CompositionSessionFailedException(string respMessage) : base(respMessage) { }
    }
}