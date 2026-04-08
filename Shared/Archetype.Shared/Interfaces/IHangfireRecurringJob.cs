namespace Archetype.Shared.Interfaces;

public interface IHangfireJob
{
    Task ExecuteAsync();
}