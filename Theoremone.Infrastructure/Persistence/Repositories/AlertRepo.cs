using Theoremone.SmartAc.Application.Common.Interfaces.Repository;
using Theoremone.SmartAc.Domain.Entities;
using Theoremone.SmartAc.Infrastructure.Persistence.Repositories.Base;

namespace Theoremone.SmartAc.Infrastructure.Persistence.Repositories
{
    public class AlertRepo : BaseRepository<Alert,int>, IAlertRepo
    {
        public AlertRepo(SmartAcContext context) : base(context)
        {
        }
    }
}
