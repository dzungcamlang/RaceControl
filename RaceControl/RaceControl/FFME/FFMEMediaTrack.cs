using RaceControl.Common.Interfaces;
using Unosquare.FFME.Common;

namespace RaceControl.FFME
{
    public class FFMEMediaTrack : IMediaTrack
    {
        public FFMEMediaTrack(StreamInfo streamInfo)
        {
            Id = streamInfo.StreamIndex;
            Name = GetName(streamInfo);
        }

        public int Id { get; }

        public string Name { get; }

        private static string GetName(StreamInfo streamInfo)
        {
            if (streamInfo.Metadata != null && streamInfo.Metadata.TryGetValue("comment", out var comment))
            {
                return comment;
            }

            return streamInfo.Language;
        }
    }
}