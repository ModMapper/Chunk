using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;

namespace Chunk.Buffer
{
    /// <summary>여러개의 버퍼로부터 구조체를 관리하는 클래스입니다.</summary>
    /// <typeparam name="TStruct">관리할 버퍼들의 구조체입니다.</typeparam>
    public class StructList<TStruct> : IList<TStruct> where TStruct : struct {
        protected List<SBufferStruct<TStruct>> Lists;

        /// <summary>버퍼로부터 새 구조체 목록을 생성합니다.</summary>
        /// <param name="Buffer">구조체를 관리할 버퍼입니다.</param>
        public StructList(SBuffer Buffer) {
            Lists = new List<SBufferStruct<TStruct>>();
            Lists.Add(new SBufferStruct<TStruct>(Buffer));
        }

        /// <summary>버퍼의 열거자로부터 새 구조체 목록을 생성합니다.</summary>
        /// <param name="Buffer">구조체를 관리할 버퍼들의 열거자입니다.</param>
        public StructList(IEnumerable<SBuffer> Buffers) {
            Lists = new List<SBufferStruct<TStruct>>(
                Buffers.Select((s) => new SBufferStruct<TStruct>(s)));
        }

        #region "Proporties"
        /// <summary>구조체의 갯수를 반환합니다.</summary>
        public int Count => Lists.Sum((s) => s.Count);

        //ICollection Implements
        bool ICollection<TStruct>.IsReadOnly => false;

        /// <summary>해당 인덱스의 구조체를 가져오거나 설정합니다.</summary>
        /// <param name="Index">구조체의 인덱스입니다.</param>
        /// <returns>해당 인덱스의 구조체입니다.</returns>
        public TStruct this[int Index] {
            get {
                foreach(var s in Lists) {
                    if(Index < s.Count) {
                        return s[Index];
                    }
                    Index -= s.Count;
                }
                throw new IndexOutOfRangeException();
            }
            set {
                foreach(var s in Lists) {
                    if(Index < s.Count) {
                        s[Index] = value;
                        return;
                    }
                    Index -= s.Count;
                }
                throw new IndexOutOfRangeException();
            }
        }
        #endregion

        #region "Add, Remove"
        /// <summary>목록의 마지막에 구조체를 추가니다.</summary>
        /// <param name="Item">마지막에 추가할 구조체입니다.</param>
        public void Add(TStruct Item)
            => Lists.Last().Add(Item);

        /// <summary>해당 인덱스에 구조체를 삽입합니다.</summary>
        /// <param name="Index">구조체를 삽입할 인덱스입니다.</param>
        /// <param name="Item">인덱스에 삽입할 구조체입니다.</param>
        public void Insert(int Index, TStruct Item) {
            foreach(var s in Lists) {
                if(Index <= s.Count) {
                    s.Insert(Index, Item);
                    return;
                }
                Index -= s.Count;
            }
            throw new IndexOutOfRangeException();
        }

        /// <summary>해당 내용의 구조체를 모두 제거합니다.</summary>
        /// <param name="Item">제거할 구조체입니다.</param>
        /// <returns>하나 이상의 구조체가 제거되었는지 여부입니다.</returns>
        public bool Remove(TStruct Item)
            => !Lists.All((s) => !s.Remove(Item));
        
        /// <summary>해당 인덱스의 구조체를 제거합니다.</summary>
        /// <param name="Index">제거할 구조체의 인덱스입니다</param>
        public void RemoveAt(int Index) {
            foreach(var s in Lists) {
                if(Index < s.Count) {
                    s.RemoveAt(Index);
                    return;
                }
                Index -= s.Count;
            }
            throw new IndexOutOfRangeException();
        }

        /// <summary>버퍼의 모든 구조체를 제거합니다..</summary>
        public void Clear() {
            foreach(var s in Lists) s.Clear();
        }
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
            int Index = 0;
            foreach(var s in Lists) {
                int i = s.IndexOf(Item);
                if(i != -1) return Index + i;
                Index += s.Count;
            }
            return -1;
        }

        /// <summary>배열에 해당 버퍼의 내용을 복사합니다.</summary>
        /// <param name="array">내용을 복사할 배열입니다.</param>
        /// <param name="arrayIndex">배열의 인덱스입니다.</param>
        public void CopyTo(TStruct[] array, int arrayIndex) {
            foreach(var i in this)
                array[arrayIndex++] = i;
        }

        /// <summary>해당 목록의 구조체를 열거합니다.</summary>
        /// <returns>해당 목록의 구조체의 열거자입니다.</returns>
        public IEnumerator<TStruct> GetEnumerator() {
            foreach(var s in Lists)
                foreach(var i in s)
                    yield return i;
        }
        
        //IEnumerator Implements
        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
    #endregion
}
