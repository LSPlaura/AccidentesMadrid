namespace AccidentesMadrid.Services.File.Common;

public interface IFileService<T>
{
    Task Salvar(IEnumerable<string> paths, int batchSize = 1000);
    IEnumerable<T> Cargar();
    Task Export(string path);
}