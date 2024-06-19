using Microsoft.EntityFrameworkCore;

namespace Modules.Migration
{
    public interface IMigrationService
    {
        void Run(IEnumerable<DbContext> contexts);
    }
}
