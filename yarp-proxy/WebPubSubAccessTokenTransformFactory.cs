using Yarp.ReverseProxy.Forwarder;
using Yarp.ReverseProxy.Transforms;
using Yarp.ReverseProxy.Transforms.Builder;

namespace yarp_proxy;

public class WebPubSubAccessTokenTransformFactory : ITransformFactory
{

    private readonly ILogger<WebPubSubAccessTokenTransformFactory> _logger;

    public WebPubSubAccessTokenTransformFactory(ILogger<WebPubSubAccessTokenTransformFactory> logger){
        this._logger = logger;
    }

    public bool Build(TransformBuilderContext context,
        IReadOnlyDictionary<string, string> transformValues)
    {
        if (transformValues.TryGetValue("WebPubSubTransform", out var value))
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException(
                    "A non-empty WebPubSubTransform value is required");
            }

            _logger.LogInformation($"WebPubSubAccessTokenTransformFactory is active with value {value}");

            foreach(var contextValue in transformValues){
                _logger.LogInformation($"found transform value: {contextValue.Key} with value: {contextValue.Value}");
            }

            // TODO: fetch actual access token
            //context.AddQueryValue("access_token", "some random access token", true);
            context.AddRequestTransform(context => {
                if(! context.Query.Collection.ContainsKey("access_token")){
                    
                    var deviceId = getDeviceIdFromRouteValues(context);
                    context.Query.Collection["access_token"] = $"my random access token value for device: {deviceId}";
                }

                return default;
            });

            return true; // Matched
        }

        return false;
    }

    private string getDeviceIdFromRouteValues(RequestTransformContext context){
        var routeValues = context.HttpContext.Request.RouteValues;
        if (!routeValues.TryGetValue("DeviceId", out var deviceId))
        {
            return "";
        }
        return deviceId?.ToString()!;
    }

    public bool Validate(TransformRouteValidationContext context, IReadOnlyDictionary<string, string> transformValues)
    {
        if (transformValues.TryGetValue("WebPubSubTransform", out var value))
        {
            if (string.IsNullOrEmpty(value))
            {
                context.Errors.Add(new ArgumentException(
                    "A non-empty WebPubSubTransform value is required"));
            }

            return true; // Matched
        }
        return false;
    }
}