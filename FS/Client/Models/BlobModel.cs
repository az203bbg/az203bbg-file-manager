using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileExplorer.Client.Models
{
    public class BlobModel
    {
        public string Name { get; set; }
        public string Uri { get; set; }
        public string Container { get; set; }
    }
}
