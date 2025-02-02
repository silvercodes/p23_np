using MTP;
using MTP.PayloadBase;

namespace Server.Routing;

internal class Route
{
    public string ActionString { get; set; }
    public Type controllerType;
    public string action;

    public Route(string actionString, Type controllerType, string action)
    {
        ActionString = actionString;
        this.controllerType = controllerType;
        this.action = action;
    }

    public void Execute<T>(ProtoMessage<T> pm)
        where T : IPayload
    {
        object? controller = Activator.CreateInstance(controllerType);          // Controller controler = new AuthControoler()

        controllerType.GetMethod(action).Invoke(controller, new[] { pm });      // controller.Auth(pm)


    }


}
