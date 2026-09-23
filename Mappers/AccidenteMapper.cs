using System.Globalization;
using AccidentesMadrid.Dtos;
using AccidentesMadrid.Models;

namespace AccidentesMadrid.Mappers;

public static class AccidenteMapper {
    private static readonly string _isoFormat = "s";
    private static readonly CultureInfo _invariant = CultureInfo.InvariantCulture;
    
    private static readonly HashSet<string> _rangosEdadValidos = new(StringComparer.OrdinalIgnoreCase) {
        "Menor de 5 años", "De 6 a 9 años", "De 10 a 14 años", "De 15 a 17 años",
        "De 18 a 20 años", "De 21 a 24 años", "De 25 a 29 años", "De 30 a 34 años",
        "De 35 a 39 años", "De 40 a 44 años", "De 45 a 49 años", "De 50 a 54 años",
        "De 55 a 59 años", "De 60 a 64 años", "De 65 a 69 años", "De 70 a 74 años",
        "Más de 74 años", "Desconocido"
    };

    public static Accidente ToModel(this AccidenteDto dto) {
        return new Accidente(
            dto.NumExpediente?.Trim() ?? string.Empty,
            ParseFecha(dto.Fecha),
            ParseHora(dto.Hora),
            dto.Localizacion?.Trim() ?? string.Empty,
            ParseInt(dto.Numero),
            ParseInt(dto.CodDistrito),
            dto.Distrito?.Trim() ?? string.Empty,
            ParseTipoAccidente(dto.TipoAccidente),
            SanitizarEstadoClima(dto.EstadoMeteorologico),
            dto.TipoVehiculo?.Trim() ?? string.Empty,
            ParseTipoPersona(dto.TipoPersona),
            SanitizarRangoEdad(dto.RangoEdad),
            ParseSexo(dto.Sexo),
            dto.CodLesividad?.Trim() ?? string.Empty,
            ParseGravedad(dto.CodLesividad),
            string.IsNullOrWhiteSpace(dto.CoordenadaXUtm) ? null : dto.CoordenadaXUtm.Trim(),
            string.IsNullOrWhiteSpace(dto.CoordenadaYUtm) ? null : dto.CoordenadaYUtm.Trim(),
            ParseBooleano(dto.PositivaAlcohol),
            ParseBooleano(dto.PositivaDroga)
        );
    }

    public static AccidenteDto ToDto(this Accidente accidente) {
        return new AccidenteDto(
            accidente.NumExpediente,
            accidente.Fecha.ToString("dd/MM/yyyy", _invariant),
            accidente.Hora.ToString(@"hh\:mm\:ss"),
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
            // Corregido para que devuelva valores homogéneos ("S" / "N")
            accidente.PositivoAlcohol ? "S" : "N",
            accidente.PositivoDroga ? "S" : "N" // Unificado a "S"/"N"
        );
    }

    private static DateTime ParseFecha(string? fecha) {
        if (string.IsNullOrWhiteSpace(fecha)) return DateTime.MinValue;

        string fechaLimpia = fecha.Trim();

        // 1. Probar formatos exactos sin hora
        string[] formatosExactos = ["dd/MM/yyyy", "d/M/yyyy", "yyyy-MM-dd", "dd-MM-yyyy"];
        if (DateTime.TryParseExact(fechaLimpia, formatosExactos, _invariant, DateTimeStyles.None, out var dtExacta))
            return dtExacta;

        // 2. Probar parseo flexible para fechas con/sin hora o formato ISO
        if (DateTime.TryParse(fechaLimpia, _invariant, DateTimeStyles.None, out var dtFlex))
            return dtFlex;

        // 3. Probar fechas con hora en cultura española por si el separador cambia
        if (DateTime.TryParse(fechaLimpia, new CultureInfo("es-ES"), DateTimeStyles.None, out var dtEs))
            return dtEs;

        return DateTime.MinValue;
    }

