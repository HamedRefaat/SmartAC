using Theoremone.SmartAc.Domain.Entities;

namespace Theoremone.SmartAc.Application.Common.Interfaces.Repository
{
  public  interface IAlertRepo : IReadableAsyncRepository<Alert, int>, IUpdatableAsyncRepository<Alert>
    {
        
    }
}
