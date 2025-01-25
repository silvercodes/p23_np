namespace MTP;

public class ProtoMessage
{
    internal const int MESSAGE_LEN_LABLE_SIZE = 4;
    public string? Action { get; set; }
    public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
    public MemoryStream? PayloadStream { get; internal set; }

    public MemoryStream GetStream()
    {
        MemoryStream memStream = new MemoryStream();

        memStream.Write(new byte[MESSAGE_LEN_LABLE_SIZE], 0, MESSAGE_LEN_LABLE_SIZE);

        StreamWriter writer = new StreamWriter(memStream);

        writer.WriteLine(Action);
        foreach (KeyValuePair<string, string> h in Headers)
            writer.WriteLine($"{h.Key}:{h.Value}");
        writer.WriteLine();
        writer.Flush();

        // write payload

        memStream.Position = 0;

        byte[] sizeLableBytes = ConvertToBytes((int)memStream.Length - MESSAGE_LEN_LABLE_SIZE);
        memStream.Write(sizeLableBytes, 0, MESSAGE_LEN_LABLE_SIZE);

        memStream.Position = 0;

        return memStream;
    }

    private byte[] ConvertToBytes(int val)
    {
        byte[] bytes = BitConverter.GetBytes(val);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);

        return bytes;
    }
}
