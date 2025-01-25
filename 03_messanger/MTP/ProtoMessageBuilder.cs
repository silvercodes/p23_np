using System.Net.Sockets;

namespace MTP;

public class ProtoMessageBuilder
{
    private NetworkStream netStream;
    public ProtoMessageBuilder(NetworkStream netStream)
    {
        this.netStream = netStream;
    }
    public ProtoMessage Receive()
    {
        int readingSize =  ConvertToInt(ReadBytes(ProtoMessage.MESSAGE_LEN_LABLE_SIZE));

        ProtoMessage pm = new ProtoMessage();

        return pm;
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
