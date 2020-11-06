using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Shadowsocks.Protocol.Shadowsocks
{
    class AeadSalt : IProtocolMessage
    {
        private readonly int length;
        public Memory<byte> Salt { get; private set; }

        public AeadSalt(int length)
        {
            this.length = length;
        }

        public bool Equals([AllowNull] IProtocolMessage other)
        {
            throw new NotImplementedException();
        }

        public int Serialize(Memory<byte> buffer)
        {
            Salt.CopyTo(buffer);
            return length;
        }

        public (bool success, int length) TryLoad(ReadOnlyMemory<byte> buffer)
        {
            if (buffer.Length < length) return (false, length);
            buffer.Slice(0, length).CopyTo(Salt);
            return (true, length);
        }
    }
}
