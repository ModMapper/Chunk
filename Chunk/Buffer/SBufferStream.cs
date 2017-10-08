using System.IO;
using System;

namespace Chunk.Buffer
{
    /// <summary>SBuffer의 읽기 및 작성 스트림입니다.</summary>
    public class SBufferStream : Stream
    {
        private SBuffer Buffer;
        private int Pos;

        /// <summary>SBuffer로부터 새 스트림을 생성합니다.</summary>
        /// <param name="SBuffer">새 스트림이 있는</param>
        internal SBufferStream(SBuffer SBuffer) {
            Buffer = SBuffer;
            Pos = 0;
        }

        /// <summary>스트림의 탐색을 지원하는지 나타내는 값을 반환합니다. 해당 스트림은 탐색이 가능합니다.</summary>
        public override bool CanSeek => true;
        /// <summary>스트림의 읽기를 지원하는지 나타내는 값을 반환합니다. 해당 스트림은 읽기가 가능합니다.</summary>
        public override bool CanRead => true;
        /// <summary>스트림의 작성을 지원하는지 나타내는 값을 반환합니다. 해당 스트림은 작성이 가능합니다.</summary>
        public override bool CanWrite => true;

        /// <summary>버퍼의 크기를 가져옵니다.</summary>
        public override long Length => Buffer.Length;

        /// <summary>현재 스트림 내의 위치를 가져오거나 설정합니다.</summary>
        public override long Position {
            get => Pos;
            set {
                if(value < 0) throw new IndexOutOfRangeException();
                if(Int32.MaxValue < value) throw new IndexOutOfRangeException();
                Pos = (int)value;
            }
        }

        /// <summary>현재 스트림 내의 위치를 설정합니다.</summary>
        /// <param name="offset">orgin 매개변수에 상대적인 바이트 오프셋입니다.</param>
        /// <param name="origin">새 위치를 가져오는데 사용되는 참조 위치를 나타내는 SeekOrigin 형식의 값입니다.</param>
        /// <returns>설정된 스트림의 위치입니다.</returns>
        public override long Seek(long offset, SeekOrigin origin) {
            switch(origin) {
            case SeekOrigin.Begin:
                Position = Length <= offset ? Length : offset;
                break;
            case SeekOrigin.Current:
                Position = Length <= Position + offset ? Length : Position + offset;
                break;
            case SeekOrigin.End:
                Position = Length <= offset ? 0 : Length - offset;
                break;
            default:
                throw new NotSupportedException();
            }
            return Position;
        }

        /// <summary>스트림 내의 모든 내용이 버퍼에 작성되게 합니다.</summary>
        public override void Flush() {}

        /// <summary>버퍼의 크기를 재설정합니다.</summary>
        /// <param name="value">재설정할 버퍼의 크기입니다.</param>
        public override void SetLength(long value) {
            if(value < 0) throw new ArgumentOutOfRangeException();
            if(Int32.MaxValue < value) throw new ArgumentOutOfRangeException();
            lock(Buffer) Buffer.SetLength((int)value);
        }

        /// <summary>스트림에서 바이트를 읽고 읽은 만큼 이동합니다.</summary>
        /// <param name="buffer">바이트를 읽어들일 버퍼입니다.</param>
        /// <param name="offset">바이트를 읽어들일 버퍼의 위치입니다.</param>
        /// <param name="count">읽어들일 바이트의 수 입니다.</param>
        /// <returns>읽어들인 바이트의 수 입니다</returns>
        public override int Read(byte[] buffer, int offset, int count) {
            count = Buffer.Read(Pos, buffer, offset, count);
            Pos += count;
            return count;
        }

        /// <summary>스트림에서 바이트를 작성하고 작성한 만큼 이동합니다.</summary>
        /// <param name="buffer">바이트를 작성할 버퍼입니다.</param>
        /// <param name="offset">바이트를 작성할 버퍼의 위치입니다.</param>
        /// <param name="count">작성할 바이트의 수 입니다.</param>
        public override void Write(byte[] buffer, int offset, int count) {
            Buffer.Write(Pos, buffer, offset, count);
            Pos += count;
        }
    }
}
