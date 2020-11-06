using Shadowsocks.Net.Crypto;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Shadowsocks.Protocol.Shadowsocks
{
    class AeadBlock : IProtocolMessage
    {
        public Memory<byte> Data;
        private readonly int tagLength;
        private readonly IAeadCrypto aead;
        private Memory<byte> nonce;

        public bool Equals([AllowNull] IProtocolMessage other)
        {
            throw new NotImplementedException();
        }

        public int Serialize(Memory<byte> buffer)
        {
            var len = Data.Length + 2 * tagLength + 2;
            if (buffer.Length < len)
                throw Util.BufferTooSmall(len, buffer.Length, nameof(buffer));
            Memory<byte> m = new byte[2];
            m.Span[0] = (byte)(Data.Length / 256);
            m.Span[1] = (byte)(Data.Length % 256);
            var len1 = aead.Encrypt(nonce.Span, m.Span, buffer.Span);
            Util.SodiumIncrement(nonce.Span);
            buffer = buffer.Slice(len1);
            aead.Encrypt(nonce.Span, Data.Span, buffer.Span);
            Util.SodiumIncrement(nonce.Span);
            return len;
        }

        public (bool success, int length) TryLoad(ReadOnlyMemory<byte> buffer)
        {
            if (buffer.Length < tagLength + 2) return (false, tagLength + 2);
            Memory<byte> m = new byte[2];
            var len = aead.Decrypt(nonce.Span, m.Span, buffer.Span);
            Util.SodiumIncrement(nonce.Span);
            if (len != 2) return (false, 0);

            var dataLength = m.Span[0] * 256 + m.Span[1];
            if (dataLength > 0x3fff) return (false, 0);

            if (buffer.Length < dataLength + 2 * tagLength + 2) return (false, dataLength + 2 * tagLength + 2);
            var dataBuffer = buffer.Slice(tagLength + 2);
            Data = new byte[dataLength];
            len = aead.Decrypt(nonce.Span, Data.Span, dataBuffer.Span);
            Util.SodiumIncrement(nonce.Span);
            if (len != dataLength) return (false, 0);
            return (true, dataLength + 2 * tagLength + 2);
        }
    }
}
