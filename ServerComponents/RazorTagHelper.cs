﻿using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;

namespace RazorTagHelpers.ServerComponents;

public abstract class RazorTagHelper : TagHelper
{

    [HtmlAttributeNotBound]
    [ViewContext]
    public ViewContext? ViewContext { get; set; }

    protected async Task<string> GetChildHtmlAsString(TagHelperOutput output)
    {
        var childContent = await output.GetChildContentAsync();
        var trimmedContent = childContent.GetContent().Trim();

        return trimmedContent;
    }

    protected async Task RenderPartialView<T>(string viewRoute, TagHelperOutput output, T model) where T : RazorTagHelperModel
    {
        if (ViewContext is null)
        {
            throw new ArgumentNullException(nameof(ViewContext));
        }

        var childContent = await output.GetChildContentAsync();

        if (childContent is not null)
        {
            model.ChildContent = childContent;
        }

        IRazorRenderer? razorRenderer = ViewContext.HttpContext.RequestServices.GetService<IRazorRenderer>();
        ArgumentNullException.ThrowIfNull(razorRenderer);

        var content = await razorRenderer.RenderAsContent(viewRoute, model, ViewContext);
        output.Content.SetHtmlContent(content);
        output.TagName = null;
    }

    protected async Task RenderPartialView(string viewRoute, TagHelperOutput output)
    {
        if (ViewContext is null)
        {
            throw new ArgumentNullException(nameof(ViewContext));
        }

        IRazorRenderer? razorRenderer = ViewContext.HttpContext.RequestServices.GetService<IRazorRenderer>();
        ArgumentNullException.ThrowIfNull(razorRenderer);

        var content = await razorRenderer.RenderAsContent<object>(viewRoute, null, ViewContext);

        output.TagName = null;
        output.Content.SetHtmlContent(content);
    }
}

