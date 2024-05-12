using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Logging;

public interface ILogArchiver
{
    void Archive(string archiveDirectory, string path);
}