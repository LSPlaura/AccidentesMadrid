namespace AccidentesMadrid.Services.Csv.Common;

public interface ICsvService<T>
{
    Task Salvar(IEnumerable<string> paths);
    IEnumerable<T> Cargar();
}