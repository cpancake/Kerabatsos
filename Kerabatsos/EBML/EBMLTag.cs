using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GisSharpBlog.NetTopologySuite.IO;

namespace Kerabatsos.EBML
{
    public class EBMLTag
    {
        private long _position;
        private long _size;
        private EBMLTagType _type;
        private EBMLTagContents _contentType;
        private object _value = null;

        public EBMLTagType Type => _type;
        public EBMLTagContents ContentType => _contentType;
        public long Position => _position;
        public long Size => _size;

        public BigEndianBinaryReader Reader;
        public List<EBMLTag> Children;
        public object Value
        {
            get
            {
                if(_contentType == EBMLTagContents.Binary && _value == null)
                {
                    long currentPosition = Reader.BaseStream.Position;
                    Reader.BaseStream.Seek(_position, System.IO.SeekOrigin.Begin);
                    _value = Reader.ReadBytes((int)_size);
                    Reader.BaseStream.Seek(currentPosition, System.IO.SeekOrigin.Begin);
                }
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        public static Dictionary<int, EBMLTagType> Tags = new Dictionary<int, EBMLTagType>()
        {
            { 0x1a45dfa3, EBMLTagType.EBML },
            { 0x4286, EBMLTagType.EBMLVersion },
            { 0x42f7, EBMLTagType.EBMLReadVersion },
            { 0x42f2, EBMLTagType.EBMLMaxIDLength },
            { 0x42f3, EBMLTagType.EBMLMaxSizeLength },
            { 0x4282, EBMLTagType.DocType },
            { 0x4287, EBMLTagType.DocTypeVersion },
            { 0x4285, EBMLTagType.DocTypeReadVersion },
            { 0xec, EBMLTagType.Void },
            { 0x18538067, EBMLTagType.Segment },
            { 0x114d9874, EBMLTagType.SeekHead },
            { 0x4dbb, EBMLTagType.Seek },
            { 0x53ab, EBMLTagType.SeekID },
            { 0x53ac, EBMLTagType.SeekPosition },
            { 0x1549a966, EBMLTagType.Info },
            { 0x2ad7b1, EBMLTagType.TimecodeScale },
            { 0x4489, EBMLTagType.Duration },
            { 0x4461, EBMLTagType.DateUTC },
            { 0x7ba9, EBMLTagType.Title },
            { 0x4d80, EBMLTagType.MuxingApp },
            { 0x5741, EBMLTagType.WritingApp },
            { 0x1f43b675, EBMLTagType.Cluster },
            { 0xe7, EBMLTagType.Timecode },
            { 0xab, EBMLTagType.PrevSize },
            { 0xa3, EBMLTagType.SimpleBlock },
            { 0xa0, EBMLTagType.BlockGroup },
            { 0xa1, EBMLTagType.Block },
            { 0x9b, EBMLTagType.Duration },
            { 0xfb, EBMLTagType.ReferenceBlock },
            { 0xcc, EBMLTagType.LaceNumber },
            { 0x1654ae6b, EBMLTagType.Tracks },
            { 0xae, EBMLTagType.TrackEntry },
            { 0xd7, EBMLTagType.TrackNumber },
            { 0x73c5, EBMLTagType.TrackUID },
            { 0x83, EBMLTagType.TrackType },
            { 0xb9, EBMLTagType.FlagEnabled },
            { 0x88, EBMLTagType.FlagDefault },
            { 0x55aa, EBMLTagType.FlagForced },
            { 0x9c, EBMLTagType.FlagLacing },
            { 0x23e383, EBMLTagType.DefaultDuration },
            { 0x536e, EBMLTagType.Name },
            { 0x22b59c, EBMLTagType.Language },
            { 0x86, EBMLTagType.CodecID },
            { 0x63a2, EBMLTagType.CodecPrivate },
            { 0x258688, EBMLTagType.CodecName },
            { 0xe0, EBMLTagType.Video },
            { 0x9a, EBMLTagType.FlagInterlaced },
            { 0x53b8, EBMLTagType.StereoMode },
            { 0x53c0, EBMLTagType.AlphaMode },
            { 0xb0, EBMLTagType.PixelWidth },
            { 0xba, EBMLTagType.PixelHeight },
            { 0x54aa, EBMLTagType.PixelCropBottom },
            { 0x54bb, EBMLTagType.PixelCropTop },
            { 0x54cc, EBMLTagType.PixelCropLeft },
            { 0x54dd, EBMLTagType.PixelCropRight },
            { 0x54b0, EBMLTagType.DisplayWidth },
            { 0x54ba, EBMLTagType.DisplayHeight },
            { 0x54b2, EBMLTagType.DisplayUnit },
            { 0x54b3, EBMLTagType.AspectRatioType },
            { 0x2383e3, EBMLTagType.FrameRate },
            { 0xe1, EBMLTagType.Audio },
            { 0xb5, EBMLTagType.SamplingFrequency },
            { 0x78b5, EBMLTagType.OutputSamplingFrequency },
            { 0x9f, EBMLTagType.Channels },
            { 0x6264, EBMLTagType.BitDepth },
            { 0x1c53bb6b, EBMLTagType.Cues },
            { 0xbb, EBMLTagType.CuePoint },
            { 0xb3, EBMLTagType.CueTime },
            { 0xb7, EBMLTagType.CueTrackPositions },
            { 0xf7, EBMLTagType.CueTrack },
            { 0xf1, EBMLTagType.CueClusterPosition },
            { 0xf0, EBMLTagType.CueRelativePosition },
            { 0xb2, EBMLTagType.CueDuration },
            { 0x5378, EBMLTagType.CueBlockNumber },
            { 0x1254c367, EBMLTagType.Tags },
            { 0x7373, EBMLTagType.Tag },
            { 0x63c0, EBMLTagType.Targets },
            { 0x68ca, EBMLTagType.TargetTypeValue },
            { 0x63ca, EBMLTagType.TargetType },
            { 0x63c5, EBMLTagType.TagTrackUID },
            { 0x67c8, EBMLTagType.SimpleTag },
            { 0x45a3, EBMLTagType.TagName },
            { 0x447a, EBMLTagType.TagLanguage },
            { 0x4484, EBMLTagType.TagDefault },
            { 0x4487, EBMLTagType.TagString },
            { 0x4485, EBMLTagType.TagBinary }
        };

        public static Dictionary<EBMLTagType, EBMLTagContents> Contents = new Dictionary<EBMLTagType, EBMLTagContents>()
        {
            { EBMLTagType.EBML, EBMLTagContents.Master },
            { EBMLTagType.EBMLVersion, EBMLTagContents.UnsignedInteger },
            { EBMLTagType.EBMLReadVersion, EBMLTagContents.UnsignedInteger },
            { EBMLTagType.EBMLMaxIDLength, EBMLTagContents.UnsignedInteger },
            { EBMLTagType.EBMLMaxSizeLength, EBMLTagContents.UnsignedInteger },
            { EBMLTagType.DocType, EBMLTagContents.String },
            { EBMLTagType.DocTypeVersion, EBMLTagContents.UnsignedInteger },
            { EBMLTagType.DocTypeReadVersion, EBMLTagContents.UnsignedInteger },
            { EBMLTagType.Void, EBMLTagContents.Binary },
            { EBMLTagType.Segment, EBMLTagContents.Master },
            { EBMLTagType.SeekHead, EBMLTagContents.Master },
            { EBMLTagType.Seek, EBMLTagContents.Master },
            { EBMLTagType.SeekID, EBMLTagContents.Binary },
            { EBMLTagType.SeekPosition, EBMLTagContents.UnsignedInteger },
            { EBMLTagType.Info, EBMLTagContents.Master },
            { EBMLTagType.TimecodeScale, EBMLTagContents.UnsignedInteger },
            { EBMLTagType.Duration, EBMLTagContents.Float },
            { EBMLTagType.DateUTC, EBMLTagContents.Date },
            { EBMLTagType.Title, EBMLTagContents.UTF8String },
            { EBMLTagType.MuxingApp, EBMLTagContents.UTF8String },
            { EBMLTagType.WritingApp, EBMLTagContents.UTF8String },
            { EBMLTagType.Cluster, EBMLTagContents.Master },
            { EBMLTagType.Timecode, EBMLTagContents.UnsignedInteger },
            { EBMLTagType.PrevSize, EBMLTagContents.UnsignedInteger },
            { EBMLTagType.SimpleBlock, EBMLTagContents.Binary },
            { EBMLTagType.BlockGroup, EBMLTagContents.Master },
            { EBMLTagType.Block, EBMLTagContents.Binary },
            { EBMLTagType.BlockDuration, EBMLTagContents.UnsignedInteger },
            { EBMLTagType.ReferenceBlock, EBMLTagContents.SignedInteger },
            { EBMLTagType.LaceNumber, EBMLTagContents.UnsignedInteger },
            { EBMLTagType.Tracks, EBMLTagContents.Master },
            { EBMLTagType.TrackEntry, EBMLTagContents.Master },
            { EBMLTagType.TrackNumber, EBMLTagContents.UnsignedInteger },
            { EBMLTagType.TrackUID, EBMLTagContents.UnsignedInteger },
            { EBMLTagType.TrackType, EBMLTagContents.UnsignedInteger },
            { EBMLTagType.FlagEnabled, EBMLTagContents.UnsignedInteger },
            { EBMLTagType.FlagDefault, EBMLTagContents.UnsignedInteger },
            { EBMLTagType.FlagForced, EBMLTagContents.UnsignedInteger },
            { EBMLTagType.FlagLacing, EBMLTagContents.UnsignedInteger },
            { EBMLTagType.DefaultDuration, EBMLTagContents.UnsignedInteger },
            { EBMLTagType.Name, EBMLTagContents.UTF8String },
            { EBMLTagType.Language, EBMLTagContents.String },
            { EBMLTagType.CodecID, EBMLTagContents.String },
            { EBMLTagType.CodecPrivate, EBMLTagContents.Binary },
            { EBMLTagType.CodecName, EBMLTagContents.UTF8String },
            { EBMLTagType.Video, EBMLTagContents.Master },
            { EBMLTagType.FlagInterlaced, EBMLTagContents.UnsignedInteger },
            { EBMLTagType.StereoMode, EBMLTagContents.UnsignedInteger },
            { EBMLTagType.AlphaMode, EBMLTagContents.UnsignedInteger },
            { EBMLTagType.PixelWidth, EBMLTagContents.UnsignedInteger },
            { EBMLTagType.PixelHeight, EBMLTagContents.UnsignedInteger },
            { EBMLTagType.PixelCropBottom, EBMLTagContents.UnsignedInteger },
            { EBMLTagType.PixelCropTop, EBMLTagContents.UnsignedInteger },
            { EBMLTagType.PixelCropLeft, EBMLTagContents.UnsignedInteger },
            { EBMLTagType.PixelCropRight, EBMLTagContents.UnsignedInteger },
            { EBMLTagType.DisplayWidth, EBMLTagContents.UnsignedInteger },
            { EBMLTagType.DisplayHeight, EBMLTagContents.UnsignedInteger },
            { EBMLTagType.DisplayUnit, EBMLTagContents.UnsignedInteger },
            { EBMLTagType.AspectRatioType, EBMLTagContents.UnsignedInteger },
            { EBMLTagType.Audio, EBMLTagContents.Master },
            { EBMLTagType.SamplingFrequency, EBMLTagContents.Float },
            { EBMLTagType.OutputSamplingFrequency, EBMLTagContents.Float },
            { EBMLTagType.Channels, EBMLTagContents.UnsignedInteger },
            { EBMLTagType.BitDepth, EBMLTagContents.UnsignedInteger },
            { EBMLTagType.Cues, EBMLTagContents.Master },
            { EBMLTagType.CuePoint, EBMLTagContents.Master },
            { EBMLTagType.CueTime, EBMLTagContents.UnsignedInteger },
            { EBMLTagType.CueTrackPositions, EBMLTagContents.Master },
            { EBMLTagType.CueTrack, EBMLTagContents.UnsignedInteger },
            { EBMLTagType.CueClusterPosition, EBMLTagContents.UnsignedInteger },
            { EBMLTagType.CueBlockNumber, EBMLTagContents.UnsignedInteger }
        };

        public EBMLTag(long position, long size, EBMLTagType type, EBMLTagContents contents)
        {
            _position = position;
            _size = size;
            _type = type;
            _contentType = contents;

            Children = new List<EBMLTag>();
        }

        public EBMLTag FindTypeInChildren(EBMLTagType type)
        {
            var results = Children.Where(x => x.Type == type);
            return results.Count() > 0 ? results.First() : null;
        }

        public EBMLTag[] FindTypesInChildren(EBMLTagType type)
        {
            return Children.Where(x => x.Type == type).ToArray();
        }

        public static EBMLTagType TypeFromID(int id)
        {
            if (Tags.ContainsKey(id))
                return Tags[id];
            return EBMLTagType.None;
        }

        public static EBMLTagContents ContentTypeFromID(int id)
        {
            EBMLTagType type = TypeFromID(id);
            if (Contents.ContainsKey(type))
                return Contents[type];
            return EBMLTagContents.Binary;
        }
    }

