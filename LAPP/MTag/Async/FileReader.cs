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
                if (FromFile)
                    return TagReader.GetTagFromFile(FilePath);
                else
                    return TagReader.GetTag(FilePath);
            });

            return t;
        }
    }
}
