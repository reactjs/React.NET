namespace React.Router
{
	/// <summary>
	/// Contains the context object used during execution in addition to 
	/// the string result of rendering the React Router component.
	/// </summary>
	public class ExecutionResult
    {
        /// <summary>
        /// String result of ReactDOMServer render of provided component.
        /// </summary>
        public string renderResult { get; set; }

        /// <summary>
        /// Context object used during JS engine execution.
        /// </summary>
        public RoutingContext context { get; set; }
    }
}