    private static TimeSpan ParseHora(string? hora) {
        if (string.IsNullOrWhiteSpace(hora)) return TimeSpan.Zero;

        string horaLimpia = hora.Trim();
        
        string[] formatosTimeSpan = [@"hh\:mm\:ss", @"h\:mm\:ss", @"hh\:mm", @"h\:mm"];
        if (TimeSpan.TryParseExact(horaLimpia, formatosTimeSpan, _invariant, out var ts))
            return ts;
        
        if (DateTime.TryParse(horaLimpia, _invariant, DateTimeStyles.NoCurrentDateDefault, out var dt))
            return dt.TimeOfDay;

        return TimeSpan.Zero;
    }

    private static string SanitizarRangoEdad(string? rango) {
        if (string.IsNullOrWhiteSpace(rango)) return "Desconocido";

        string valor = rango.Trim();
        return _rangosEdadValidos.Contains(valor) ? valor : "Desconocido";
    }

    private static int ParseInt(string? valor) {
        if (string.IsNullOrWhiteSpace(valor)) return 0;
        return int.TryParse(valor.Trim(), out var resultado) ? resultado : 0;
    }

    private static Sexo ParseSexo(string? sexo) {
        return (sexo?.Trim().ToLower()) switch {
            "hombre" => Sexo.Hombre,
            "mujer" => Sexo.Mujer,
            _ => Sexo.Desconocido
        };
    }

    private static TipoPersona ParseTipoPersona(string? tipo) {
        return (tipo?.Trim().ToLower()) switch {
            "conductor" => TipoPersona.Conductor,
            "pasajero" => TipoPersona.Pasajero,
            "peatón" or "peaton" => TipoPersona.Peatón,
            _ => TipoPersona.Conductor
        };
    }

    private static TipoAccidente ParseTipoAccidente(string? tipo) {
        if (string.IsNullOrWhiteSpace(tipo)) return TipoAccidente.OtrasCausas;

        string t = tipo.Trim().ToLower();
        
        if (t.Contains("doble") || t.Contains("colisión") || t.Contains("colision")) 
            return TipoAccidente.ColisionDoble;

        if (t.Contains("múltiple") || t.Contains("multiple")) 
            return TipoAccidente.ColisionMultiple;

        if (t.Contains("alcance")) 
            return TipoAccidente.Alcance;

        if (t.Contains("obstáculo") || t.Contains("obstaculo")) 
            return TipoAccidente.ChoqueObstaculo;

        if (t.Contains("atropello")) 
            return TipoAccidente.AtropelloPersona;

        if (t.Contains("vuelco")) 
            return TipoAccidente.Vuelco;

        if (t.Contains("caída") || t.Contains("caida")) 
            return TipoAccidente.Caida;

        return TipoAccidente.OtrasCausas;
    }

    private static string SanitizarEstadoClima(string? clima) {
        if (string.IsNullOrWhiteSpace(clima)) return "Se desconoce";

        string val = clima.Trim();
        HashSet<string> climaValido = new(StringComparer.OrdinalIgnoreCase) {
            "Despejado", "Nublado", "Lluvia débil", "LLuvia intensa", "Nevando", "Granizando", "Se desconoce"
        };

        return climaValido.Contains(val) ? val : "Se desconoce";
    }

    private static Gravedad ParseGravedad(string? cod) {
        if (string.IsNullOrWhiteSpace(cod)) return Gravedad.SinAsistencia;

        string nCod = cod.Trim().TrimStart('0');

        return nCod switch {
            "1" or "2" or "5" or "6" or "7" => Gravedad.Leve,
            "3" => Gravedad.Grave,
            "4" => Gravedad.Fallecido,
            "14" or "" => Gravedad.SinAsistencia,
            "77" => Gravedad.Desconocido,
            _ => Gravedad.SinAsistencia
        };
    }

    private static bool ParseBooleano(string? valor) {
        if (string.IsNullOrWhiteSpace(valor)) return false;
        string v = valor.Trim().ToUpperInvariant();
        return v is "S" or "SI" or "1" or "TRUE" or "1.0";
    }
}