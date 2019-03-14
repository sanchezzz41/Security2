using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Security2.Database;
using Security2.Database.Entities;
using Security2.Dto.Models;

namespace Security2.Domain.Services
{
    public class NewsService
    {
        private readonly DatabaseContext _context;

        public NewsService(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<Guid> Create(NewsInfo model)
        {
            var result = new News(model.Title, model.Content);
            _context.News.Add(result);
            await _context.SaveChangesAsync();
            return result.Id;
        }

        public async Task<List<NewsModel>> Get()
        {
            return await _context.News.AsNoTracking()
                .Select(x => new NewsModel(x.Id, x.Title, x.Content, x.CreatedDate))
                .ToListAsync();
        }
    }
}
