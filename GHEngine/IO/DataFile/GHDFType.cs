namespace GHEngine.IO.DataFile;

public enum GHDFType : byte
{
    None = 0,

    Int8 = 1,
    UInt8 = 2,
    Int16 = 3,
    UInt16 = 4,
    Int32 = 5,
    UInt32 = 6,
    Int64 = 7,
    UInt64 = 8,

    Float = 9,
    Double = 10,

    Boolean = 11,
    
    String = 12,

    Compound = 13
}