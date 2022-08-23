using Theoremone.SmartAc.Application.Common.Interfaces;

namespace Theoremone.SmartAc.Infrastructure.Services
{
    public class DateTimeService : IDateTime
    {
        public DateTimeOffset Now => DateTimeOffset.Now;
    }
}
