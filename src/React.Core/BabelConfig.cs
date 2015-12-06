using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace React
{
	/// <summary>
	/// Configuration for Babel (see http://babeljs.io/docs/usage/options/ for detailed 
	/// documentation).
	/// </summary>
	public class BabelConfig
	{
		/// <summary>
		/// Gets or sets the Babel plugins to use. See http://babeljs.io/docs/plugins/ for a full
		/// list of plugins.
		/// </summary>
		public ISet<string> Plugins { get; set; }

		/// <summary>
		/// Gets or sets the Babel presets to use. See http://babeljs.io/docs/plugins/ for a full
		/// list of presets.
		/// </summary>
		public ISet<string> Presets { get; set; }

		/// <summary>
		/// Creates a new instance of <see cref="BabelConfig" />.
		/// </summary>
		public BabelConfig()
		{
			// Use es2015-no-commonjs by default so Babel doesn't prepend "use strict" to the start of the
			// output. This messes with the top-level "this", as we're not actually using JavaScript modules
			// in ReactJS.NET yet.
			Presets = new HashSet<string> { "es2015-no-commonjs", "stage-1", "react" };
			Plugins = new HashSet<string>();
		}

		/// <summary>
		/// Serializes this Babel configuration into the format required for Babel.
		/// </summary>
		/// <returns></returns>
		public string Serialize()
		{
			return JsonConvert.SerializeObject(this, new JsonSerializerSettings
			{
				ContractResolver = new CamelCasePropertyNamesContractResolver(),
			});
		}
	}
}
