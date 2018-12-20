using System;

namespace HWI.Internal.Services.FusionPro
{
    public class CompositionStatusFailedException : ApplicationException
    {
        public CompositionStatusFailedException(int respCompositionId, string statusMessage)
            : base($"Composition {respCompositionId} status check failed: {statusMessage}") { }
    }
}