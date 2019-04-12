using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using React.Exceptions;

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
		/// Serializes this Babel configuration into the format required for Babel.
		/// </summary>
		/// <returns></returns>
		public string Serialize(string babelVersion)
		{
			ISet<string> defaultPresets = babelVersion == BabelVersions.Babel7
				? new HashSet<string> { "typescript", "react" }
				: babelVersion == BabelVersions.Babel6 || babelVersion == null
					? new HashSet<string> { "es2015-no-commonjs", "stage-1", "react" }
					: throw new ArgumentException(nameof(babelVersion));

			ISet<string> defaultPlugins = babelVersion == BabelVersions.Babel7
				? new HashSet<string> { "proposal-class-properties", "proposal-object-rest-spread" }
				: babelVersion == BabelVersions.Babel6 || babelVersion == null
					? new HashSet<string>()
					: throw new ArgumentException(nameof(babelVersion));

			return JsonConvert.SerializeObject(
				new BabelConfig
				{
					Plugins = Plugins ?? defaultPlugins,
					Presets = Presets ?? defaultPresets,
				}, 
				new JsonSerializerSettings
				{
					ContractResolver = new CamelCasePropertyNamesContractResolver(),
				});
		}
	}
}
