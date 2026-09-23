using System.Globalization;
using System.IO;
using System.Text;
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
        
        Type[] dataTypes = Enumerable.Repeat(typeof(string), 19).ToArray();

        string csvContent = File.ReadAllText(path, Encoding.UTF8).Replace("\"", "");
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

        return DataFrame.LoadCsv(
            csvStream: stream,
            separator: ';',
            header: true,
            dataTypes: dataTypes,
            cultureInfo: CultureInfo.InvariantCulture
        );
    }
}