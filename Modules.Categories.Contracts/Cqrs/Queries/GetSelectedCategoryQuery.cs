using MediatR;
using Modules.Categories.Contracts.Models;

namespace Modules.Categories.Contracts.Cqrs.Queries;

public class GetSelectedCategoryQuery : IRequest<CategoryInfo>;