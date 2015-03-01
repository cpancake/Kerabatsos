using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using GisSharpBlog.NetTopologySuite.IO;

namespace Kerabatsos.EBML
{
    public class EBMLReader
    {
        private BigEndianBinaryReader _reader;
        private Stream _stream;
        private EBMLTag _rootTag;

        public EBMLReader(Stream stream)
        {
            _reader = new BigEndianBinaryReader(stream);
            _stream = stream;
        }

        public EBMLFile Read()
        {
            _rootTag = new EBMLTag(0, _stream.Length, EBMLTagType.None, EBMLTagContents.Master);
            while (_reader.BaseStream.Position < _stream.Length)
                _rootTag.Children.Add(ReadTag());
            return new EBMLFile(_rootTag);
        }

        private EBMLTag ReadTag()
        {
            // find tag id
            int idSize = GetCodedSize(_reader.ReadByte());
            _reader.BaseStream.Seek(-1, SeekOrigin.Current);
            int id = (int)GetLongFromBytes(_reader.ReadBytes(idSize));
            // find tag data size
            int dataSize = GetCodedSize(_reader.ReadByte());
            _reader.BaseStream.Seek(-1, SeekOrigin.Current);
            byte[] data = _reader.ReadBytes(dataSize);
            // remove size hint from first byte
            data[0] -= (byte)Math.Pow(2, 8 - dataSize);
            long size = GetLongFromBytes(data);
            EBMLTag tag = new EBMLTag(_reader.BaseStream.Position, size, EBMLTag.TypeFromID(id), EBMLTag.ContentTypeFromID(id));
            tag.Reader = _reader;
            if (tag.ContentType == EBMLTagContents.Binary)
                _reader.BaseStream.Position += size;
            if (tag.ContentType == EBMLTagContents.Date)
                tag.Value = _reader.ReadUInt64();
            if (tag.ContentType == EBMLTagContents.Float)
                tag.Value = (size == 4 ? _reader.ReadSingle() : _reader.ReadDouble());
            if (tag.ContentType == EBMLTagContents.SignedInteger)
                tag.Value = GetLongFromBytes(_reader.ReadBytes((int)size));
            if (tag.ContentType == EBMLTagContents.String)
                tag.Value = Encoding.ASCII.GetString(_reader.ReadBytes((int)size));
            if (tag.ContentType == EBMLTagContents.UnsignedInteger)
                tag.Value = GetULongFromBytes(_reader.ReadBytes((int)size));
            if (tag.ContentType == EBMLTagContents.UTF8String)
                tag.Value = Encoding.UTF8.GetString(_reader.ReadBytes((int)size));
            if(tag.ContentType == EBMLTagContents.Master)
            {
                // there's more tags to be found!
                long startPos = _reader.BaseStream.Position;
                while (size - (_reader.BaseStream.Position - startPos) > 1)
                    tag.Children.Add(ReadTag());
            }
            return tag;
        }

        // https://github.com/johnnoel/ebmlreader/blob/master/EbmlReader/Ebml.cs#L133
        public static int GetCodedSize(byte toRead)
        {
            int ret = 1;
            for (int i = 7; i >= 0; i--, ret++)
                if ((toRead >> i) == 1)
                    break;

            return ret;
        }

        public static long GetLongFromBytes(byte[] id)
        {
            long ret = 0;
            foreach (byte b in id)
                ret = (ret << 8) | b;

            return ret;
        }

        public static ulong GetULongFromBytes(byte[] id)
        {
            ulong ret = 0;
            foreach (byte b in id)
                ret = (ret << 8) | b;

            return ret;
        }
    }
}
