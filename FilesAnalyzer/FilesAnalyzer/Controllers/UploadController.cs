using FilesAnalyzer.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FilesAnalyzer.Controllers
{
    public class UploadController : Controller
    {
        // GET: Upload
        public ActionResult Index()
        {
            return View("Upload");
        }

        //[HttpGet]
        //public ActionResult Upload()
        //{
        //    return View();
        //}

        [HttpPost]
        public ActionResult Upload()
        {
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];

                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    string[] fileNameSplit = fileName.Split('.');
                    var myUniqueFileName = string.Format(@"{0}.{1}", DateTime.Now.Ticks, fileNameSplit[1]);
                    var path = Path.Combine(Server.MapPath("~/Files/"), myUniqueFileName);//TO DO change path
                    file.SaveAs(path);
                    if ( new FDBFileRepository().AddFileRecord("author", "description", fileNameSplit[1], fileNameSplit[0], myUniqueFileName, file.ContentLength, "title"))
                    {//succesfully uploaded
                        return RedirectToAction("UploadDocument");
                    }

                }
            }

            return View();
        }
    }
}