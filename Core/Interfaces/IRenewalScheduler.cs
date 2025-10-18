using Core.Models;

namespace Core.Interfaces
{
    public interface IRenewalScheduler
    {
        Task ScheduleRenewalAsync(Domain domain);
        Task RunManualRenewalAsync();
        event EventHandler<RenewalLog> RenewalCompleted;
    }
}
