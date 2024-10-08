﻿using Modules.Categories.Contracts;
using Modules.Categories.Contracts.Models;

namespace Modules.Categories.Repositories;

public class CategoriesDbInfoRepository : ICategoriesDbInfoRepository
{
    private readonly CategoryDbContext _context;

    public CategoriesDbInfoRepository(CategoryDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    public bool Initialized => _context.CategoriesDbInfo.Any();

    public void Initialize()
    {
        if (!_context.CategoriesDbInfo.Any())
        {
            _context.CategoriesDbInfo.Add(new CategoriesDbInfo { Initialized = true });
            _context.SaveChanges();
        }
    }
}
