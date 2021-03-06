﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LKMMVC_1.Models;
using LKMMVC_1.Areas.Admin.ViewModel;
using System.IO;
using ImageResizer;

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


        // GET: Admin/News/Create
        public ActionResult Create()
        {
            //perduodamas messge po sekmingo patalpinimo
            var Message = TempData["ResultMessage"];
            if (Message != null)
            {
                ViewBag.ResultMessage = Message.ToString();
            }
            return View();
        }

        // POST: Admin/News/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Title,Content,PostDate,")] News news)
        {

            FileUploadValidation uploadedFiles = new FileUploadValidation();

            //tikrinama ar is vis prisegtas failas ir ar tinkamas formatas ir dydis, bei ar ivesta data
            if (Request.Files[0].ContentLength > 0 && news.PostDate != DateTime.MinValue)
            {
                uploadedFiles.filesize = 6000;
                uploadedFiles.ValidateUploadedUserFile(Request.Files, SuportedTypes.Images);
            }
            //--------------

            if (ModelState.IsValid && uploadedFiles.IsValid)
            {

                string uploadDirectoryYears = Path.Combine(Request.PhysicalApplicationPath, @"Photo\News\" + news.PostDate.Year.ToString());
                string uploadDirectoryMonth = Path.Combine(uploadDirectoryYears, news.PostDate.Month.ToString());
                string uploadDirectory = Path.Combine(uploadDirectoryMonth, CalculationHelper.ChangeNewsTitle(news.Title));
                string uploadDirectoryForView = uploadDirectory.Substring(uploadDirectory.IndexOf("Photo"));
                //sukuria Thumb kataloga sumazintoms nuotraukoms
                string uploadDirectoryThumb = Path.Combine(uploadDirectory, "Thumb");
                string uploadDirectoryThumbView = uploadDirectoryThumb.Substring(uploadDirectoryThumb.IndexOf("Photo"));

                List<NewsPhotoDetail> photoDetails = new List<NewsPhotoDetail>();
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    var file = Request.Files[i];

                    if (file != null && file.ContentLength > 0)
                    {
                        var fileName = i + 1 + Path.GetExtension(file.FileName);

                        NewsPhotoDetail photoDetail = new NewsPhotoDetail()
                        {
                            FileName = fileName,
                            NewsID = news.NewsID,
                            PhotoLocation = uploadDirectoryForView,
                            PhotoLocationThumb = uploadDirectoryThumbView
                        };
                        photoDetails.Add(photoDetail);

                        if (!Directory.Exists(uploadDirectory))
                        {
                            Directory.CreateDirectory(uploadDirectory);
                        }

                        //var path1 = Path.Combine(Server.MapPath("~/Photo/News/"), photoDetail.FileName);
                        var path = Path.Combine(uploadDirectory, photoDetail.FileName);
                        file.SaveAs(path);

                        //sumazintas foto(thumbnail) sukuria pavadinima ir ikelia--------
                        ResizeSettings resizeSetting = new ResizeSettings
                        {
                            Width = 105,
                            Height = 70,
                            Format = "png"
                        };

                        //patikrina ar egzistuoja Thumb katalogas jei ne sukuria
                        if (!Directory.Exists(uploadDirectoryThumb))
                        {
                            Directory.CreateDirectory(uploadDirectoryThumb);
                        }
                        var pathThumb = Path.Combine(uploadDirectoryThumb, photoDetail.FileName);
                        ImageBuilder.Current.Build(path, pathThumb, resizeSetting);
                        //-------------------------------------------------------------------
                    }
                }

                news.NewsPhotoDetails = photoDetails;
                news.Title = news.Title.ToUpper();
                //encodinimas keliant i DB
                news.Content = HttpUtility.HtmlEncode(news.Content);
                db.News.Add(news);
                db.SaveChanges();

                //pranesimas po sekmingo patalpinimo, TempData naudojame su RedirectToAction
                TempData["ResultMessage"] = uploadedFiles.Message;
                return RedirectToAction("Create");
            }

            //pranesimas apie nepavykusi ikelima failu
            ViewBag.ResultMessage = uploadedFiles.Message;
            return View(news);
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
        public ActionResult Edit([Bind(Include = "NewsID,Title,Content,PostDate,FileName ,NewsPhotos,NewsPhotoDetailID")] NewsViewModel newsViewModel, bool[] Remove, IEnumerable<HttpPostedFileBase> uploadFiles)
        {
            int oldPhotoCount = db.NewsPhotoDetails.Where(m => m.NewsID == newsViewModel.NewsID).Count();
            //checkinimo reikia ar yra foto is viso
            if (Utils.IsAny(uploadFiles) && oldPhotoCount > 0)
            {

                var newsPhotos = newsViewModel.NewsPhotos.ToArray();

                ResizeSettings resizeSetting = new ResizeSettings
                {
                    Width = 105,
                    Height = 70,
                    Format = "png"
                };

                //sukuriamas kelias kur uploadint faila
                var uploadDirectory = Path.Combine(Request.PhysicalApplicationPath, db.NewsPhotoDetails.Find(newsPhotos.FirstOrDefault().NewsPhotoDetailID).PhotoLocation);
                //sumazintas foto(thumbnail) sukuria pavadinima ir ikelia--------
                var uploadDirectoryThumb = Path.Combine(uploadDirectory, "Thumb");

                for (int i = 0; i < uploadFiles.Count(); i++)
                {
                    if (uploadFiles.ElementAt(i) != null)
                    {
                        newsPhotos[i].FileName = newsPhotos[i].EditedFileName;
                        var path = Path.Combine(uploadDirectory, newsPhotos[i].FileName);
                        uploadFiles.ElementAt(i).SaveAs(path);

                        var pathThumb = Path.Combine(uploadDirectoryThumb, newsPhotos[i].FileName);
                        ImageBuilder.Current.Build(path, pathThumb, resizeSetting);
                    }

                }
            }
            else if (newsViewModel.NewsPhotos != null && Utils.CheckBoxDeletePhoto(newsViewModel.NewsPhotos))
            {
                NewsPhotoDetail newsPhotoDetailDelete;
                foreach (var item in newsViewModel.NewsPhotos)
                {
                    if (item.IsChecked == true)
                    {
                        newsPhotoDetailDelete = db.NewsPhotoDetails.Find(item.NewsPhotoDetailID);
                        //db.Entry(newsPhotoDetailDelete).State = EntityState.Deleted;
                        db.NewsPhotoDetails.Remove(newsPhotoDetailDelete);
                    }
                }
                //db.SaveChanges();




                Utils.CheckBoxDeletePhoto(newsViewModel.NewsPhotos);
            }
            else if (oldPhotoCount == 0)
            {
                FileUploadValidation uploadedFiles = new FileUploadValidation();

                //tikrinama ar is vis prisegtas failas ir ar tinkamas formatas ir dydis, bei ar ivesta data
                if (Request.Files[0].ContentLength > 0 && newsViewModel.PostDate != DateTime.MinValue)
                {
                    uploadedFiles.filesize = 6000;
                    uploadedFiles.ValidateUploadedUserFile(Request.Files, SuportedTypes.Images);
                }
                //--------------

                if (ModelState.IsValid && uploadedFiles.IsValid)
                {

                    string uploadDirectoryYears = Path.Combine(Request.PhysicalApplicationPath, @"Photo\News\" + newsViewModel.PostDate.Year.ToString());
                    string uploadDirectoryMonth = Path.Combine(uploadDirectoryYears, newsViewModel.PostDate.Month.ToString());

                    string uploadDirectory = Path.Combine(uploadDirectoryMonth, CalculationHelper.ChangeNewsTitle(newsViewModel.Title));
                    string uploadDirectoryForView = uploadDirectory.Substring(uploadDirectory.IndexOf("Photo"));
                    //sukuria Thumb kataloga sumazintoms nuotraukoms
                    string uploadDirectoryThumb = Path.Combine(uploadDirectory, "Thumb");
                    string uploadDirectoryThumbView = uploadDirectoryThumb.Substring(uploadDirectoryThumb.IndexOf("Photo"));

                    List<NewsPhotoDetail> photoDetails = new List<NewsPhotoDetail>();
                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        var file = Request.Files[i];

                        if (file != null && file.ContentLength > 0)
                        {
                            var fileName = i + 1 + Path.GetExtension(file.FileName);

                            NewsPhotoDetail photoDetail = new NewsPhotoDetail()
                            {
                                FileName = fileName,
                                NewsID = newsViewModel.NewsID,
                                PhotoLocation = uploadDirectoryForView,
                                PhotoLocationThumb = uploadDirectoryThumbView
                            };
                            photoDetails.Add(photoDetail);

                            if (!Directory.Exists(uploadDirectory))
                            {
                                Directory.CreateDirectory(uploadDirectory);
                            }

                            //var path1 = Path.Combine(Server.MapPath("~/Photo/News/"), photoDetail.FileName);
                            var path = Path.Combine(uploadDirectory, photoDetail.FileName);
                            file.SaveAs(path);

                            //sumazintas foto(thumbnail) sukuria pavadinima ir ikelia--------
                            ResizeSettings resizeSetting = new ResizeSettings
                            {
                                Width = 105,
                                Height = 70,
                                Format = "png"
                            };

                            //patikrina ar egzistuoja Thumb katalogas jei ne sukuria
                            if (!Directory.Exists(uploadDirectoryThumb))
                            {
                                Directory.CreateDirectory(uploadDirectoryThumb);
                            }
                            var pathThumb = Path.Combine(uploadDirectoryThumb, photoDetail.FileName);
                            ImageBuilder.Current.Build(path, pathThumb, resizeSetting);
                            //-------------------------------------------------------------------
                        }
                    }

                    News newsUpdate = db.News.Find(newsViewModel.NewsID);
                    //encodinimas keliant i DB
                    newsUpdate.Content = HttpUtility.HtmlEncode(newsViewModel.Content);
                    newsUpdate.Title = newsViewModel.Title;
                    newsUpdate.PostDate = newsViewModel.PostDate;

                    foreach (var item in photoDetails)
                    {
                        //irasomos visos pridetos nuotraukos i DB
                        db.NewsPhotoDetails.Add(item);
                    }


                    db.Entry(newsUpdate).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                   
                    //pranesimas po sekmingo patalpinimo, TempData naudojame su RedirectToAction
                    TempData["ResultMessage"] = uploadedFiles.Message;
                    return RedirectToAction("Create");
                }



            }







            //NewsPhotoDetail photoDetail = new NewsPhotoDetail()
            //{
            //    FileName = fileName,
            //    NewsID = news.NewsID,
            //    PhotoLocation = uploadDirectoryForView,
            //    PhotoLocationThumb = uploadDirectoryThumbView
            //};
            //photoDetails.Add(photoDetail);


            if (ModelState.IsValid)
            {
                News newsUpdate = db.News.Find(newsViewModel.NewsID);

                newsUpdate.Content = HttpUtility.HtmlEncode(newsViewModel.Content);
                newsUpdate.Title = newsViewModel.Title;
                newsUpdate.PostDate = newsViewModel.PostDate;


                db.Entry(newsUpdate).State = EntityState.Modified;
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
