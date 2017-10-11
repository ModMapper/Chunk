using Chunk.Buffer;

using System.Text;
using System.IO;
using System;

namespace Chunk
{
    /// <summary>Chunk의 구휙을 관리하는 클래스입니다.</summary>
    public class Section : IDisposable {
        private const int BufferSize = 4096;
        private SBuffer SData;
        private byte[] SName;

        /// <summary>해당 이름의 구휙을 생성합니다.</summary>
        /// <param name="Name">생성할 구휙의 이름입니다.</param>
        public Section(string Name) {
            SData = new SBuffer(BufferSize);
            var CName = Encoding.ASCII.GetBytes(Name);
            Array.Resize(ref CName, 4);
            SName = CName;
        }
        
        /// <summary>스트림으로부터 해당 구휙을 읽어들입니다.</summary>
        /// <param name="Stream">해당 구휙을 읽어들일 스트림입니다.</param>
        public Section(Stream Stream) { //Deserialize
            var Reader = new BinaryReader(Stream);
            SName = Reader.ReadBytes(4);    //4Byte Char
            int Size = Reader.ReadInt32();  //4Byte Int
            if(0 < Size)    //Read Data
                SData = new SBuffer(Stream, Size, BufferSize);
            else            //Jump Offset
                Stream.Seek(Size, SeekOrigin.Current);
        }

        /// <summary>스트림에 해당 구휙을 작성합니다.</summary>
        /// <param name="Stream">해당 구휙을 작성할 스트림입니다.</param>
        public void WriteTo(Stream Stream) {
            Stream.Write(SName, 0, 4);
            Stream.Write(BitConverter.GetBytes(SData.Length), 0, 4);
            SData.WriteTo(Stream);
        }

        /// <summary>구휙의 이름을 가져오거나 설정합니다.</summary>
        public string Name {    //4Byte Char
            get => Encoding.ASCII.GetString(SName);
            set {
                var CName = Encoding.ASCII.GetBytes(value);
                Array.Resize(ref CName, 4);
                SName = CName;
            }
        }

        /// <summary>구휙을 읽거나 작성할 수 있는 스트림을 가져옵니다.</summary>
        /// <returns>구휙의 읽기 및 작성 스트림입니다.</returns>
        public Stream GetStream()
            => new SBufferStream(SData);

        /// <summary>구휙의 데이터를 모두 해제합니다.</summary>
        public void Dispose()
            => SData.Dispose();
    }
}
