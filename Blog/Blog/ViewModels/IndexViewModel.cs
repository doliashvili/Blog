using Blog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.ViewModels
{
    public class IndexViewModel
    {
        public int PageNumber { get; set; }
        public int PageCount { get; set; }
        public IEnumerable<Post> Posts { get; set; }
        public bool CanGoNext { get; set; }
        public string Search { get; set; }
        public string Category { get; set; }
        public List<int> Pages { get; internal set; }
    }
}
