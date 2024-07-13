using Microsoft.EntityFrameworkCore;
using Modules.Tasks.Contracts.Models;
using Modules.Tasks.Repositories;

namespace Modules.RecycleBin.Repositories;

public class RecycleBinRepository
{
    private readonly TaskItemDbContext _context;

    public RecycleBinRepository(TaskItemDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    public List<TaskItem> GetDeletedTasksFromCategory(int categoryId)
    {
        return _context.Tasks
            .Where(x => x.CategoryId == categoryId)
            .Where(x => x.IsDeleted)
            .Include(x => x.Reminders)
            .ToList();
    }

    public List<IGrouping<int, TaskItem>> GetDeletedTasksGroupByCategory()
    {
        return _context.Tasks
            .Where(x => x.IsDeleted)
            .Include(x => x.Reminders)
            .GroupBy(x => x.CategoryId)
            .ToList();
    }

    public void RestoreTaskItem(TaskItem taskItem, int newListOrder = 0)
    {
        var dbTask = _context.Tasks.Find(taskItem.Id);
        ArgumentNullException.ThrowIfNull(dbTask);

        dbTask.IsDeleted = false;
        dbTask.DeletedDate = null;
        dbTask.ListOrder = newListOrder;

        _context.SaveChanges();
    }

}
