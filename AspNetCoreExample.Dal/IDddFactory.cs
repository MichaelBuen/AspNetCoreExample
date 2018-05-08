namespace AspNetCoreExample.Dal
{
    public interface IDddFactory
    {
        IDdd OpenDdd();
        IDdd OpenDddForChanges();
    }
}
