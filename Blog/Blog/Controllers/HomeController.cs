using Blog.Data;
using Blog.Data.FileManager;
using Blog.Data.Repo;
using Blog.Models;
using Blog.Models.Comments;
using Blog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blog.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepository _repo;
        private readonly IFileManager _fileManager;

        public HomeController(IRepository repo
            , IFileManager fileManager)
        {
            _repo = repo;
            _fileManager = fileManager;
        }

        public IActionResult Index(int pageNumber, string category,string search)
        {
            if (pageNumber < 1)
            {
                return RedirectToAction("Index", new { pageNumber = 1, category });
            }

            var vm = _repo.GetPosts(pageNumber,category,search);

            return View(vm);
        }

        public IActionResult Post(int id)
        {
            var post = _repo.GetPostById(id);
            return View(post);
        }

        [HttpGet("/Image/{image}")]
        [ResponseCache(CacheProfileName = "Monthly")]
        public IActionResult Image(string image)
        {
            var mime = image.Substring(image.LastIndexOf('.') + 1);
            return new FileStreamResult(_fileManager.ImageSream(image), $"image/{mime}");
        }

        public async Task<IActionResult> CommentAsync(CommentViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Post", new { id = vm.PostId });
            }

            var post = _repo.GetPostById(vm.PostId);

            if (vm.MainCommentId == 0)
            {
                post.MainComments = post.MainComments ?? new List<MainComment>();

                post.MainComments.Add(new MainComment
                {
                    Created = DateTime.Now,
                    Message = vm.Message
                });

                _repo.UpdatePost(post);
            }
            else
            {
                var subComment = new SubComment
                {
                    Message = vm.Message,
                    MainCommentId = vm.MainCommentId,
                    Created = DateTime.Now,
                };

                _repo.AddSubComments(subComment);
            }

            await _repo.SaveChangesAsync();

            return RedirectToAction("Post", new { id = vm.PostId });
        }
    }
}
