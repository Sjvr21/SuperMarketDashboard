using System;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SuperMarketDashboard;

public delegate Task HttpMiddleware(HttpListenerRequest req, HttpListenerResponse res, Hashtable options);

public class HttpRouter
{
    private List<(string method, string path, HttpMiddleware handler)> routes = new();
    private List<HttpMiddleware> middlewares = new();

    public void Use(HttpMiddleware middleware) => middlewares.Add(middleware);

    public void AddGet(string path, HttpMiddleware handler) => routes.Add(("GET", path, handler));
    public void AddPost(string path, HttpMiddleware handler) => routes.Add(("POST", path, handler));

    public async Task HandleRequest(HttpListenerContext context, Hashtable options)
    {
        var req = context.Request;
        var res = context.Response;

        foreach (var middleware in middlewares)
            await middleware(req, res, options);

        foreach (var route in routes)
        {
            if (req.HttpMethod == route.method && req.Url.AbsolutePath == route.path)
            {
                await route.handler(req, res, options);
                res.Close();
                return;
            }
        }

        await HttpUtils.NotFound(req, res);
        res.Close();
    }
}
