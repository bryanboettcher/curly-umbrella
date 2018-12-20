using System;

namespace HWI.Internal.Services.FusionPro
{
    public class CompositionFailedException : ApplicationException
    {
        public CompositionFailedException(int respCompositionId, string addRespMessage)
            : base($"Composition {respCompositionId} failed : {addRespMessage}") { }
    }
}