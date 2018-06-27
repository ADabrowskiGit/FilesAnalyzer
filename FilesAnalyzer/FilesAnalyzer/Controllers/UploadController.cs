using FilesAnalyzer.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TikaOnDotNet.TextExtraction;

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

                    string author = "";
                    string title = "";
                    string description = "";
                    if (fileNameSplit[1] != "pdf")
                    {
                        var textExtractor = new TextExtractor();
                        var wordDocContents = textExtractor.Extract(path);
                        if (!wordDocContents.Metadata.TryGetValue("Author", out author))
                        {
                            author = "";
                        }
                        if (!wordDocContents.Metadata.TryGetValue("title", out title))
                        {
                            title = "";
                        }
                        if (!wordDocContents.Metadata.TryGetValue("description", out description))
                        {
                            description = "";
                        }
                    }
                    if (new FDBFileRepository().AddFileRecord(author, description, fileNameSplit[1], fileNameSplit[0], myUniqueFileName, file.ContentLength, title))
                    {//succesfully uploaded
                        return RedirectToAction("UploadDocument");
                    }

                }
            }

            return View();
        }
    }
}