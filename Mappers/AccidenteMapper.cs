using System.Globalization;
using AccidentesMadrid.Dtos;
using AccidentesMadrid.Models;

namespace AccidentesMadrid.Mappers;

public static class AccidenteMapper {
    private static readonly string _isoFormat = "s";
    private static readonly CultureInfo _invariant = CultureInfo.InvariantCulture;
    public static Accidente ToModel(this AccidenteDto dto) {
        return new Accidente(
            dto.NumExpediente,
            ParseFecha(dto.Fecha),
            ParseHora(dto.Hora),
            dto.Localizacion,
            ParseInt(dto.Numero),
            ParseInt(dto.CodDistrito),
            dto.Distrito,
            ParseTipoAccidente(dto.TipoAccidente),
            dto.EstadoMeteorologico,
            dto.TipoVehiculo,
            ParseTipoPersona(dto.TipoPersona),
            dto.RangoEdad,
            ParseSexo(dto.Sexo),
            dto.CodLesividad,
            ParseGravedad(dto.CodLesividad),
            string.IsNullOrWhiteSpace(dto.CoordenadaXUtm) ? null : dto.CoordenadaXUtm,
            string.IsNullOrWhiteSpace(dto.CoordenadaYUtm) ? null : dto.CoordenadaYUtm,
            ParseAlcohol(dto.PositivaAlcohol),
            ParseDroga(dto.PositivaDroga)
        );
    }

    public static AccidenteDto ToDto(this Accidente accidente)
    {
        return new AccidenteDto(
            accidente.NumExpediente,
            accidente.Fecha.ToString(_isoFormat, _invariant),
            accidente.Hora.ToString(),
            accidente.Localizacion,
            accidente.Numero.ToString(),
            accidente.CodDistrito.ToString(),
            accidente.Distrito,
            accidente.TipoAccidente.ToString(),
            accidente.EstadoMeteorologico,
            accidente.TipoVehiculo,
            accidente.TipoPersona.ToString(),
            accidente.RangoEdad,
            accidente.Sexo.ToString(),
            accidente.CodLesividad,
            accidente.Gravedad.ToString(),
            accidente.CoordenadaXUtm ?? "Desconocida",
            accidente.CoordenadaYUtm ?? "Desconocida",
            accidente.PositivoAlcohol.ToString(),
            accidente.PositivoDroga.ToString()
        );
    }

    private static DateTime ParseFecha(string fecha) {
        var partes = fecha.Split('/');
        return new DateTime(int.Parse(partes[2]), int.Parse(partes[1]), int.Parse(partes[0]));
    }

    private static TimeSpan ParseHora(string hora) {
        var partes = hora.Split(':');
        return new TimeSpan(int.Parse(partes[0]), int.Parse(partes[1]), int.Parse(partes[2]));
    }

    private static int ParseInt(string valor) {
        return int.TryParse(valor, out var resultado) ? resultado : 0;
    }

    private static Sexo ParseSexo(string sexo) {
        return sexo.ToLower() switch {
            "hombre" => Sexo.Hombre,
            "mujer" => Sexo.Mujer,
            _ => Sexo.Desconocido
        };
    }

    private static TipoPersona ParseTipoPersona(string tipo) {
        return tipo.ToLower() switch {
            "conductor" => TipoPersona.Conductor,
            "pasajero" => TipoPersona.Pasajero,
            "peatón" or "peaton" => TipoPersona.Peatón,
            _ => TipoPersona.Conductor
        };
    }

    private static TipoAccidente ParseTipoAccidente(string tipo) {
        return tipo.ToLower() switch {
            { } t when t.Contains("doble") => TipoAccidente.ColisionDoble,
            { } t when t.Contains("múltiple") || t.Contains("multiple") => TipoAccidente.ColisionMultiple,
            { } t when t.Contains("alcance") => TipoAccidente.Alcance,
            { } t when t.Contains("obstáculo") || t.Contains("obstaculo") => TipoAccidente.ChoqueObstaculo,
            { } t when t.Contains("atropello") => TipoAccidente.AtropelloPersona,
            { } t when t.Contains("vuelco") => TipoAccidente.Vuelco,
            { } t when t.Contains("caída") || t.Contains("caida") => TipoAccidente.Caida,
            _ => TipoAccidente.OtrasCausas
        };
    }

    private static Gravedad ParseGravedad(string cod) {
        
        var nCod = cod.Trim().TrimStart('0');

        return nCod switch {
            "1" or "2" or "5" or "6" or "7" => Gravedad.Leve,
            "3" => Gravedad.Grave,
            "4" => Gravedad.Fallecido,
            "14" or "" or " " => Gravedad.SinAsistencia,
            "77" => Gravedad.Desconocido,
            _ => Gravedad.SinAsistencia
        };
    }

    private static bool ParseAlcohol(string alcohol) {
        return alcohol.Equals("S", StringComparison.CurrentCultureIgnoreCase);
    }

    private static bool ParseDroga(string droga) {
        return droga == "1";
    }
}