using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAPP.MTag.Async
{
    public class FileReader
    {
        public static async Task<Tag> GetTag(string FilePath, bool FromFile)
        {
            Tag t = await Task.Run(() =>
            {
                return TagReader.GetTag(FilePath);
            });

            return t;
        }
    }
}
