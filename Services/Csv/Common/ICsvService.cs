namespace AccidentesMadrid.Services.Csv.Common;

public interface ICsvService<T>
{
    Task Salvar(IEnumerable<string> paths, int batchSize = 1000);
    IEnumerable<T> Cargar();
    Task Export(string path);
}