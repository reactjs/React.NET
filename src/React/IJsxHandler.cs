namespace React
{
	/// <summary>
	/// ASP.NET handler that transforms JSX into JavaScript.
	/// </summary>
	public interface IJsxHandler
	{
		/// <summary>
		/// Executes the handler. Outputs JavaScript to the response.
		/// </summary>
		void Execute();
	}
}