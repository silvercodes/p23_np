using System.Net.Sockets;
using MTP.PayloadBase;

namespace MTP;

public class ProtoMessageBuilder
{
    private NetworkStream netStream;
    private MemoryStream memStream;
    public ProtoMessageBuilder(NetworkStream netStream)
    {
        this.netStream = netStream;
    }
    public ProtoMessage<T> Receive<T>()
        where T : IPayload
    {
        int readingSize = ConvertToInt(ReadBytes(ProtoMessage<T>.MESSAGE_LEN_LABLE_SIZE));

        memStream = new MemoryStream(readingSize);
        memStream.Write(ReadBytes(readingSize), 0, readingSize);
        memStream.Position = 0;

        ProtoMessage<T> pm = new ProtoMessage<T>();

        using StreamReader sr = new StreamReader(memStream);

        ExtractMetadata(pm, sr);
        ExtractPayloadStream(pm);

        pm.InjectPayloadBuilder();

        return pm;
    }

    private void ExtractMetadata<T>(ProtoMessage<T> pm, StreamReader sr)
        where T : IPayload
    {
        sr.BaseStream.Position = 0;

        pm.Action = sr.ReadLine();

        string? headerLine;
        while(! string.IsNullOrEmpty(headerLine = sr.ReadLine()))
            pm.SetHeader(headerLine);
    }

    private void ExtractPayloadStream<T>(ProtoMessage<T> pm)
        where T : IPayload
    {
        int payloadLength = pm.PaylodLength;

        memStream.Seek(-payloadLength, SeekOrigin.End);

        pm.PayloadStream = new MemoryStream(payloadLength);
        memStream.CopyTo(pm.PayloadStream);
        pm.PayloadStream.Position = 0;
    }

    private byte[] ReadBytes(int count)
    {
        byte[] bytes = new byte[count];
        netStream.ReadExactly(bytes, 0, count);

        return bytes;
    }

    private int ConvertToInt(byte[] bytes)
    {
        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);

        return BitConverter.ToInt32(bytes, 0);
    }
}
