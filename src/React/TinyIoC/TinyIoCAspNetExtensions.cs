using System;
using System.Linq;
using System.Web;

namespace React.TinyIoC
{
    public class HttpContextLifetimeProvider : TinyIoCContainer.ITinyIoCObjectLifetimeProvider
    {
        private const string PREFIX = "TinyIoC.HttpContext.";
        private readonly string _KeyName = PREFIX + Guid.NewGuid();

        public object GetObject()
        {
            return HttpContext.Current.Items[_KeyName];
        }

        public void SetObject(object value)
        {
            HttpContext.Current.Items[_KeyName] = value;
        }

        public void ReleaseObject()
        {
            var item = GetObject() as IDisposable;

            if (item != null)
                item.Dispose();

            SetObject(null);
        }

        public static void DisposeAll()
        {
            var items = HttpContext.Current.Items;
            var disposableItems = items.Keys.OfType<string>()
                .Where(key => key.StartsWith(PREFIX))
                .Select(key => items[key])
                .Where(item => item is IDisposable);

            foreach (var item in disposableItems)
            {
                ((IDisposable)item).Dispose();
            }
        }
    }

    public static class TinyIoCAspNetExtensions
    {
        public static TinyIoC.TinyIoCContainer.RegisterOptions AsPerRequestSingleton(this TinyIoC.TinyIoCContainer.RegisterOptions registerOptions)
        {
            return TinyIoCContainer.RegisterOptions.ToCustomLifetimeManager(registerOptions, new HttpContextLifetimeProvider(), "per request singleton");
        }
    }
}
