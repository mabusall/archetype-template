namespace Sample.Notification.Helper;

public class EmailTemplateRenderer(IRazorViewEngine engine,
                                   IServiceProvider serviceProvider,
                                   ITempDataProvider tempDataProvider)
    : IEmailTemplateRenderer
{
    public async Task<string> RenderRazorTemplateAsStringAsync<T>(string razorViewName, T model)
    {
        DefaultHttpContext httpContext = new() { RequestServices = serviceProvider };
        ActionContext actionContext = new(httpContext, new RouteData(), new ActionDescriptor());

        using StringWriter sw = new();
        ViewEngineResult viewResult = engine.FindView(actionContext, razorViewName, false);

        if (!viewResult.Success) return string.Empty;

        ViewDataDictionary viewDataDictionary = new(new EmptyModelMetadataProvider(),
                                                        new ModelStateDictionary())
        {
            Model = model
        };

        ViewContext viewContext = new(actionContext,
                                          viewResult.View,
                                          viewDataDictionary,
                                          new TempDataDictionary(actionContext.HttpContext, tempDataProvider),
                                          sw,
                                          new HtmlHelperOptions());

        await viewResult.View.RenderAsync(viewContext);

        return sw.ToString();
    }

    public async Task<string> RenderRazorTemplateFromPathAsStringAsync<T>(string razorViewPath, T model)
    {
        DefaultHttpContext httpContext = new() { RequestServices = serviceProvider };
        ActionContext actionContext = new(httpContext, new RouteData(), new ActionDescriptor());

        using StringWriter sw = new();
        ViewEngineResult viewResult = engine.GetView(razorViewPath, razorViewPath, false);

        if (!viewResult.Success) return string.Empty;

        ViewDataDictionary viewDataDictionary = new(new EmptyModelMetadataProvider(),
                                                        new ModelStateDictionary())
        {
            Model = model
        };

        ViewContext viewContext = new(actionContext,
                                          viewResult.View,
                                          viewDataDictionary,
                                          new TempDataDictionary(actionContext.HttpContext, tempDataProvider),
                                          sw,
                                          new HtmlHelperOptions());

        await viewResult.View.RenderAsync(viewContext);

        return sw.ToString();
    }
}