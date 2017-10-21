using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Collections;
using System;

namespace Chunk.Buffer
{
    /// <summary>버퍼의 구조체를 관리하는 클래스입니다.</summary>
    /// <typeparam name="TStruct">관리할 버퍼의 구조체입니다.</typeparam>
    public class SBufferStruct<TStruct>: IList<TStruct> where TStruct : struct {
        private SBuffer Buffer;
        private int StructSize;

        /// <summary>버퍼의 구조체를 관리하는 클래스를 생성합니다.</summary>
        /// <param name="Buf">구조체를 관리할 버퍼입니다.</param>
        public SBufferStruct(SBuffer Buf) {
            Buffer = Buf;
            StructSize = Marshal.SizeOf<TStruct>();
        }

        #region "Properties"
        /// <summary>구조체의 갯수를 반환합니다.</summary>
        public int Count => (Buffer.Length + StructSize - 1) / StructSize;

        // ICollection Implements
        bool ICollection<TStruct>.IsReadOnly => false;

        /// <summary>해당 인덱스의 구조체를 가져오거나 설정합니다.</summary>
        /// <param name="Index">구조체의 인덱스입니다.</param>
        /// <returns>해당 인덱스의 구조체입니다.</returns>
        public unsafe TStruct this[int Index] {
            get {
                if(Index < 0) throw new IndexOutOfRangeException();
                var Buf = new byte[StructSize];
                Buffer.Read(Index * StructSize, Buf, 0, StructSize);
                fixed(byte* pBuf = Buf)
                    return Marshal.PtrToStructure<TStruct>(new IntPtr(pBuf));
            }
            set {
                if(Index < 0) throw new IndexOutOfRangeException();
                var Buf = new byte[StructSize];
                fixed (byte* pBuf = Buf)
                    Marshal.StructureToPtr(value, new IntPtr(pBuf), false);
                Buffer.Write(Index * StructSize, Buf, 0, StructSize);
            }
        }
        #endregion

        #region "Add, Remove"
        /// <summary>버퍼의 마지막에 구조체를 추가니다.</summary>
        /// <param name="Item">마지막에 추가할 구조체입니다.</param>
        public void Add(TStruct Item)
            => this[Count] = Item;
        
        /// <summary>해당 인덱스에 구조체를 삽입합니다.</summary>
        /// <param name="Index">구조체를 삽입할 인덱스입니다.</param>
        /// <param name="Item">인덱스에 삽입할 구조체입니다.</param>
        public void Insert(int Index, TStruct Item) {
            if(Count < Index) throw new IndexOutOfRangeException();
            for(int i = Count; i < Index; i--)
                this[i] = this[i - 1];
            this[Index] = Item;
        }
        
        /// <summary>해당 내용의 구조체를 모두 제거합니다.</summary>
        /// <param name="Item">제거할 구조체입니다.</param>
        /// <returns>하나 이상의 구조체가 제거되었는지 여부입니다.</returns>
        public bool Remove(TStruct Item) {
            int D = 0;
            for(int i = 0; i < Count; i++) {
                var S = this[i];
                if(S.Equals(Item)) D++;
                else if(D != 0) this[i - D] = this[i];
            }
            Buffer.SetLength((Count - D) * StructSize);
            return D != 0;
        }

        /// <summary>해당 인덱스의 구조체를 제거합니다.</summary>
        /// <param name="Index">제거할 구조체의 인덱스입니다</param>
        public void RemoveAt(int Index) {
            if(Index < 0) throw new IndexOutOfRangeException();
            if(Count <= Index) throw new IndexOutOfRangeException();
            for(int i = Index + 1; i < Count; i++)
                this[i - 1] = this[i];
            Buffer.SetLength((Count - 1) * StructSize);
        }

        /// <summary>버퍼의 모든 구조체를 제거합니다.</summary>
        public void Clear()
            => Buffer.SetLength(0);
        #endregion

        #region "List"
        /// <summary>해당 구조체가 존재 여부를 반환합니다.</summary>
        /// <param name="Item">찾을 구조체입니다.</param>
        /// <returns>해당 구조체의 존재 여부입니다.</returns>
        public bool Contains(TStruct Item)
            => IndexOf(Item) != -1;

        /// <summary>해당 내용의 첫 구조체의 인덱스를 반환합니다.</summary>
        /// <param name="Item">찾을 구조체입니다.</param>
        /// <returns>해당 구조체의 인덱스입니다.</returns>
        public int IndexOf(TStruct Item) {
            for(int i = 0; i < Count; i++)
                if(this[i].Equals(Item)) return i;
            return -1;
        }

        /// <summary>배열에 해당 버퍼의 내용을 복사합니다.</summary>
        /// <param name="array">내용을 복사할 배열입니다.</param>
        /// <param name="arrayIndex">배열의 인덱스입니다.</param>
        public void CopyTo(TStruct[] array, int arrayIndex) {
            for(int i = 0; i < Count; i++)
                array[arrayIndex + i] = this[i]; 
        }

        /// <summary>해당 버퍼의 구조체를 열거합니다.</summary>
        /// <returns>해당 버퍼의 구조체의 열거자입니다.</returns>
        public IEnumerator<TStruct> GetEnumerator() {
            for(int i = 0; i < Count; i++)
                yield return this[i];
        }

        //IEnumerator Implements
        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
        #endregion
    }
}
