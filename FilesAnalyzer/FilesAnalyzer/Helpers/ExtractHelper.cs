using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;

namespace FilesAnalyzer.Helpers
{
    public class SampleData
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
    }

    public class ExtractHelper
    {

        public string parseToString(string path)
        {
            try
            {
                return "";
            }
            catch (Exception ex)
            {
                //TO DO Exception handle
                return "";
            }
        }


    }


    public class LuceneSearch
    {
        private static Query parseQuery(string searchQuery, QueryParser parser)
        {
            Query query;
            try
            {
                query = parser.Parse(searchQuery.Trim());
            }
            catch (ParseException)
            {
                query = parser.Parse(QueryParser.Escape(searchQuery.Trim()));
            }
            return query;
        }

        private SampleData _mapLuceneDocumentToData(Document doc)
        {
            return new SampleData
            {
                Title = doc.Get("title"),
                Author = doc.Get("Author"),
                Description = doc.Get("description")
            };
        }
        private IEnumerable<SampleData> _mapLuceneToDataList(IEnumerable<Document> hits)
        {
            return hits.Select(_mapLuceneDocumentToData).ToList();
        }
        private IEnumerable<SampleData> _mapLuceneToDataList(IEnumerable<ScoreDoc> hits,
            IndexSearcher searcher)
        {
            return hits.Select(hit => _mapLuceneDocumentToData(searcher.Doc(hit.Doc))).ToList();
        }

        private IEnumerable<SampleData> _search(string searchQuery)
        {
            // validation
            if (string.IsNullOrEmpty(searchQuery.Replace("*", "").Replace("?", ""))) return new List<SampleData>();

            // set up lucene searcher
            using (var searcher = new IndexSearcher(FSDirectory.Open(@"C:\Users\Totalit\Source\Repos\FilesAnalyzer\FilesAnalyzer\FilesAnalyzer\IndexedFiles"), false))
            {
                var hits_limit = 1000;
                var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);


                var parser = new MultiFieldQueryParser
                    (Lucene.Net.Util.Version.LUCENE_30, new[] { "Author", "title", "description" }, analyzer);
                var query = parseQuery(searchQuery, parser);
                var hits = searcher.Search
                (query, null, hits_limit, Sort.RELEVANCE).ScoreDocs;
                var results = _mapLuceneToDataList(hits, searcher);
                analyzer.Close();
                searcher.Dispose();
                return results;

            }
        }

        public IEnumerable<SampleData> Search(string input)
        {
            if (string.IsNullOrEmpty(input)) return new List<SampleData>();

            var terms = input.Trim().Replace("-", " ").Split(' ')
                .Where(x => !string.IsNullOrEmpty(x)).Select(x => x.Trim() + "*");
            input = string.Join(" ", terms);

            return _search(input);
        }

        public IEnumerable<SampleData> GetAllIndexRecords()
        {

            // set up lucene searcher
            var searcher = new IndexSearcher(FSDirectory.Open(@"C:\Users\Totalit\Source\Repos\FilesAnalyzer\FilesAnalyzer\FilesAnalyzer\IndexedFiles"), false);
            var reader = IndexReader.Open(FSDirectory.Open(@"C:\Users\Totalit\Source\Repos\FilesAnalyzer\FilesAnalyzer\FilesAnalyzer\IndexedFiles"), false);
            var docs = new List<Document>();
            var term = reader.TermDocs();
            while (term.Next()) docs.Add(searcher.Doc(term.Doc));
            reader.Dispose();
            searcher.Dispose();
            return _mapLuceneToDataList(docs);
        }
    }
}