using FilesAnalyzer.DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FilesAnalyzer.Repositories
{
    public class FDBFileRepository
    {
        public bool AddFileRecord(string author, string description, string extension, string originalName, string serverName, int size, string title)
        {
            try
            {
                FDBFile fDBFile = new FDBFile();
                fDBFile.Author = author;
                fDBFile.Description = description;
                fDBFile.Extension = extension;
                fDBFile.IsDeleted = false;
                fDBFile.OriginalName = originalName;
                fDBFile.ServerName = serverName;
                fDBFile.Size = size;
                fDBFile.Title = title;

                using (var context = new FDBEntities())
                {
                    context.FDBFile.Add(fDBFile);
                    context.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                //TO DO exception to handle
                return false;
            }

        }
    }
}