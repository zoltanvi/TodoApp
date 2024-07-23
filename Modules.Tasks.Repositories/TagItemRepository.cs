using Microsoft.EntityFrameworkCore;
using Modules.Tasks.Contracts;
using Modules.Tasks.Contracts.Models;

namespace Modules.Tasks.Repositories;

public class TagItemRepository : ITagItemRepository
{
    private readonly TaskItemDbContext _context;

    public TagItemRepository(TaskItemDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        _context = context;
    }

    public TagItem AddTag(TagItem tag)
    {
        _context.Tags.Add(tag);
        _context.SaveChanges();

        return tag;
    }

    public List<TagItem> GetTags()
    {
        return _context.Tags.ToList();
    }

    public TagItem? GetTagById(int id)
    {
        return _context.Tags.Include(x => x.TaskItems).FirstOrDefault(x => x.Id == id);
    }

    public TagItem? GetTagByName(string name)
    {
        return _context.Tags.FirstOrDefault(x => x.Name.Equals(name));
    }

    public void DeleteTag(TagItem tag)
    {
        _context.Tags.Remove(tag);
        _context.SaveChanges();
    }

    public TagItem UpdateTag(TagItem tagItem)
    {
        var dbTag = _context.Tags.Find(tagItem.Id);
        ArgumentNullException.ThrowIfNull(dbTag);

        dbTag.Name = tagItem.Name;
        dbTag.Color = tagItem.Color;

        _context.SaveChanges();

        return dbTag;
    }
}
