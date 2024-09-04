using MediatR;
using Modules.Categories.Contracts;
using Modules.Tasks.Contracts;
using Modules.Tasks.Contracts.Cqrs.Commands;
using Modules.Tasks.Contracts.Models;
using Prism.Events;

namespace Modules.Tasks.Views.CqrsHandling.CommandHandlers;

public class MoveIncompleteTasksToNewCategoryCommandHandler
    : BaseMoveTaskToNewCategoryCommandHandler, IRequestHandler<MoveIncompleteTasksToNewCategoryCommand>
{
    public MoveIncompleteTasksToNewCategoryCommandHandler(
        ITaskItemRepository taskItemRepository,
        ICategoriesRepository categoriesRepository,
        IEventAggregator eventAggregator,
        IMediator mediator)
        : base(taskItemRepository, categoriesRepository, eventAggregator, mediator)
    {
    }

    public Task Handle(MoveIncompleteTasksToNewCategoryCommand request, CancellationToken cancellationToken)
    {
        return HandleInternal(request.OldCategoryId, request.NewCategoryId, cancellationToken);
    }

    protected override List<TaskItem> GetOldCategoryTasks(int oldCategoryId)
    {
        var oldCategoryTasks = TaskItemRepository.GetActiveTasksFromCategory(oldCategoryId);
        ArgumentNullException.ThrowIfNull(oldCategoryTasks);

        oldCategoryTasks = oldCategoryTasks.Where(x => !x.IsDone).ToList();
        return oldCategoryTasks;
    }
}
