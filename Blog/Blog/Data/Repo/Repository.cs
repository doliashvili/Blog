using Blog.Extensions.Expressions;
using Blog.Helpers;
using Blog.Models;
using Blog.Models.Comments;
using Blog.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Blog.Data.Repo
{
    public class Repository : IRepository
    {
        private readonly Data.AppDbContext _dbContext;

        public Repository(Data.AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void AddPost(Post post)
        {
            _dbContext.Posts.Add(post);
        }

        public void AddSubComments(SubComment subComment)
        {
            _dbContext.SubComments.Add(subComment);
        }

        public Post GetPostById(int id)
        {
            return _dbContext.Posts
                .Include(x => x.MainComments)
                .ThenInclude(x => x.SubComments)
                .FirstOrDefault(o => o.Id == id);
        }

        public List<Post> GetPosts()
        {
            return _dbContext
                .Posts
                .ToList();
        }

        public IndexViewModel GetPosts(int pageNumber, string category, string search)
        {
            Expression<Func<Post, bool>> predicate = default;



            if (!string.IsNullOrWhiteSpace(category))
            {
                Expression<Func<Post, bool>> inCategory = post => EF.Functions.Like(post.Category,$"%{category}%");
                predicate = inCategory;
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                Expression<Func<Post, bool>> inSearch = post =>
                   EF.Functions.Like(post.Body, $"%{search}%")
                 || EF.Functions.Like(post.Title, $"%{search}%")
                 || EF.Functions.Like(post.Description, $"%{search}%");

                if (predicate != null)
                {
                    predicate = predicate.And(inSearch);
                }
                else
                {
                    predicate = inSearch;
                }
            }


            int pageSize = 1;
            int skipAmount = pageSize * (pageNumber - 1);
            int capacity = skipAmount + pageSize;

            int postsCount;
            IQueryable<Post> query;
            if (predicate != null)
            {
                query = _dbContext.Posts.AsNoTracking().Where(predicate);
                postsCount = _dbContext.Posts.Count(predicate);
            }
            else
            {
                query = _dbContext.Posts.AsNoTracking();
                postsCount = _dbContext.Posts.Count();
            }


            bool canGoNext = postsCount > capacity;
            var pageCount = (int)Math.Ceiling((double)postsCount / pageSize);

            var vm = new IndexViewModel
            {
                PageNumber = pageNumber,
                CanGoNext = canGoNext,
                Category = category,
                PageCount = pageCount,
                Pages = PageHelper.GetPages(pageNumber, pageCount).ToList(),

                Posts = query
                .Skip(skipAmount)
                .Take(pageSize)
                .ToList()
            };

            return vm;
        }

        public void RemovePost(int id)
        {
            var entity = GetPostById(id);
            _dbContext.Posts.Remove(entity);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public void UpdatePost(Post post)
        {
            _dbContext.Posts.Update(post);
        }
    }
}
