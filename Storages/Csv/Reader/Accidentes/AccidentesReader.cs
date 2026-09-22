using System.Text;
using AccidentesMadrid.Dtos;
using AccidentesMadrid.Mappers;
using AccidentesMadrid.Models;
using AccidentesMadrid.Storages.Accidentes.Reader;

namespace AccidentesMadrid.Storages.Csv.Reader.Accidentes;

public class AccidentesReader : IAccidentesReader
{
    public IAsyncEnumerable<Accidente> Cargar(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Archivo no encontrado: {path}");
        }

        return File.ReadLinesAsync(path, Encoding.UTF8)
            .Skip(1)
            .Select(ParseLinea)
            .OfType<Accidente>();
    }

    private Accidente? ParseLinea(string linea) {
        var partes = linea.Split(';', StringSplitOptions.TrimEntries);
        
        if (partes.Length < 19) return null; 

        var dto = new AccidenteDto(
            partes[0],
            partes[1],
            partes[2],
            partes[3],
            partes[4],
            partes[5],
            partes[6],
            partes[7],
            partes[8],
            partes[9],
            partes[10],
            partes[11],
            partes[12],
            partes[13],
            partes[14],
            partes[15],
            partes[16],
            partes[17],
            partes[18]
        );
        return dto.ToModel();
    }
}