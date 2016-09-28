using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Ext.LRC
{
    public class Reader
    {
        string FP = "";
        List<string> AllData;

        public Reader(string FilePath)
        {
            FP = FilePath;
            AllData = new List<string>();
            using (StreamReader sr = new StreamReader(FP, System.Text.Encoding.Default))
            {
                while(sr.Peek() > -1)
                {
                    AllData.Add(sr.ReadLine());
                }
            }
        }

        public string RemoveTimeAndHeader()
        {
            int i = 0;
            List<string> data = new List<string>();
            foreach(string line in AllData)
            {
                if(line.StartsWith("[") == true)
                {
                    int e = line.IndexOf("]");
                    if(e > -1)
                    {
                        string tex = line.Substring(e + 1);
                        if(tex.Length > 0)
                        {
                            data.Add(line.Substring(e + 1));
                        }
                    }
                }
                else
                {
                    data.Add(line);
                }
                i++;
            }

            return String.Join("\r\n", data.ToArray());
        }

        public class Lyrics
        {
            public DateTime Time;
            public string Text;
        }
    }
}
