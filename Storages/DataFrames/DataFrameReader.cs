using Microsoft.Data.Analysis;

namespace AccidentesMadrid.Storages.DataFrames;

public class DataFrameReader
{
    public DataFrame LoadDataFrame(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Archivo no encontrado: {path}");
        }

        return DataFrame.LoadCsv(
            filename: path,
            separator: ';',
            encoding: System.Text.Encoding.UTF8
        );
    }
}