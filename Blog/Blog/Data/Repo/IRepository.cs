using Blog.Models;
using Blog.Models.Comments;
using Blog.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Data.Repo
{
    public interface IRepository
    {
        Post GetPostById(int id);
        List<Post> GetPosts();
        IndexViewModel GetPosts(int pageNumber, string category,string search);
        void AddPost(Post post);
        void RemovePost(int id);
        void UpdatePost(Post post);
        void AddSubComments(SubComment subComment);
        Task<bool> SaveChangesAsync();
    }
}
