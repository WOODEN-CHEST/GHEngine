using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;


namespace GHEngine.IO.DataFile;

internal sealed class DataFileReaderV1 : DataFileReader
{
    // Fields.
    public override int FormatVersion => 1;


    // Constructors.
    internal DataFileReaderV1(BinaryReader reader, string filePath) : base(reader, filePath) { }


    // Methods.
    public override GHDFCompound Read()
    {
        try
        {
            return ReadCompound();
        }
        catch (Exception e) when (e is not DataReadException)
        {
            throw new DataReadException(FilePath, e.ToString());
        }
    }


    // Private methods
    private object ReadItem(int id)
    {
        byte ItemTypeByte = Reader.ReadByte();
        GHDFType ItemType = (GHDFType)(ItemTypeByte & ~(int)DataTypeFlag.Array);
        bool IsArray = (ItemTypeByte & (int)DataTypeFlag.Array) != 0;

        object Value = IsArray ? ReadArray(ItemType) : ReadSingleValue(ItemType);

        return Value;
    }

    private object ReadSingleValue(GHDFType itemType)
    {
        return itemType switch
        {
            GHDFType.Int8 => Reader!.ReadByte(),
            GHDFType.SInt8 => Reader!.ReadSByte(),
            GHDFType.Int16 => Reader!.ReadInt16(),
            GHDFType.UInt16 => Reader!.ReadUInt16(),
            GHDFType.Int32 => Reader!.ReadInt32(),
            GHDFType.UInt32 => Reader!.ReadUInt32(),
            GHDFType.Int64 => Reader!.ReadInt64(),
            GHDFType.UInt64 => Reader!.ReadUInt64(),

            GHDFType.Float => Reader!.ReadSingle(),
            GHDFType.Double => Reader!.ReadDouble(),

            GHDFType.Boolean => Reader!.ReadBoolean(),

            GHDFType.Char => Reader!.ReadChar(),
            GHDFType.String => Reader!.ReadString(),

            GHDFType.Compound => ReadCompound(),

            _ => throw new DataReadException($"Unknown or invalid data type found." +
            $"{Environment.NewLine}Enum name: \"{itemType}\". Enum byte value: {(byte)itemType}.")
        };
    }

    private object[] ReadArray(GHDFType typeOfValue)
    {
        int ArrayLength = Reader!.Read7BitEncodedInt();
        if (ArrayLength < 0)
        {
            throw new DataReadException($"Invalid data array length: {ArrayLength}");
        }

        object[] DataArray = new object[ArrayLength];

        for (int i = 0; i < ArrayLength; i++)
        {
            DataArray[i] = ReadSingleValue(typeOfValue);
        }

        return DataArray;
    }

    private GHDFCompound ReadCompound()
    {
        GHDFCompound Compound = new();

        int ID;
        while ((ID = Reader.Read7BitEncodedInt()) != IDataFile.TERMINATING_ID)
        {
            Compound.Add(ID, ReadItem(ID));
        }

        return Compound;
    }
}