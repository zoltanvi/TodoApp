namespace Modules.Common.Database;

public interface IDbInfoRepository
{
    bool Initialized { get; }
    void Initialize();
}
