namespace AspNetCoreExample.Ddd.Connection
{
	using AspNetCoreExample.Ddd.Access.Read;
	using AspNetCoreExample.Ddd.Access.ReadWrite;

    public interface IDatabaseFactory
    {
        IDdd OpenDdd();

        IDddWithUpdater OpenDddForUpdate();
    }
}
