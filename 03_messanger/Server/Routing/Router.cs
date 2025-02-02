using MTP;
using MTP.PayloadBase;

namespace Server.Routing;

internal class Router
{
    public List<Route> Routes { get; set; }

    public Router(List<Route> routes)
    {
        Routes = routes;
    }

    public void Handle<T>(ProtoMessage<T> pm)
        where T: IPayload
    {
        Route route = Routes.First(r => r.ActionString == pm.Action);
        route.Execute<T>(pm);


    }

}
