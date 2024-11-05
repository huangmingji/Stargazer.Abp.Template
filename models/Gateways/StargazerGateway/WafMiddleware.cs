namespace StargazerGateway;

public static class WafMiddleware
{
    public static void UseWaf(this IReverseProxyApplicationBuilder proxyPipeline)
    {
        proxyPipeline.Use((context, next) =>
        {
            if (!CheckServerDefense(context))
            {
                context.Response.StatusCode = 444;
                return context.Response.WriteAsync("I catch you!");
            }
            return next();
        });
    }

    private static bool CheckServerDefense(HttpContext context)
    {
        if(!CheckHeader(context)) return false;
        if(!CheckQueryString(context)) return false;
        if(!CheckBody(context)) return false;
        return true;
    }

    private static bool CheckHeader(HttpContext context)
    {
        // TODO: 检查请求头是否包含敏感词
        return true;
    }

    private static bool CheckQueryString(HttpContext context)
    {
        if(context.Request.Query.ContainsKey("base64_encode")
           || context.Request.Query.ContainsKey("base64_decode")
           || context.Request.Query.ContainsKey("WEB-INF")
           || context.Request.Query.ContainsKey("META-INF")
           || context.Request.Query.ContainsKey("%3Cscript%3E")
           || context.Request.Query.ContainsKey("%3Ciframe")
           || context.Request.Query.ContainsKey("%3Cimg")
           || context.Request.Query.ContainsKey("proc/self/environ")
           || context.Request.Query.ContainsKey("mosConfig_"))
        {
            return false;
        }
        // TODO: 检查请求参数是否包含敏感词
        return true;
    }

    private static bool CheckBody(HttpContext context)
    {
        if (context.Request.Method == "POST"
            || context.Request.Method == "PUT"
            || context.Request.Method == "PATCH")
        {
            // TODO: 读取请求体, 判断是否包含敏感词
            // 重置请求体
            context.Request.Body.Seek(0, SeekOrigin.Begin);
            return true;
        }

        return  true;
    }
}