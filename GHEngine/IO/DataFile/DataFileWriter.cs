using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.IO.DataFile;

public class DataFileWriter
{
    // Fields.
    public const int LatestVersion = 1;


    // Constructors.
    public DataFileWriter() { }


    // Methods.
    public void Write(string path, GHDFCompound baseCompound)
    {
        if (!Path.IsPathFullyQualified(path))
        {
            throw new ArgumentException("Path isn't fully qualified.", nameof(path));
        }
        string TargetPath = Path.ChangeExtension(path, DataFile.FileExtension);

        try
        {
            using (Stream DataStream = File.OpenWrite(TargetPath))
            {
                Write(DataStream, baseCompound);
            }
        }
        catch (IOException e)
        {
            throw new DataFileWriteException(path, e.ToString());
        }
    }

    public void Write(Stream stream, GHDFCompound baseCompound)
    {
        if (baseCompound == null)
        {
            throw new ArgumentNullException(nameof(baseCompound));
        }

        try
        {
            using BinaryWriter Writer = new(stream);
            WriteMetaData(Writer);
            WriteData(Writer, baseCompound);
        }
        catch (Exception e)
        {
            throw new DataFileWriteException(e.ToString());
        }
    }


    // Private methods.
    private void WriteMetaData(BinaryWriter writer)
    {
        writer.Write(DataFile.Signature);
        writer.Write(DataFile.FormatVersion);
    }

    private void WriteData(BinaryWriter writer, GHDFCompound baseCompound)
    {
        List<byte> Bytes = new();
        WriteBytesOfCompound(Bytes, baseCompound);
        writer.Write(Bytes.ToArray());
    }

    private void WriteEntry(BinaryWriter writer, int id, GHDFType type, object value)
    {
        
    }

    private byte[] WriteBytesOfValue(GHDFType type, object value)
    {
        if (value is byte or sbyte)
        {
            return new byte[] { (byte)value };
        }
        else if (value is short or ushort)
        {
            return BitConverter.GetBytes((short)value);
        }
        else if (value is int or uint)
        {
            return BitConverter.GetBytes((int)value);
        }
        else if (value is long or ulong)
        {
            return BitConverter.GetBytes((long)value);
        }
        else if (value is float)
        {
            return BitConverter.GetBytes((float)value);
        }
        else if (value is double)
        {
            return BitConverter.GetBytes((double)value);
        }
        else if (value is bool)
        {
            return BitConverter.GetBytes((bool)value);
        }
        else if (value is GHDFCompound)
        {
            return WriteBytesOfCompound((GHDFCompound)value);
        }
        else
        {
            throw new DataFileWriteException($"Invalid data type \"{value.GetType()}\"");
        }
    }

    private void WriteBytesOfCompound(List<byte> bytes, GHDFCompound compound)
    {
        WriteEncodedInt(bytes, compound.Count);

        foreach (KeyValuePair<int, object> Entry in compound)
        {
            GHDFType Type = GetObjectType(Entry.Value);



        }


    }

    private GHDFType GetObjectType(object obj)
    {
        if (obj is byte)
        {
            return GHDFType.UInt8;
        }
        else if (obj is sbyte)
        {
            return GHDFType.Int8;
        }
        else if (obj is short)
        {
            return GHDFType.Int16;
        }
        else if (obj is ushort)
        {
            return GHDFType.UInt16;
        }
        else if (obj is int)
        {
            return GHDFType.Int32;
        }
        else if (obj is uint)
        {
            return GHDFType.UInt32;
        }
        else if (obj is long)
        {
            return GHDFType.Int64;
        }
        else if (obj is ulong)
        {
            return GHDFType.UInt64;
        }
        else if (obj is float)
        {
            return GHDFType.Float;
        }
        else if (obj is double)
        {
            return GHDFType.Double;
        }
        else if (obj is bool)
        {
            return GHDFType.Boolean;
        }
        else if (obj is string)
        {
            return GHDFType.String;
        }
        else if (obj is GHDFCompound)
        {
            return GHDFType.Compound;
        }
        else
        {
            throw new DataFileWriteException($"Unknown data type \"{obj.GetType()}\"");
        }
    }

    private void WriteEncodedInt(List<byte> bytes, int value)
    {
        uint Value = (uint)value;

        do
        {
            bytes.Add((byte)((Value & 127u) | (Value > 127u ? 128u : 0u)));
            Value >>= 7;
        } while (Value > 0);
    }
}