    public enum EBMLTagType
    {
        None,
        EBML,
        EBMLVersion,
        EBMLReadVersion,
        EBMLMaxIDLength,
        EBMLMaxSizeLength,
        DocType,
        DocTypeVersion,
        DocTypeReadVersion,
        Void,
        Segment,
        SeekHead,
        Seek,
        SeekID,
        SeekPosition,
        Info,
        TimecodeScale,
        Duration,
        DateUTC,
        Title,
        MuxingApp,
        WritingApp,
        Cluster,
        Timecode,
        PrevSize,
        SimpleBlock,
        BlockGroup,
        Block,
        BlockDuration,
        ReferenceBlock,
        LaceNumber,
        Tracks,
        TrackEntry,
        TrackNumber,
        TrackUID,
        TrackType,
        FlagEnabled,
        FlagDefault,
        FlagForced,
        FlagLacing,
        DefaultDuration,
        Name,
        Language,
        CodecID,
        CodecPrivate,
        CodecName,
        Video,
        FlagInterlaced,
        StereoMode,
        AlphaMode,
        PixelWidth,
        PixelHeight,
        PixelCropBottom,
        PixelCropTop,
        PixelCropLeft,
        PixelCropRight,
        DisplayWidth,
        DisplayHeight,
        DisplayUnit,
        AspectRatioType,
        FrameRate,
        Audio,
        SamplingFrequency,
        OutputSamplingFrequency,
        Channels,
        BitDepth,
        Cues,
        CuePoint,
        CueTime,
        CueTrackPositions,
        CueTrack,
        CueClusterPosition,
        CueRelativePosition,
        CueDuration,
        CueBlockNumber,
        Tags,
        Tag,
        Targets,
        TargetTypeValue,
        TargetType,
        TagTrackUID,
        SimpleTag,
        TagName,
        TagLanguage,
        TagDefault,
        TagString,
        TagBinary
    }

    public enum EBMLTagContents
    {
        Master,
        SignedInteger,
        UnsignedInteger,
        String,
        UTF8String,
        Binary,
        Float,
        Date
    }
}
