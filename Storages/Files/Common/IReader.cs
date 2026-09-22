namespace AccidentesMadrid.Storages.Files.Common;

/// <summary>
/// Permite importar los datos usando un almacenamiento persistente
/// </summary>
public interface IReader<T>
{
    /// <summary>
    /// Exporta los datos de un fichero
    /// </summary>
    /// <param name="path">La ruta al fichero</param>
    /// <returns>Coleccion de los datos mapeados o el error correspondiente</returns>
    IAsyncEnumerable<T> Load(string path);
}