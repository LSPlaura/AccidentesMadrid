namespace AccidentesMadrid.Services.File.Common;

public interface IFileService<T>
{
    Task Import(IEnumerable<string> paths, int batchSize = 1000);
    IEnumerable<T> GetAll();
    Task Export(string path);
}