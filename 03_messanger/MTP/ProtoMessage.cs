using MTypes;

namespace MTP;

public class ProtoMessage
{
    private const char HEADER_SEPARATOR = ':';
    private const string HEADER_PAYLOAD_LEN_KEY = "len";
    private const string HEADER_PAYLOAD_TYPE_KEY = "ptype";
    internal const int MESSAGE_LEN_LABLE_SIZE = 4;
    public string? Action { get; set; }
    internal Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
    public IPayload? Payload { get; private set; }
    public MemoryStream? PayloadStream { get; internal set; }
    public int PaylodLength
    {
        get
        {
            Headers.TryGetValue(HEADER_PAYLOAD_LEN_KEY, out string? value);

            if (string.IsNullOrEmpty(value))
                return 0;

            return Convert.ToInt32(value);
        }
    }

    public void SetHeader(string key, string value)
    {
        Headers[key] = value;
    }
    public void SetHeader(string? headerLine)
    {
        if (headerLine is null)
            return;

        string[] chunks = headerLine.Split(HEADER_SEPARATOR, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        SetHeader(chunks[0], chunks[1]);
    }
    public void SetPayload(IPayload payload)
    {
        Payload = payload;

        PayloadStream = payload.GetStream();

        Headers[HEADER_PAYLOAD_LEN_KEY] = PayloadStream.Length.ToString();
        Headers[HEADER_PAYLOAD_TYPE_KEY] = Payload.PType;
    }

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

        if (Payload is not null && PayloadStream is not null)
        {
            PayloadStream.Position = 0;
            PayloadStream.CopyTo(memStream);
        }

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
