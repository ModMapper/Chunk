using System.Text;
using System.IO;
using System;

namespace Chunk
{
    /// <summary>Chunk의 구휙을 관리하는 클래스입니다.</summary>
    public class Section {
        private byte[] SName = new byte[4]; //4 Bytes
        private MemoryStream SData;
        
        /// <summary>새 구휙을 생성합니다.</summary>
        /// <param name="Name">생성할 구휙의 이름입니다.</param>
        public Section(string Name) {
            this.Name = Name;
            SData = new MemoryStream();
        }

        /// <summary>배열로부터 새 구휙을 생성합니다.</summary>
        /// <param name="Name">생성할 구휙의 이름입니다.</param>
        /// <param name="Data">구휙의 데이터입니다.</param>
        public Section(string Name, byte[] Data) {
            this.Name = Name;
            SData = new MemoryStream();
            SData.Write(Data, 0, Data.Length);
            SData.Position = 0;
        }

        /// <summary>구휙의 이름을 가져오거나 작성합니다.</summary>
        public string Name {
            get {
                return Encoding.ASCII.GetString(SName);
            }
            set {
                var Data = Encoding.ASCII.GetBytes(value);
                Array.Copy(Data, SName, Data.Length < 4 ? Data.Length : 4);
            }
        }

        /// <summary>구휙의 크기를 가져옵니다.</summary>
        public int Size => (int)SData.Length;

        /// <summary>구휙의 스트림을 가져옵니다.</summary>
        public Stream Data => SData;


        #region "Serialize"
        // Section Format
        // [4 Bytes] Name
        // [4 Bytes] Size
        // [Size] Data

        /// <summary>Deserialize Section</summary>
        /// <param name="Stream">Read Stream</param>
        internal void Deserialize(Stream Stream) {
            var Data = new byte[8];
            if(Stream.Read(Data, 0, 8) < 8) return;
            Array.Copy(Data, SName, 4);
            int Size = BitConverter.ToInt32(Data, 4);

            SData.SetLength(Size);
            Stream.Read(SData.GetBuffer(), 0, Size);
        }

        /// <summary>Serialize Section</summary>
        /// <param name="Stream">Write Stream</param>
        internal void Serialize(Stream Stream) {
            Stream.Write(SName, 0, 4);
            Stream.Write(BitConverter.GetBytes(Size), 0, 4);
            SData.WriteTo(Stream);
        }
        #endregion
    }
}
