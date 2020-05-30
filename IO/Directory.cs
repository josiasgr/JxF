using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JxF.IO
{
    public class Directory
    {
        public Task<IEnumerable<DeletedStatus>> DeleteFolders(
            string baseFolder,
            string regexPattern,
            bool useRecycleBin = true,
            bool dryrun = false
        )
        {
            var regex = new Regex(regexPattern);

            var foldersToDelete = new DirectoryInfo(baseFolder)
                                    .GetDirectories("*", System.IO.SearchOption.AllDirectories)
                                    .Where(w => regex.IsMatch(w.Name))
                                    .Select(s => new DeletedStatus { FullName = s.FullName });

            if (dryrun || !foldersToDelete.Any())
            {
                return Task.FromResult(foldersToDelete);
            }

            foreach (var folder in foldersToDelete)
            {
                try
                {
                    if (System.IO.Directory.Exists(folder.FullName))
                    {
                        if (useRecycleBin)
                        {
                            FileSystem.DeleteDirectory(folder.FullName, UIOption.AllDialogs, RecycleOption.SendToRecycleBin);
                        }
                        else
                        {
                            System.IO.Directory.Delete(folder.FullName, true);
                        }
                    }
                }
                catch (Exception ex)
                {
                    folder.Exception = ex;
                }
            }
            return Task.FromResult(foldersToDelete);
        }
    }
}