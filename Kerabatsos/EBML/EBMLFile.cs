using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kerabatsos.EBML
{
    public class EBMLFile
    {
        public Dictionary<ulong, EBMLAudioTrack> AudioTracks;
        public Dictionary<ulong, EBMLVideoTrack> VideoTracks;

        public EBMLFile(EBMLTag root)
        {
            AudioTracks = new Dictionary<ulong, EBMLAudioTrack>();
            VideoTracks = new Dictionary<ulong, EBMLVideoTrack>();
            var ebmlTag = root.FindTypeInChildren(EBMLTagType.EBML);
            string doctype = (string)(ebmlTag.FindTypeInChildren(EBMLTagType.DocType).Value);
            if (doctype != "webm")
                throw new InvalidDataException("The doctype '" + doctype + "' is not supported.");
            var segment = root.FindTypeInChildren(EBMLTagType.Segment);
            var tracks = segment.FindTypeInChildren(EBMLTagType.Tracks);
            var entries = tracks.FindTypesInChildren(EBMLTagType.TrackEntry);
            foreach (EBMLTag track in entries.Where(x => (ulong)(x.FindTypeInChildren(EBMLTagType.TrackType).Value) == 1))
            {
                var videoTrack = new EBMLVideoTrack(track);
                VideoTracks.Add(videoTrack.Number, videoTrack);
            }
            foreach (EBMLTag track in entries.Where(x => (ulong)x.FindTypeInChildren(EBMLTagType.TrackType).Value == 2))
            {
                var audioTrack = new EBMLAudioTrack(track);
                AudioTracks.Add(audioTrack.Number, audioTrack);
            }
            if (VideoTracks.Values.Any(x => x.Codec != "V_VP8"))
                throw new InvalidDataException("The video codec " + VideoTracks.Values.First(x => x.Codec != "V_VP8").Codec + " is not valid.");
            if (AudioTracks.Values.Any(x => x.Codec != "A_VORBIS"))
                throw new InvalidDataException("The audio codec " + AudioTracks.Values.First(x => x.Codec != "A_VORBIS").Codec + " is not valid.");
            var clusters = segment.FindTypesInChildren(EBMLTagType.Cluster);
            foreach(EBMLTag cluster in clusters)
            {
                var timecode = (ulong)cluster.FindTypeInChildren(EBMLTagType.Timecode).Value;
                var simpleBlocks = cluster.FindTypesInChildren(EBMLTagType.SimpleBlock);
                foreach (EBMLTag t in simpleBlocks)
                    ParseBlock(t, true);
                var blocks = cluster.FindTypesInChildren(EBMLTagType.Block);
                foreach (EBMLTag t in blocks)
                    ParseBlock(t, false);
            }
        }

        private void ParseBlock(EBMLTag block, bool simple)
        {
            var reader = block.Reader;
            reader.BaseStream.Position = block.Position;
            int size = EBMLReader.GetCodedSize(reader.ReadByte());
            reader.BaseStream.Seek(-1, SeekOrigin.Current);
            byte[] data = reader.ReadBytes(size);
            data[0] -= (byte)Math.Pow(2, 8 - size);
            ulong trackNumber = EBMLReader.GetULongFromBytes(data);
            short timecode = reader.ReadInt16();
            byte flags = reader.ReadByte();
            EBMLBlock eBlock = new EBMLBlock(reader.BaseStream.Position, block.Size, trackNumber, timecode);
            if (simple)
            {
                eBlock.Keyframe = (flags & 0x80) == 1;
                eBlock.Discardable = (flags & 0x02) == 1;
            }
            eBlock.Invisible = (flags & 0x08) == 1;
            byte lacing = (byte)(flags & 0x06);
            if (lacing == 1)
                eBlock.Lacing = EBMLBlock.BlockLacing.Xiph;
            if (lacing == 3)
                eBlock.Lacing = EBMLBlock.BlockLacing.EBML;
            if (lacing == 2)
                eBlock.Lacing = EBMLBlock.BlockLacing.FixedSize;
            if (AudioTracks.ContainsKey(trackNumber))
                AudioTracks[trackNumber].Blocks.Add(eBlock);
            if (VideoTracks.ContainsKey(trackNumber))
                VideoTracks[trackNumber].Blocks.Add(eBlock);
        }

        public class EBMLBlock
        {
            public bool Keyframe = false;
            public bool Invisible = false;
            public BlockLacing Lacing = BlockLacing.None;
            public bool Discardable = false;

            private short _timecode;
            private ulong _trackNumber;
            private long _position;
            private long _size;

            public short Timecode => _timecode;
            public ulong TrackNumber => _trackNumber;
            public long Position => _position;
            public long Size => _size;

            public EBMLBlock(long position, long size, ulong trackNumber, short timecode)
            {
                _size = size;
                _position = position;
                _timecode = timecode;
                _trackNumber = trackNumber;
            }
            
            public enum BlockLacing
            {
                None,
                Xiph,
                EBML,
                FixedSize
            }
        }

        public class EBMLVideoTrack
        {
            private ulong _number;
            private ulong _uid;
            private string _language;
            private string _codec;
            private ulong _width, _height;
            public List<EBMLBlock> Blocks;

            public ulong Number => _number;
            public ulong UID => _uid;
            public string Language => _language;
            public string Codec => _codec;
            public ulong Width => _width;
            public ulong Height => _height;

            public EBMLVideoTrack(EBMLTag track)
            {
                Blocks = new List<EBMLBlock>();
                _number = (ulong)track.FindTypeInChildren(EBMLTagType.TrackNumber).Value;
                _uid = (ulong)track.FindTypeInChildren(EBMLTagType.TrackUID).Value;
                _language = (string)track.FindTypeInChildren(EBMLTagType.Language).Value;
                _codec = (string)track.FindTypeInChildren(EBMLTagType.CodecID).Value;
                var video = track.FindTypeInChildren(EBMLTagType.Video);
                _width = (ulong)video.FindTypeInChildren(EBMLTagType.PixelWidth).Value;
                _height = (ulong)video.FindTypeInChildren(EBMLTagType.PixelHeight).Value;
            }
        }

        public class EBMLAudioTrack
        {
            private ulong _number;
            private ulong _uid;
            private string _language;
            private string _codec;
            private ulong _channels;
            private float _samplingRate;
            private ulong _bitDepth;
            public List<EBMLBlock> Blocks;

            public ulong Number => _number;
            public ulong UID => _uid;
            public string Language => _language;
            public string Codec => _codec;
            public ulong Channels => _channels;
            public float SamplingRate => _samplingRate;
            public ulong BitDepth => _bitDepth;

            public EBMLAudioTrack(EBMLTag track)
            {
                Blocks = new List<EBMLBlock>();
                _number = (ulong)track.FindTypeInChildren(EBMLTagType.TrackNumber).Value;
                _uid = (ulong)track.FindTypeInChildren(EBMLTagType.TrackUID).Value;
                _language = (string)track.FindTypeInChildren(EBMLTagType.Language).Value;
                _codec = (string)track.FindTypeInChildren(EBMLTagType.CodecID).Value;
                var audio = track.FindTypeInChildren(EBMLTagType.Audio);
                _channels = (ulong)track.FindTypeInChildren(EBMLTagType.Channels).Value;
                _samplingRate = (ulong)track.FindTypeInChildren(EBMLTagType.SamplingFrequency).Value;
                _bitDepth = (ulong)track.FindTypeInChildren(EBMLTagType.BitDepth).Value;
            }
        }
    }
}
