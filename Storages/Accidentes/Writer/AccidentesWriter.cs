using System.Text;
using AccidentesMadrid.Mappers;
using AccidentesMadrid.Models;

namespace AccidentesMadrid.Storages.Accidentes.Writer;

public class AccidentesWriter : IAccidentesWriter
{
    public bool Load(IEnumerable<Accidente> items, string path)
    {
        try
        {
            using var writer = new StreamWriter(path, false, Encoding.UTF8);
            writer.WriteLine("num_expediente;fecha;hora;localizacion;numero;cod_distrito;distrito;tipo_accidente;estado_meteorológico;tipo_vehiculo;tipo_persona;rango_edad;sexo;cod_lesividad;lesividad;coordenada_x_utm;coordenada_y_utm;positiva_alcohol;positiva_droga");
            
            var lista = items.Select(p => p.ToDto()).ToList();
            lista.ForEach(c =>
                writer.WriteLine(
                    $"{c.NumExpediente};{c.Fecha};{c.Hora};{c.Localizacion};{c.Numero};{c.CodDistrito};{c.Distrito};{c.TipoAccidente};{c.EstadoMeteorologico};{c.TipoVehiculo};{c.TipoPersona};{c.RangoEdad};{c.Sexo};{c.CodLesividad};{c.Lesividad};{c.CoordenadaXUtm};{c.CoordenadaYUtm};{c.PositivaAlcohol};{c.PositivaDroga}"));

            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}