using System;
using System.Text;

namespace LAPP.NAudio.Wave
{
    /// <summary>
    /// Holds information about a RIFF file chunk
    /// </summary>
    public class RiffChunk
    {
        /// <summary>
        /// Creates a RiffChunk object
        /// </summary>
        public RiffChunk(int identifier, int length, long streamPosition)
        {
            Identifier = identifier;
            Length = length;
            StreamPosition = streamPosition;
        }

        /// <summary>
        /// The chunk identifier
        /// </summary>
        public int Identifier { get; private set; }

        /// <summary>
        /// The chunk identifier converted to a string
        /// </summary>
        public string IdentifierAsString
        {
            get
            {
                return Encoding.UTF8.GetString(BitConverter.GetBytes(Identifier));
            }
        }

        /// <summary>
        /// The chunk length
        /// </summary>
        public int Length { get; private set; }

        /// <summary>
        /// The stream position this chunk is located at
        /// </summary>
        public long StreamPosition { get; private set; }
    }
}