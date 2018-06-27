using FilesAnalyzer.Helpers;
using FilesAnalyzer.Repositories;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.HtmlControls;
using TikaOnDotNet.TextExtraction;
using static javax.measure.unit.Dimension;

namespace FilesAnalyzer.Controllers
{
    public class UploadController : Controller
    {
        // GET: Upload
        public ActionResult Index()
        {
            return View("Upload");
        }



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

                        Analyzer analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29);
                        Lucene.Net.Store.Directory directory = FSDirectory.Open(@"C:\Users\Totalit\Source\Repos\FilesAnalyzer\FilesAnalyzer\FilesAnalyzer\IndexedFiles");
                        IndexWriter writer = new IndexWriter(directory, analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED);
                        Document doc = new Document();

                        doc.Add(new Field("Author",
                        author,
                        Field.Store.YES,
                        Field.Index.ANALYZED));
                        doc.Add(new Field("title",
                        title,
                        Field.Store.YES,
                        Field.Index.ANALYZED));
                        doc.Add(new Field("description",
                        description,
                        Field.Store.YES,
                        Field.Index.ANALYZED));
                        doc.Add(new Field("text",
                        wordDocContents.Text,
                        Field.Store.YES,
                        Field.Index.ANALYZED));


                        writer.AddDocument(doc);
                        writer.Optimize();
                        writer.Close();

                    }
                    else
                    {
                        // TO DO html version
                    }


                }
            }

            return View();
        }

        [HttpPost]
        public ActionResult GetSearchData(string tbSearchValue)
        {

            List<SampleData> searchresult = new LuceneSearch().Search(tbSearchValue).ToList();


            return Json(new { Url = Url.Action("List", searchresult) });
        }
    }
}