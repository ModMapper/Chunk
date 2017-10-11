using System.Collections.Generic;
using System.Collections;
using System.IO;
using System;

namespace Chunk
{
    /// <summary>Chunk를 관리하는 클래스입니다.</summary>
    public class Chunk : IList<Section>, IDisposable {
        private List<Section> Sections;

        /// <summary>빈 Chunk를 생성합니다.</summary>
        public Chunk() {
            Sections = new List<Section>();
        }

        /// <summary>스트림으로부터 Chunk를 읽어들입니다.</summary>
        /// <param name="Stream">Chunk를 읽어들일 스트림입니다.</param>
        public Chunk(Stream Stream) {
            Sections = new List<Section>();
            while(Stream.Position + 8 < Stream.Length)
                Sections.Add(new Section(Stream));
        }

        /// <summary>Chunk의 모든 데이터를 해제합니다.</summary>
        public void Dispose() {
            foreach(var s in Sections)
                s.Dispose();
            Sections.Clear();
        }

        /// <summary>스트림에 Chunk를 작성합니다.</summary>
        /// <param name="Stream">Chunk를 작성할 스트림입니다.</param>
        public void WriteTo(Stream Stream) {
            foreach(var s in Sections)
                s.WriteTo(Stream);
        }

        #region "Properties"
        /// <summary>Chunk 구휙의 갯수를 가져옵니다.</summary>
        public int Count => Sections.Count;

        // ICollection Implements
        bool ICollection<Section>.IsReadOnly => false;

        /// <summary>해당 인덱스의 구휙을 가져오거나 설정합니다.</summary>
        /// <param name="Index">구휙의 인덱스입니다.</param>
        /// <returns>해당 인덱스의 구휙입니다.</returns>
        public Section this[int Index] {
            get => Sections[Index];
            set => Sections[Index] = value;
        }

        /// <summary>해당 이름의 구휙을 열거합니다.</summary>
        /// <param name="Name">구휙의 이름입니다.</param>
        /// <returns>해당 이름의 구휙들의 열거자입니다.</returns>
        public IEnumerator<Section> this[string Name] {
            get {
                foreach(var s in Sections)
                    if(s.Name == Name) yield return s;
            }
        }
        #endregion

        #region "Add, Insert"
        /// <summary>목록의 끝에 새 구휙을 추가합니다.</summary>
        /// <param name="Name">생성할 구휙의 이름입니다.</param>
        /// <returns>생성된 구휙입니다.</returns>
        public Section Add(string Name) {
            var Item = new Section(Name);
            Add(Item);
            return Item;
        }

        /// <summary>목록의 끝에 구휙을 추가합니다.</summary>
        /// <param name="Item">추가할 구휙입니다.</param>
        public void Add(Section Item)
            => Sections.Add(Item);

        /// <summary>해당 인덱스에 새 구휙을 추가합니다.</summary>
        /// <param name="Index">구휙을 삽입할 인덱스입니다.</param>
        /// <param name="Name">생성할 구휙의 이름입니다.</param>
        /// <returns>생성된 구휙입니다.</returns>
        public Section Insert(int Index, string Name) {
            var Item = new Section(Name);
            Insert(Index, Item);
            return Item;
        }

        /// <summary>해당 인덱스에 구휙을 삽입합니다.</summary>
        /// <param name="Index">구휙을 삽입할 인덱스입니다.</param>
        /// <param name="Item">삽입할 구휙입니다.</param>
        public void Insert(int Index, Section Item)
            => Sections.Insert(Index, Item);
        #endregion

        #region "Remove"
        /// <summary>해당 이름의 구휙을 모두 제거합니다.</summary>
        /// <param name="Name">제거할 구휙의 이름입니다.</param>
        public void Remove(string Name)
            => Sections.RemoveAll((s) => s.Name == Name);

        /// <summary>해당 인덱스의 구휙을 제거합니다.</summary>
        /// <param name="Index">제거할 구휙의 인덱스입니다.</param>
        public void Remove(int Index)
            => Sections.RemoveAt(Index);

        /// <summary>맨 처음 발견되는 해당 구휙을 제거합니다.</summary>
        /// <param name="Item">제거할 구휙입니다.</param>
        /// <returns>구휙의 제거 여부입니다.</returns>
        public bool Remove(Section Item)
            => Sections.Remove(Item);

