using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAPP.MTag.Async
{
    public class DirectoryReader
    {
        public DirectoryReader() { }

        public string[] SearchExtensions { get; set; } = TagReader.SupportedExtension;

        public System.IO.SearchOption SearchOption { get; set; } = System.IO.SearchOption.TopDirectoryOnly;

        public event EventHandler GetTagsCompleted;

        public async Task<Tag[]> GetTags(string DirectoryPath, bool FromFile)
        {
            Tag[] ts = await Task.Run(() =>
            {
                List<Tag> Tags = new List<Tag>();
                string[] Paths = System.IO.Directory.GetFiles(DirectoryPath, "*.*", SearchOption);

                for(int i= 0; Paths.Length > i; i++)
                {
                    if (SearchExtensions.Contains(System.IO.Path.GetExtension(Paths[i])))
                    {
                        Tags.Add(TagReader.GetTag(Paths[i]));
                    }
                }

                GetTagsCompleted?.Invoke(this, new EventArgs());

                return Tags.ToArray();
            });

            return ts;
        }
    }
}
