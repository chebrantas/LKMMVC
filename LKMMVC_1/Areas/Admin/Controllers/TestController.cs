using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LKMMVC_1.Models;
using LKMMVC_1.Areas.Admin.ViewModel;


namespace LKMMVC_1.Areas.Admin.Controllers
{
    public class TestController : Controller
    {
        private CommonContext db = new CommonContext();

        // GET: Admin/Test
        public ActionResult Index()
        {
            return View(db.News.ToList());
        }



        // GET: Admin/News/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            News news = db.News.Find(id);

            List<NewsPhotoDetail> newsPhotosList = new List<NewsPhotoDetail>();
            newsPhotosList = db.NewsPhotoDetails.Where(np => np.NewsID == id).ToList();

            if (news == null)
            {
                return HttpNotFound();
            }
            //  var newsPhotos = db.NewsPhotoDetails.Where(ph => ph.NewsID == id).ToList();


            NewsViewModel newsViewModel = new NewsViewModel()
            {
                NewsID = news.NewsID,
                PostDate = news.PostDate,
                Content = HttpUtility.HtmlDecode(news.Content),
                Title = news.Title,
                NewsPhotos = newsPhotosList,
            };


            return View(newsViewModel);
        }

        // POST: Admin/News/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "NewsID,Title,Content,PostDate,FileName ,NewsPhotos,NewsPhotoDetailID")] NewsViewModel newsViewModel)
        {
            //var fname = newsViewModel.NewsPhotos.ToArray();


            if (ModelState.IsValid)
            {

                newsViewModel.Content = HttpUtility.HtmlEncode(newsViewModel.Content);

                db.Entry(newsViewModel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(newsViewModel);
        }




        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
