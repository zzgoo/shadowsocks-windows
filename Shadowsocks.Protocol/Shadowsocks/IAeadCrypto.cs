using System;
using System.Collections.Generic;
using System.Text;

namespace Shadowsocks.Protocol.Shadowsocks
{
    interface IAeadCrypto
    {
        int Encrypt(ReadOnlySpan<byte> nonce, ReadOnlySpan<byte> plain, Span<byte> cipher);
        int Decrypt(ReadOnlySpan<byte> nonce, Span<byte> plain, ReadOnlySpan<byte> cipher);
    }
}
