using System;

namespace HWI.Internal.Services.FusionPro
{
    public class CompositionJobFailedException : ApplicationException
    {
        public CompositionJobFailedException(int respCompositionId, string jobStatus, string statusMessage)
            : base($"Composition {respCompositionId} status: {jobStatus} ({statusMessage})")
        {
        }
    }
}