        // IList Implements
        void IList<Section>.RemoveAt(int index)
            => Sections.RemoveAt(index);

        /// <summary>모든 구휙을 제거합니다.</summary>
        public void Clear()
            => Sections.Clear();
        #endregion

        #region "Find"
        /// <summary>해당 이름의 구휙의 인덱스를 찾습니다.</summary>
        /// <param name="Name">인덱스를 찾을 구휙의 이름입니다.</param>
        /// <returns>찾아낸 구휙의 인덱스입니다.</returns>
        public int IndexOf(string Name)
            => Sections.FindIndex((s) => s.Name == Name);

        /// <summary>해당 구휙의 인덱스를 찾습니다.</summary>
        /// <param name="Item">인덱스를 찾을 구휙입니다.</param>
        /// <returns>찾아낸 구휙의 인덱스입니다.</returns>
        public int IndexOf(Section Item)
            => Sections.IndexOf(Item);

        /// <summary>해당 이름의 구휙의 마지막 인덱스를 찾습니다.</summary>
        /// <param name="Name">인덱스를 찾을 구휙의 이름입니다.</param>
        /// <returns>찾아낸 구휙의 인덱스입니다.</returns>
        public int LastIndexOf(string Name)
            => Sections.FindLastIndex((s) => s.Name == Name);

        /// <summary>해당 구휙의 마지막 인덱스를 찾습니다.</summary>
        /// <param name="Item">인덱스를 찾을 구휙입니다.</param>
        /// <returns>찾아낸 구휙의 인덱스입니다.</returns>
        public int LastIndexOf(Section Item)
            => Sections.LastIndexOf(Item);

        /// <summary>해당 이름의 구휙을 모두 찾습니다.</summary>
        /// <param name="Name">찾을 구휙의 이름입니다.</param>
        /// <returns>구휙의 인덱스가 들어간 열거자입니다.</returns>
        public IEnumerator<int> Find(string Name) {
            for(int i = 0; i < Sections.Count; i++)
                if(Sections[i].Name == Name) yield return i;
        }

        /// <summary>해당 이름의 구휙이 존재하는지에 여부를 반환합니다.</summary>
        /// <param name="Name">존재 여부를 찾을 구휙의 이름입니다.</param>
        /// <returns>구휙의 존재 여부입니다.</returns>
        public bool Contains(string Name)
            => IndexOf(Name) != -1;

        /// <summary>해당 구휙이 존재하는지에 여부를 반환합니다.</summary>
        /// <param name="Name">존재 여부를 찾을 구휙입니다.</param>
        /// <returns>구휙의 존재 여부입니다.</returns>
        public bool Contains(Section Item)
            => Sections.Contains(Item);
        #endregion

        #region "List"
        /// <summary>Chunk를 배열에 복사합니다.</summary>
        /// <returns>값이 복사된 배열입니다.</returns>
        public Section[] ToArray()
             => Sections.ToArray();

        // ICollection Implements
        void ICollection<Section>.CopyTo(Section[] array, int arrayIndex)
            => Sections.CopyTo(array, arrayIndex);

        /// <summary>구휙을 순서대로 정렬합니다.</summary>
        public void Sort()
            => Sections.Sort((x, y) => string.Compare(x.Name, y.Name));

        /// <summary>구휙을 해당 우선순위대로 정렬합니다.</summary>
        /// <param name="Index">구휙을 정렬할 우선순위입니다.</param>
        public void Sort(string[] Index) {
            Sections.Sort((x, y) => {
                int ix = Array.IndexOf(Index, x.Name);
                int iy = Array.IndexOf(Index, y.Name);
                if(ix == -1 && iy == -1) x.Name.CompareTo(y.Name);
                if(ix == -1) return 1;
                if(iy == -2) return -1;
                return ix.CompareTo(iy);
            });
        }
        #endregion

        #region "Enumerable"
        public IEnumerator<Section> GetEnumerator() {
            throw new System.NotImplementedException();
        }

        // IEnumerable Implements
        IEnumerator IEnumerable.GetEnumerator() {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}
