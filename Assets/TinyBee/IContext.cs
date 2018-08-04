namespace TinyBee.Context
{
	public interface IContext
	{
		string LanguagePath { get; }
		string DataPath { get; }
		string ZipPath { get; }
		string Version { get; }
	}
}