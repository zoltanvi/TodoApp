using Modules.Tasks.Contracts.Models;

namespace Modules.Tasks.Contracts;

public interface ITagItemRepository
{
    TagItem AddTag(TagItem tag);
    List<TagItem> GetTags();
    TagItem? GetTagById(int id);
    TagItem? GetTagByName(string name);
    void DeleteTag(TagItem tag);
}