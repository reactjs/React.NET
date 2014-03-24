using System;
using System.Linq;
using System.Web;

#pragma warning disable 1591
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
        public static TinyIoCContainer.RegisterOptions AsPerRequestSingleton(this TinyIoCContainer.RegisterOptions registerOptions)
        {
            return TinyIoCContainer.RegisterOptions.ToCustomLifetimeManager(registerOptions, new HttpContextLifetimeProvider(), "per request singleton");
        }

	    /// <summary>
	    /// Determines if this library is executing in the context of an ASP.NET web application.
	    /// </summary>
	    //private static readonly Lazy<bool> _isInAspNet = new Lazy<bool>(() => HttpContext.Current != null);
	    public static bool? IsInAspNet { get; set; }

	    /// <summary>
	    /// If in the context of an ASP.NET web application, register this type as a per-request
	    /// singleton. Otherwise, register it as a regular singleton 
	    /// </summary>
	    /// <param name="registerOptions">TinyIoC registration options</param>
	    /// <returns>TinyIoC registration</returns>
	    public static TinyIoCContainer.RegisterOptions AsReactSingleton(this TinyIoCContainer.RegisterOptions registerOptions)
	    {
		    if (IsInAspNet == null)
		    {
			    throw new Exception("IsInAspNet not set yet!");
		    }

		    return IsInAspNet.Value
			    ? registerOptions.AsPerRequestSingleton()
			    : registerOptions.AsSingleton();
	    }
    }
}
#pragma warning restore