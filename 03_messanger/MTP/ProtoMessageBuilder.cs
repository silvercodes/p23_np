using System.Net.Sockets;

namespace MTP;

public class ProtoMessageBuilder
{
    private NetworkStream netStream;
    private MemoryStream memStream;
    public ProtoMessageBuilder(NetworkStream netStream)
    {
        this.netStream = netStream;
    }
    public ProtoMessage Receive()
    {
        int readingSize = ConvertToInt(ReadBytes(ProtoMessage.MESSAGE_LEN_LABLE_SIZE));

        memStream = new MemoryStream(readingSize);
        memStream.Write(ReadBytes(readingSize), 0, readingSize);
        memStream.Position = 0;

        ProtoMessage pm = new ProtoMessage();

        using StreamReader sr = new StreamReader(memStream);

        ExtractMetadata(pm, sr);
        ExtractPayloadStream(pm);

        return pm;
    }

    private void ExtractMetadata(ProtoMessage pm, StreamReader sr)
    {
        sr.BaseStream.Position = 0;

        pm.Action = sr.ReadLine();

        string? headerLine;
        while(! string.IsNullOrEmpty(headerLine = sr.ReadLine()))
            pm.SetHeader(headerLine);
    }

    private void ExtractPayloadStream(ProtoMessage pm)
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
