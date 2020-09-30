using Blog.Data.FileManager;
using Blog.Data.Repo;
using Blog.Models;
using Blog.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PanelController : Controller
    {
        private readonly IRepository _repo;
        private readonly IFileManager _fileManager;

        public PanelController(IRepository repo,
            IFileManager fileManager
            )
        {
            _repo = repo;
            _fileManager = fileManager;
        }

        public IActionResult Index()
        {
            var posts = _repo.GetPosts();
            return View(posts);
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return View(new PostViewModel());
            }
            else
            {
                var post = _repo.GetPostById((int)id);
                var vm = new PostViewModel
                {
                    Title = post.Title,
                    Body = post.Body,
                    Id = post.Id,
                    Category = post.Category,
                    CurrentImage = post.Image,
                    Tags = post.Tags,
                    Description = post.Description
                };
                return View(vm);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(PostViewModel vm)
        {
            var post = new Post
            {
                Title = vm.Title,
                Body = vm.Body,
                Id = vm.Id,
                Category=vm.Category,
                Tags=vm.Tags,
                Description=vm.Description
            };

            if (vm.Image==null)
            {
                post.Image = vm.CurrentImage;
            }
            else
            {
                if (!string.IsNullOrEmpty(vm.CurrentImage))
                {
                    _fileManager.RemoveImage(vm.CurrentImage);
                }
                post.Image = await _fileManager.SaveImage(vm.Image);
            }

            if (post.Id == 0)
            {
                _repo.AddPost(post);
                await _repo.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            _repo.UpdatePost(post);
            await _repo.SaveChangesAsync();
            return RedirectToAction("Index");

        }

        [HttpGet]
        public async Task<IActionResult> RemoveAsync(int id)
        {
            _repo.RemovePost(id);
            await _repo.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
