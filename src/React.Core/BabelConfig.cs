using System.Collections.Generic;
using Newtonsoft.Json;

namespace React
{
	/// <summary>
	/// Configuration for Babel (see http://babeljs.io/docs/usage/options/ for detailed 
	/// documentation).
	/// </summary>
	public class BabelConfig
	{
		/// <summary>
		/// Gets or sets whether Babel's "loose mode" is used for all transformers. See 
		/// http://babeljs.io/docs/advanced/loose/ for more information. Only one of 
		/// <see cref="AllLoose"/> or <see cref="Loose"/> can be used at a time.
		/// </summary>
		public bool AllLoose { get; set; }

		/// <summary>
		/// Gets or sets whether Babel should use a reference to babelHelpers instead of placing
		/// helpers at the top of your code. Meant to be used in conjunction with external 
		/// helpers (http://babeljs.io/docs/advanced/external-helpers/)
		/// </summary>
		public bool ExternalHelpers { get; set; }

		/// <summary>
		/// Gets or sets the transformers to use in Babel's "loose mode". See
		/// http://babeljs.io/docs/advanced/loose/ for more information. Only one of 
		/// <see cref="AllLoose"/> or <see cref="Loose"/> can be used at a time.
		/// </summary>
		public IEnumerable<string> Loose { get; set; }

		/// <summary>
		/// Gets or sets an transformers to optionally use. See 
		/// http://babeljs.io/docs/advanced/transformers/#optional for a full list of transformers
		/// </summary>
		public IEnumerable<string> Optional { get; set; }

		/// <summary>
		/// Gets or sets the experimental proposal stage (http://babeljs.io/docs/usage/experimental/).
		/// </summary>
		public int Stage { get; set; }

		/// <summary>
		/// Creates a new instance of <see cref="BabelConfig" />.
		/// </summary>
		public BabelConfig()
		{
			Stage = 2;
		}

		/// <summary>
		/// Serializes this Babel configuration into the format required for Babel.
		/// </summary>
		/// <returns></returns>
		public string Serialize()
		{
			var config = new Dictionary<string, object>
			{
				{"externalHelpers", ExternalHelpers},
				{"optional", Optional},
				{"stage", Stage},
			};
			if (AllLoose)
			{
				config.Add("loose", "all");
			}
			else if (Loose != null)
			{
				config.Add("loose", Loose);
			}
			return JsonConvert.SerializeObject(config);
		}
	}
}
