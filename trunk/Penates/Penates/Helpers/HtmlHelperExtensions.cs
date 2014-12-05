using System;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace Penates.Helpers
{
    public static class HtmlHelperExtensions{

        public static string GetLocalResource(this HtmlHelper htmlHelper, string virtualPath, string resourceKey) {
            try {
                var resource = htmlHelper.ViewContext.HttpContext.GetLocalResourceObject(virtualPath, resourceKey);
                return resource != null ? resource.ToString() : string.Empty;
            } catch (Exception) {
                return null;
            }
        }

        public static string Resource(this HtmlHelper htmlHelper, string resourceKey) {
            var virtualPath = ((WebViewPage) htmlHelper.ViewDataContainer).VirtualPath;
            return GetLocalResource(htmlHelper, virtualPath, resourceKey);
        }

        public static string GetLocalResource(this  HtmlHelper<dynamic> htmlHelper, string virtualPath, string resourceKey) {
            try {
                var resource = htmlHelper.ViewContext.HttpContext.GetLocalResourceObject(virtualPath, resourceKey);
                return resource != null ? resource.ToString() : string.Empty;
            } catch (Exception) {
                return null;
            }
        }

        public static string Resource(this  HtmlHelper<dynamic> htmlHelper, string resourceKey) {
            var virtualPath = ((WebViewPage) htmlHelper.ViewDataContainer).VirtualPath;
            return GetLocalResource(htmlHelper, virtualPath, resourceKey);
        }

        public static string getCulture(this HtmlHelper htmlHelper) {
            return System.Threading.Thread.CurrentThread.CurrentCulture.IetfLanguageTag;
        }

        public static string getCulture(this HtmlHelper<dynamic> htmlHelper) {
            return System.Threading.Thread.CurrentThread.CurrentCulture.IetfLanguageTag;
        }

        public static string getUICulture(this HtmlHelper htmlHelper) {
            return System.Threading.Thread.CurrentThread.CurrentUICulture.IetfLanguageTag;
        }

        public static string getUICulture(this HtmlHelper<dynamic> htmlHelper) {
            return System.Threading.Thread.CurrentThread.CurrentUICulture.IetfLanguageTag;
        }
    }
}