using MTP;
using MTP.PayloadBase;
using MTypes;

namespace Server.Controllers;

internal class AuthController
{
    public void Login(ProtoMessage<AuthRequestPayload> pm)
    {
        AuthRequestPayload? p = pm.GetPayload();

        Console.WriteLine($"{p.Login} {p.Password}");
    }
}
