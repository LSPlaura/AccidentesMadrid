using System.Text;
using AccidentesMadrid.Mappers;
using AccidentesMadrid.Models;

namespace AccidentesMadrid.Storages.Accidentes.Writer;

public class AccidentesWriter : IAccidentesWriter
{
    public async Task<bool> Load(IEnumerable<Accidente> items, string path)
    {
        try
        {
            using var writer = new StreamWriter(path, false, Encoding.UTF8);
            await writer.WriteLineAsync("num_expediente;fecha;hora;localizacion;numero;cod_distrito;distrito;tipo_accidente;estado_meteorológico;tipo_vehiculo;tipo_persona;rango_edad;sexo;cod_lesividad;lesividad;coordenada_x_utm;coordenada_y_utm;positiva_alcohol;positiva_droga");
            
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