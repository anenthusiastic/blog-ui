using data.Abstract;
using entity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using ui.ViewModels;

namespace firstapp.Controllers
{
    public class HomeController:Controller
    {
        private IPostRepository _postrepository;
        private IHeaderRepository _headerrepository;
        private IPostCategoryRepository _postcategoryrepository;
        private ICategoryRepository _categoryrepository;
        private IMessageRepository _messagerepository;
        public HomeController(IPostRepository _postrepository,
            IHeaderRepository _headerrepository, 
            IPostCategoryRepository _postcategoryrepository,
            ICategoryRepository _categoryrepository,
            IMessageRepository _messagerepository){

            this._postrepository = _postrepository;
            this._categoryrepository = _categoryrepository;
            this._headerrepository = _headerrepository;
            this._messagerepository = _messagerepository;
            this._postcategoryrepository = _postcategoryrepository;
        }
        private static int sayaç = 5;
        
        public IActionResult Index(){
            sayaç = 5;
            ViewBag.header = _headerrepository.GetById(4);
            ViewBag.categories = _categoryrepository.GetAll();
            List<Post> allPosts = _postrepository.GetAll();
            int postcount = allPosts.Count;
            if(sayaç>postcount){
                sayaç = postcount;
            }
            var posts = allPosts.GetRange(0,sayaç);
            List<PostViewModel> postViewModels = new List<PostViewModel>();
            foreach (var item in posts)
            {
                List<string> categorynames = new List<string>();
                var ids = _postcategoryrepository.GetCategories(item.id);
                foreach (var id in ids)
                {
                    categorynames.Add(_categoryrepository.GetById(id).name);
                }
                PostViewModel postviewmodel = new PostViewModel(){
                    post = item,
                    categoryNames = categorynames,
                    postHeader = _headerrepository.GetById(item.headerId)
                };
                postViewModels.Add(postviewmodel);
            }
            
            ViewBag.olderPostsButton = postcount == sayaç?"disabled":"";
            return View(postViewModels);
        }

        public IActionResult About(){
            ViewBag.categories = _categoryrepository.GetAll();
            ViewBag.header = ViewBag.header = _headerrepository.GetById(2);
            return View();
        }
        public IActionResult Contact(){
            ViewBag.categories = _categoryrepository.GetAll();
            ViewBag.header = _headerrepository.GetById(3);
            return View();
        }
        public IActionResult Login(){
            ViewBag.categories = _categoryrepository.GetAll();
            return View();
        }
         public IActionResult Post(int? id,string q,string category){
             ViewBag.categories = _categoryrepository.GetAll();
            List<Post> allposts =  _postrepository.GetAll();
            if(!string.IsNullOrEmpty(q)){
                List<Post> searchPosts = allposts.Where(i => (_headerrepository.GetById(i.headerId).heading.ToLower().Contains(q))).ToList();
                List<PostViewModel> postViewModels = new List<PostViewModel>();
                foreach (var item in searchPosts)
                {
                    List<string> categorynames = new List<string>();
                    var ids = _postcategoryrepository.GetCategories(item.id);
                    foreach (var catId in ids)
                    {
                        categorynames.Add(_categoryrepository.GetById(catId).name);
                    }
                    var postvm = new PostViewModel(){
                        post= item,
                        categoryNames = categorynames,
                        postHeader = _headerrepository.GetById(item.headerId)
                    };
                    postViewModels.Add(postvm);
                }
                ViewBag.header = _headerrepository.GetById(15);
                return View("list",postViewModels);
            }

            if(!string.IsNullOrEmpty(category)){
                var catId= _categoryrepository.GetAll().FirstOrDefault(c => (c.name.Equals(category))).id;
                var searchPostIds = _postcategoryrepository.GetPosts(catId);
                List<PostViewModel> postViewModels = new List<PostViewModel>();
                foreach (var postId in searchPostIds)
                {
                    List<string> categorynames = new List<string>();
                    var ids = _postcategoryrepository.GetCategories(postId);
                    foreach (var cId in ids)
                    {
                        categorynames.Add(_categoryrepository.GetById(cId).name);
                    }
                    var postvm = new PostViewModel(){
                        post= _postrepository.GetById(postId),
                        categoryNames = categorynames,
                        postHeader = _headerrepository.GetById(_postrepository.GetById(postId).headerId)
                    };
                    postViewModels.Add(postvm);
                }
                ViewBag.header = _headerrepository.GetAll().FirstOrDefault(c => (c.heading.Equals(category)));
                return View("list",postViewModels);
            }

            PostViewModel pvm ;
            if(id!= null){
                Post p = allposts.FirstOrDefault(p=>p.id == id);

                pvm = new PostViewModel(){
                    post = p
                }; 
                ViewBag.header = _headerrepository.GetById(p.headerId);
                p.viewCount++;
                _postrepository.Update(p);
                
            }

            else{
                Random rastgele = new Random();
                int sayi = rastgele.Next(allposts.Count);
                Post p = allposts[sayi];
                pvm = new PostViewModel(){
                    post = p
                };
                ViewBag.header = _headerrepository.GetById(p.headerId);
                p.viewCount++;
                _postrepository.Update(p);
            }
            return View(pvm);
        }
        public IActionResult olderPosts(){
            ViewBag.categories = _categoryrepository.GetAll();
            ViewBag.header = ViewBag.header = _headerrepository.GetById(4);
            sayaç+=5;
            var allposts =_postrepository.GetAll();
            int postcount = allposts.Count();
            if(sayaç>postcount){
                sayaç = postcount;
            }

            List<PostViewModel> postViewModels = new List<PostViewModel>();
            var posts = allposts.GetRange(0,sayaç);
            foreach (var item in posts)
            {
                List<string> categorynames = new List<string>();
                var ids = _postcategoryrepository.GetCategories(item.id);
                foreach (var catId in ids)
                {
                    categorynames.Add(_categoryrepository.GetById(catId).name);
                }
                PostViewModel postviewmodel = new PostViewModel(){
                    post = item,
                    categoryNames = categorynames,
                    postHeader = _headerrepository.GetById(item.headerId)
                };
                postViewModels.Add(postviewmodel);
            }
            

            ViewBag.olderPostsButton = postcount == sayaç?"disabled":"";
            return View("index",postViewModels);
        }

        public IActionResult Delivered(Message m){
            ViewBag.categories = _categoryrepository.GetAll();
            ViewBag.header = _headerrepository.GetById(3);
            _messagerepository.Create(m);
            return View();
        }
    }
}