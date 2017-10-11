using Marshal = System.Runtime.InteropServices.Marshal;
using System.Collections.Generic;
using System.IO;
using System;

namespace Chunk.Buffer
{
    /// <summary>크기 조절이 자유로운 버퍼입니다.</summary>
    public class SBuffer : IDisposable
    {
        private const int DefaultSize = 4096;
        private readonly int BlockSize;
        private List<IntPtr> Blocks;
        private int BufferLength;

        #region "New"
        /// <summary>새 버퍼를 생성합니다.</summary>
        /// <param name="Size">버퍼의 블록 크기입니다.</param>
        public SBuffer(int Block = DefaultSize) {
            Blocks = new List<IntPtr>();
            BlockSize = Block;
            BufferLength = 0;
        }

        /// <summary>스트림으로부터 새 버퍼를 생성합니다.</summary>
        /// <param name="Stream">버퍼를 생성할 스트림입니다.</param>
        /// <param name="Length">스트림으로부터 읽어들일 크기입니다.</param>
        /// <param name="Size">버퍼의 블록 크기입니다.</param>
        public SBuffer(Stream Stream, int Length, int Block = DefaultSize) : this(Block) {
            var Buffer = new byte[BlockSize];
            int Read = BlockSize;
            do {
                if(Length < BlockSize) Read = Length;
                if((Read = Stream.Read(Buffer, 0, Read)) == 0) return;

                var Ptr = Alloc();
                Marshal.Copy(Buffer, 0, Ptr, Read);
                Blocks.Add(Ptr);

                BufferLength += Read;
            } while(Read == BlockSize);
        }
        #endregion

        #region "Dispose"
        ~SBuffer() => Dispose();

        /// <summary>버퍼의 모든 메모리를 해제합니다.</summary>
        public void Dispose() {
            lock(Blocks) {  //In case
                foreach(var p in Blocks)
                    Free(p);
                Blocks.Clear();
                BufferLength = 0;
            }
        }
        #endregion

        #region "Memory"
        private IntPtr Alloc()
            => Marshal.AllocCoTaskMem(BlockSize);

        private void Free(IntPtr Ptr)
            => Marshal.FreeCoTaskMem(Ptr);
        #endregion

        #region "Length"
        /// <summary>버퍼의 크기를 반환합니다.</summary>
        public int Length => BufferLength;

        /// <summary>버퍼의 크기를 재설정합니다.</summary>
        /// <param name="value">새로 설정할 버퍼의 크기입니다.</param>
        public void SetLength(int value) {
            if(value < 0) throw new ArgumentOutOfRangeException();
            lock(Blocks) {  //In case
                int IdxN = GetIndex(value);
                int IdxC = Blocks.Count;

                //Increase
                if(IdxN > IdxC)
                    for(int i = IdxC; i < IdxN; i++)
                        Blocks.Add(Alloc());

                //Decrease
                if(IdxN < IdxC) {
                    for(int i = IdxN; i < IdxC; i++)
                        Free(Blocks[i]);
                    Blocks.RemoveRange(IdxN, IdxC - IdxN);
                }

                BufferLength = value;
            }
        }
        private int GetLast(int Size) {
            int Last = Size % BlockSize;
            return Last == 0 ? BlockSize : Last;
        }

        private int GetIndex(int Size)
            => (Size + BlockSize - 1) / BlockSize;
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
            int IdxS = Position / BlockSize;
            int IdxE = GetIndex(Position + Count) - 1;
            int PosS = Position % BlockSize;

            if(IdxS == IdxE)
                Marshal.Copy(Blocks[IdxS] + PosS, Buffer, Offset, Count);
            else {
                int Size = BlockSize - PosS;
                Marshal.Copy(Blocks[IdxS] + PosS, Buffer, Offset, Size);
                Offset += Size;

                for(int i = IdxS + 1; i < IdxE; i++) {
                    Marshal.Copy(Blocks[i], Buffer, Offset, BlockSize);
                    Offset += BlockSize;
                }

                Marshal.Copy(Blocks[IdxE], Buffer, Offset, GetLast(Position + Count));
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
            int IdxS = Position / BlockSize;
            int IdxE = GetIndex(Position + Count) - 1;
            int PosS = Position % BlockSize;

            if(IdxS == IdxE)
                Marshal.Copy(Buffer, Offset, Blocks[IdxS] + PosS, Count);
            else {
                int Size = BlockSize - PosS;
                Marshal.Copy(Buffer, Offset, Blocks[IdxS] + PosS, Size);
                Offset += Size;

                for(int i = IdxS + 1; i < IdxE; i++) {
                    Marshal.Copy(Buffer, Offset, Blocks[i], BlockSize);
                    Offset += BlockSize;
                }

                Marshal.Copy(Buffer, Offset, Blocks[IdxE], GetLast(Position + Count));
            }
        }

        /// <summary>스트림에 버퍼의 내용을 작성합니다.</summary>
        /// <param name="Stream">버퍼의 내용을 작성할 스트림입니다.</param>
        public void WriteTo(Stream Stream) {
            var Buffer = new byte[BlockSize];
            int Size = GetLast(BufferLength);
            int Last = Blocks.Count - 1;
            for(int i = 0; i < Last; i++) {
                Marshal.Copy(Blocks[i], Buffer, 0, BlockSize);
                Stream.Write(Buffer, 0, BlockSize);
            }
            Marshal.Copy(Blocks[Last], Buffer, 0, Size);
            Stream.Write(Buffer, 0, Size);
        }
        #endregion
    }
}
