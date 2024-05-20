using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.IO.DataFile;

public static class DataFile
{
    // Static fields.
    public const string FileExtension = ".ghdf";

    public static byte[] Signature { get; } =
    { 102, 37, 143, 181, 3, 205, 123, 185, 148, 157, 98, 177, 178, 151, 43, 170 };

    public const int FormatVersion = 1;
}