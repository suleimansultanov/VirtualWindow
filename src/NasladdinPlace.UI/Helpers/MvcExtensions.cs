using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using NasladdinPlace.UI.Managers.Reference.Attributes;
using System;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace NasladdinPlace.UI.Helpers
{
    public static class MvcExtensions
    {
        public static Task<IHtmlContent> RenderControlForAsync<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, FormRendererHelper formRendererHelper = null, string template = "Renderer/_renderControls")
        {
            return BaseRenderControlForAsync(htmlHelper, expression, template, formRendererHelper);
        }

        public static Task<IHtmlContent> RenderTextForAsync<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, FormRendererHelper formRendererHelper = null, string template = "Renderer/_renderText")
        {
            return BaseRenderControlForAsync(htmlHelper, expression, template, formRendererHelper);
        }

        public static Task<IHtmlContent> RenderControlAsync<TModel>(this IHtmlHelper<TModel> htmlHelper, RenderAttribute data, string template = "Renderer/_renderControls")
        {
            return BaseRenderControlAsync(htmlHelper, data, template);
        }

        public static Task<IHtmlContent> RenderTextAsync<TModel>(this IHtmlHelper<TModel> htmlHelper, RenderAttribute data, string template = "Renderer/_renderText")
        {
            return BaseRenderControlAsync(htmlHelper, data, template);
        }

        public static IHtmlContent RenderDisplayFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, FormRendererHelper formRendererHelper = null, string template = "Renderer/_renderControls")
        {
            formRendererHelper = formRendererHelper ?? new FormRendererHelper();

            var prop = GetPropertyInfo(expression);

            var formRendererItemInfo = formRendererHelper.GetField(prop);

            return htmlHelper.RenderDisplay(formRendererItemInfo.RenderInfo, template);
        }

        public static IHtmlContent RenderDisplay<TModel>(this IHtmlHelper<TModel> htmlHelper, RenderAttribute data, string template = "Renderer/_renderControls")
        {
            return new HtmlString(data.DisplayName);
        }

        private static Task<IHtmlContent> BaseRenderControlForAsync<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string partialName, FormRendererHelper formRendererHelper = null)
        {
            formRendererHelper = formRendererHelper ?? new FormRendererHelper();

            var prop = GetPropertyInfo(expression);

            var formRendererItemInfo = formRendererHelper.GetField(prop);

            return htmlHelper.BaseRenderControlAsync(formRendererItemInfo.RenderInfo, partialName);
        }

        private static async Task<IHtmlContent> BaseRenderControlAsync<TModel>(this IHtmlHelper<TModel> htmlHelper, RenderAttribute data, string partialName)
        {
            var htmlContent = await htmlHelper.PartialAsync(partialName, data);
            return new HtmlString(htmlContent.GetString());
        }

        private static PropertyInfo GetPropertyInfo<TModel, TProperty>(Expression<Func<TModel, TProperty>> expression)
        {
            var unaryExpr = expression.Body as UnaryExpression;
            var memberExpr = (MemberExpression)(unaryExpr?.Operand ?? expression.Body);

            var prop = memberExpr.Member as PropertyInfo;
            if (prop == null)
            {
                throw new InvalidOperationException("Specified member is not a property.");
            }
            return prop;
        }

        public static string GetString(this IHtmlContent content)
        {
            var writer = new StringWriter();
            content.WriteTo(writer, HtmlEncoder.Default);
            return writer.ToString();
        }
    }
}
