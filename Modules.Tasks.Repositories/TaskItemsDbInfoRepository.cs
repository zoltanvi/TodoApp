using Modules.Tasks.Contracts;
using Modules.Tasks.Contracts.Models;

namespace Modules.Tasks.Repositories;

public class TaskItemsDbInfoRepository : ITaskItemsDbInfoRepository
{
    private readonly TaskItemDbContext _context;

    public TaskItemsDbInfoRepository(TaskItemDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        _context = context;
    }

    public bool Initialized => _context.TasksDbInfo.Any();

    public void Initialize()
    {
        if (!_context.TasksDbInfo.Any())
        {
            _context.TasksDbInfo.Add(new TaskItemsDbInfo { Initialized = true });
            _context.SaveChanges();
        }
    }
}