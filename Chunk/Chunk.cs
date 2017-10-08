using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Chunk
{
    /// <summary>Chunk를 관리하는 클래스입니다.</summary>
    public class Chunk : IList<Section> {
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

        /// <summary>스트림에 Chunk를 작성합니다.</summary>
        /// <param name="Stream">Chunk를 작성할 스트림입니다.</param>
        public void WriteTo(Stream Stream) {
            foreach(var s in Sections)
                s.WriteTo(Stream);
        }

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
        public IEnumerator this[string Name] {
            get {
                foreach(var s in Sections)
                    if(s.Name == Name) yield return s;
            }
        }

    }
}
