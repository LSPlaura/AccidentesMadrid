namespace AccidentesMadrid.Storages.Common;

/// <summary>
/// Permite exportar los datos usando un almacenamiento persistente
/// </summary>
public interface IWriter<T>
{
    /// <summary>
    /// Exporta los datos a un fichero
    /// </summary>
    /// <param name="items">Coleccion de datos a guardar</param>
    /// <param name="path">Ruta en la que se va a guardar el archivo</param>
    /// <returns>True si se ha podido salvar los datos</returns>
    Task<bool> Load(IEnumerable<T> items, string path);
}