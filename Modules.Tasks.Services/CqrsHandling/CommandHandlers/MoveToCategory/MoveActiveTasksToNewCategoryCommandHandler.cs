using MediatR;
using Modules.Categories.Contracts;
using Modules.Tasks.Contracts;
using Modules.Tasks.Contracts.Cqrs.Commands;
using Modules.Tasks.Contracts.Models;
using Prism.Events;

namespace Modules.Tasks.Services.CqrsHandling.CommandHandlers.MoveToCategory;

public class MoveActiveTasksToNewCategoryCommandHandler :
    BaseMoveTaskToNewCategoryCommandHandler, IRequestHandler<MoveActiveTasksToNewCategoryCommand>
{
    public MoveActiveTasksToNewCategoryCommandHandler(
        ITaskItemRepository taskItemRepository,
        ICategoriesRepository categoriesRepository,
        IEventAggregator eventAggregator,
        IMediator mediator)
    : base(taskItemRepository, categoriesRepository, eventAggregator, mediator)
    {
    }

    protected override List<TaskItem> GetOldCategoryTasks(int oldCategoryId)
    {
        var oldCategoryTasks = TaskItemRepository.GetActiveTasksFromCategory(oldCategoryId);
        ArgumentNullException.ThrowIfNull(oldCategoryTasks);

        return oldCategoryTasks;
    }

    protected override void InsertTasksToNewCategory(
        int newCategoryId,
        List<TaskItem> oldCategoryTasks,
        List<TaskItem> newCategoryTasks,
        CancellationToken cancellationToken)
    {
        var oldPinnedTasks = oldCategoryTasks.Where(x => x.Pinned).ToList();
        var oldNonPinnedTasks = oldCategoryTasks.Where(x => !x.Pinned).ToList();

        // Move pinned tasks to category
        InsertTasksToList(
            sourceList: oldPinnedTasks,
            destinationList: newCategoryTasks,
            destinationCategoryId: newCategoryId,
            cancellationToken);

        TaskItemRepository.MoveTasksToCategory(oldPinnedTasks, newCategoryId);

        // Move non-pinned tasks to category
        InsertTasksToList(
            sourceList: oldNonPinnedTasks,
            destinationList: newCategoryTasks,
            destinationCategoryId: newCategoryId,
            cancellationToken);

        TaskItemRepository.MoveTasksToCategory(oldNonPinnedTasks, newCategoryId);
    }

    public Task Handle(MoveActiveTasksToNewCategoryCommand request, CancellationToken cancellationToken)
    {
        return HandleInternal(request.OldCategoryId, request.NewCategoryId, cancellationToken);
    }
}
