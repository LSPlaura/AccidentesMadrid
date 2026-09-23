using System.Text;
using AccidentesMadrid.Mappers;
using AccidentesMadrid.Models;

namespace AccidentesMadrid.Storages.Files.Csv.Writer.Accidentes;

public class AccidentesWriter : IAccidentesWriter
{
    public async Task<bool> Load(IEnumerable<Accidente> items, string path)
    {
        try
        {
            using var writer = new StreamWriter(path, false, Encoding.UTF8);
            await writer.WriteLineAsync(string.Join(";", 
                Config.Config.NumExpediente,
                Config.Config.Fecha,
                Config.Config.Hora,
                Config.Config.Localizacion,
                Config.Config.Numero,
                Config.Config.CodDistrito,
                Config.Config.Distrito,
                Config.Config.TipoAccidente,
                Config.Config.EstadoMeteorologico,
                Config.Config.TipoVehiculo,
                Config.Config.TipoPersona,
                Config.Config.RangoEdad,
                Config.Config.Sexo,
                Config.Config.CodLesividad,
                Config.Config.Lesividad,
                Config.Config.CoordenadaXUtm,
                Config.Config.CoordenadaYUtm,
                Config.Config.PositivaAlcohol,
                Config.Config.PositivaDroga
            ));
            
            foreach (var accidente in items)
            {
                var dto = accidente.ToDto();

                await writer.WriteLineAsync(
                    $"{dto.NumExpediente};{dto.Fecha};{dto.Hora};{dto.Localizacion};{dto.Numero};{dto.CodDistrito};{dto.Distrito};{dto.TipoAccidente};{dto.EstadoMeteorologico};{dto.TipoVehiculo};{dto.TipoPersona};{dto.RangoEdad};{dto.Sexo};{dto.CodLesividad};{dto.Lesividad};{dto.CoordenadaXUtm};{dto.CoordenadaYUtm};{dto.PositivaAlcohol};{dto.PositivaDroga}");
            }
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}