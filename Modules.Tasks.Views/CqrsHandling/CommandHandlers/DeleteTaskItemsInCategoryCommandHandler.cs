using MediatR;
using Modules.Tasks.Contracts;
using Modules.Tasks.Contracts.Cqrs.Commands;

namespace Modules.Tasks.Views.CqrsHandling.CommandHandlers;

public class DeleteTaskItemsInCategoryCommandHandler : IRequestHandler<DeleteTaskItemsInCategoryCommand>
{
    private readonly ITaskItemRepository _taskItemRepository;

    public DeleteTaskItemsInCategoryCommandHandler(ITaskItemRepository taskItemRepository)
    {
        ArgumentNullException.ThrowIfNull(taskItemRepository);
        _taskItemRepository = taskItemRepository;
    }

    public Task Handle(DeleteTaskItemsInCategoryCommand request, CancellationToken cancellationToken)
    {
        _taskItemRepository.DeleteTasksInCategory(request.CategoryId);

        return Task.CompletedTask;
    }
}
