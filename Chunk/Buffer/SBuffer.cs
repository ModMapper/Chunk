using System.Collections.Generic;
using System.IO;
using System;

namespace Chunk.Buffer
{
    /// <summary>크기 조절이 자유로운 버퍼입니다.</summary>
    internal class SBuffer
    {
        private readonly int DataSize;
        private List<byte[]> Data;
        private int DataLength;

        /// <summary>새 버퍼를 생성합니다.</summary>
        /// <param name="Size">버퍼의 단위 크기입니다.</param>
        public SBuffer(int Size = 4096) {
            Data = new List<byte[]>();
            DataSize = Size;
        }

        /// <summary>스트림으로부터 새 버퍼를 생성합니다.</summary>
        /// <param name="Stream">버퍼를 생성할 스트림입니다.</param>
        /// <param name="Size">버퍼의 단위 크기입니다.</param>
        public SBuffer(Stream Stream, int Size = 4096) : this(Size) {
            byte[] Buffer;
            int Read;
            do {
                Buffer = new byte[Size];
                if((Read = Stream.Read(Buffer, 0, Size)) == 0) return;
                DataLength += Read;
                Data.Add(Buffer);
            } while(Read == Size);
        }

        /// <summary>스트림으로부터 새 버퍼를 생성합니다.</summary>
        /// <param name="Stream">버퍼를 생성할 스트림입니다.</param>
        /// <param name="Length">스트림에서 읽어들일 크기입니다.</param>
        /// <param name="Size">버퍼의 단위 크기입니다.</param>
        public SBuffer(Stream Stream, int Length, int Size = 4096) : this(Size) {
            byte[] Buffer;
            int Read = Size;
            do {
                Buffer = new byte[Size];
                if(Length < Size) Read = Length;
                if((Read = Stream.Read(Buffer, 0, Read)) == 0) return;
                DataLength += Read;
                Data.Add(Buffer);
            } while(Read == Size);
        }

        /// <summary>스트림에 버퍼의 내용을 작성합니다.</summary>
        /// <param name="Stream">버퍼의 내용을 작성할 스트림입니다.</param>
        public void WriteTo(Stream Stream) {
            int Last = Data.Count - 1;
            for(int i = 0; i < Last; i++)
                Stream.Write(Data[i], 0, DataSize);
            Stream.Write(Data[Last], 0, GetLast(DataLength));
        }

        #region "Length"
        /// <summary>버퍼의 크기를 반환합니다.</summary>
        public int Length => DataLength;

        /// <summary>버퍼의 크기를 재설정합니다.</summary>
        /// <param name="NewLength">새로 설정할 버퍼의 크기입니다.</param>
        public void SetLength(int NewLength) {
            if(NewLength < 0) throw new ArgumentOutOfRangeException();
            int IdxN = GetIndex(NewLength);
            int IdxC = Data.Count;

            //Upsize
            if(IdxN > IdxC)
                for(int i = IdxC; i < IdxN; i++)
                    Data.Add(new byte[DataSize]);

            //Downsize
            if(IdxN < IdxC)
                Data.RemoveRange(IdxN, IdxC - IdxN);

            DataLength = NewLength;
        }

        protected int GetLast(int Size) {
            int Last = Size % DataSize;
            return Last == 0 ? DataSize : Last;
        }

        protected int GetIndex(int Size)
            => (Size + DataSize - 1) / DataSize;
        #endregion

        #region "Read Write"
        /// <summary>버퍼의 내용을 읽습니다.</summary>
        /// <param name="Position">읽어들일 내용의 위치입니다.</param>
        /// <param name="Buffer">내용을 읽어들일 버퍼입니다.</param>
        /// <param name="Offset">버퍼의 위치입니다.</param>
        /// <param name="Count">읽어들일 내용의 크기입니다.</param>
        /// <returns>읽어들인 내용의 크기입니다.</returns>
        public int Read(int Position, byte[] Buffer, int Offset, int Count) {
            if(Length < Position + Count) Count = Length - Position;
            int IdxS = Position / DataSize;
            int IdxE = GetIndex(Position + Count) - 1;
            int PosS = Position % DataSize;

            if(IdxS == IdxE)
                Array.Copy(Data[IdxS], PosS, Buffer, Offset, Count);
            else {
                int Size = DataSize - PosS;
                Array.Copy(Data[IdxS], PosS, Buffer, Offset, Size);
                Offset += Size;

                for(int i = IdxS + 1; i < IdxE; i++) {
                    Array.Copy(Data[i], 0, Buffer, Offset, DataSize);
                    Offset += DataSize;
                }

                Array.Copy(Data[IdxE], 0, Buffer, Offset, GetLast(Position + Count));
            }
            return Count;
        }

        /// <summary>버퍼에 내용을 작성합니다.</summary>
        /// <param name="Position">작성할 내용의 위치입니다.</param>
        /// <param name="Buffer">작성할 내용의 버퍼입니다.</param>
        /// <param name="Offset">버퍼의 위치입니다.</param>
        /// <param name="Count">작성할 내용의 크기입니다.</param>
        public void Write(int Position, byte[] Buffer, int Offset, int Count) {
            if(Length < Position + Count) SetLength(Position + Count);
            int IdxS = Position / DataSize;
            int IdxE = GetIndex(Position + Count) - 1;
            int PosS = Position % DataSize;

            if(IdxS == IdxE)
                Array.Copy(Buffer, Offset, Data[IdxS], PosS, Count);
            else {
                int Size = DataSize - PosS;
                Array.Copy(Buffer, Offset, Data[IdxS], PosS, Size);
                Offset += Size;

                for(int i = IdxS + 1; i < IdxE; i++) {
                    Array.Copy(Buffer, Offset, Data[i], 0, DataSize);
                    Offset += DataSize;
                }

                Array.Copy(Buffer, Offset, Data[IdxE], 0, GetLast(Position + Count));
            }
        }
        #endregion
    }
}

