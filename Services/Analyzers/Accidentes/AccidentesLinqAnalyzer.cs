using AccidentesMadrid.Models;
using AccidentesMadrid.Services.Analyzers.Accidentes.Common;

namespace AccidentesMadrid.Services.Analyzers.Accidentes;

public class AccidentesLinqAnalyzer() : IAccidentesAnalyzer
{
    // Total de accidentes
    public int TotalAccidentes(IEnumerable<Accidente> accidentes)
    {
        return accidentes.Count();
    }
    // Accidentes por distrito (top 5)
    public IEnumerable<(string Distrito, int Total)> AccidentesPorDistrito(IEnumerable<Accidente> accidentes)
    {
        return accidentes
            .DistinctBy(a => a.NumExpediente)
            .GroupBy(a => a.Distrito)
            .Select(g => (Distrito: g.Key, Total: g.Count()))
            .OrderByDescending(x => x.Total)
            .Take(5);
    }
    // Accidentes por tipo
    public IEnumerable<(TipoAccidente TipoAccidente, int Total)> AccidentesPorTipo(IEnumerable<Accidente> accidentes)
    {
        return accidentes
            .DistinctBy(a => a.NumExpediente)
            .GroupBy(a => a.TipoAccidente)
            .Select(g => (Tipo: g.Key, Total: g.Count()));
    }
    
    //Accidentes por estado meteorológico
    public IEnumerable<(string EstadoMetereologico, int Total)> AccidentesPorEstadoMetereologico(IEnumerable<Accidente> accidentes)
    {
        return accidentes
            .DistinctBy(a => a.NumExpediente)
            .GroupBy(a => a.EstadoMeteorologico)
            .Select(g => (EstadoMeteorologico: g.Key, Total: g.Count()));
    }
    
    //personas que se ven implicadas en un accidente por sexo
    public IEnumerable<(Sexo Sexo, int Total)> AccidentesPorSexo(IEnumerable<Accidente> accidentes)
    {
        return accidentes
            .GroupBy(a => a.Sexo)
            .Select(g => (Sexo: g.Key, Total: g.Count()));
    }
    // Accidentes por rango de edad //personas que se ven implicadas en un accidente por rango de edad
    public IEnumerable<(string RangoEdad, int Total)> AccidentesPorRangoEdad(IEnumerable<Accidente> accidentes)
    {
        return accidentes
            .GroupBy(a => a.RangoEdad)
            .Select(g => (RangoEdad: g.Key, Total: g.Count()));
    }
    // Positivos en alcohol
    public int PositivosAlcohol(IEnumerable<Accidente> accidentes)
    {
        return accidentes.Count(a => a.PositivoAlcohol);
    }
    // Positivos en drogas
    public int PositivosDrogas(IEnumerable<Accidente> accidentes)
    {
        return accidentes.Count(a => a.PositivoDroga);
    }
    // Accidentes por día de la semana
    public IEnumerable<(DayOfWeek Dia, int Total)> AccidentesPorDiaDeLaSemana(IEnumerable<Accidente> accidentes)
    {
        return accidentes
            .DistinctBy(a => a.NumExpediente)
            .GroupBy(a => a.Fecha.DayOfWeek)
            .Select(g => (Dia: g.Key, Total: g.Count()))
            .OrderByDescending(x => x.Total);
    }
    // Accidentes por mes
    public IEnumerable<(int Mes, int Total)> AccidentesPorMes(IEnumerable<Accidente> accidentes)
    {
        return accidentes
            .DistinctBy(a => a.NumExpediente)
            .GroupBy(a => a.Fecha.Month)
            .Select(g => (Mes: g.Key, Total: g.Count()))
            .OrderBy(m => m.Mes);
    }
    // Hora con más accidentes
    public (int Hora, int Total) HoraConMasAccidentes(IEnumerable<Accidente> accidentes)
    {
        return accidentes
            .DistinctBy(a => a.NumExpediente)
            .GroupBy(a => a.Hora.Hours)
            .Select(g => (Hora: g.Key, Total: g.Count()))
            .MaxBy(a => a.Total);
    }
    // Lesiones más frecuentes
    public (TipoAccidente TipoAccidente, int Total) LesionesMasFrecuentes(IEnumerable<Accidente> accidentes)
    {
        return accidentes
            .GroupBy(a => a.TipoAccidente)
            .Select(g => (Tipo: g.Key, Total: g.Count()))
            .MaxBy(a => a.Total);
    }
    // Tipo de vehículo más implicado
    public (string TipoVehiculo, int Total) VehiculoMasImplicado(IEnumerable<Accidente> accidentes)
    {
        return accidentes
            .GroupBy(a => a.TipoVehiculo)
            .Select(g => (Tipo: g.Key, Total: g.Count()))
            .MaxBy(a => a.Total);
    }
    // Accidentes con peatones
    public IEnumerable<string> AccidentesPeatones(IEnumerable<Accidente> accidentes)
    {
        return accidentes
        .Where(a => a.TipoPersona == TipoPersona.Peatón)
        .DistinctBy(a => a.NumExpediente).Select(a => a.NumExpediente);
    }
    // Proporción hombre/mujer
    public IEnumerable<(Sexo Sexo, int Total)> ProporcionHombreMujer(IEnumerable<Accidente> accidentes)
    {
        return accidentes
            .Where(a => a.TipoPersona == TipoPersona.Conductor)
            .GroupBy(a => a.Sexo)
            .Select(g => (Sexo: g.Key, Total: g.Count()));
    }
    // Distritos con más peatones
    public IEnumerable<(string Distrito, int Total)> DistritosConMasPeatones(IEnumerable<Accidente> accidentes)
    {
        return accidentes
            .Where(a => a.TipoPersona == TipoPersona.Peatón)
            .GroupBy(a => a.Distrito)
            .Select(g => (Distrito: g.Key, Total: g.Count()))
            .OrderByDescending(d => d.Total);
    }
    // Fin de semana vs entre semana
    public IEnumerable<(string TipoDia, int Total)> ComparacionDiasLaboralesVsFinde(IEnumerable<Accidente> accidentes)
    {
        return accidentes
            .GroupBy(a => a.Fecha.DayOfWeek == DayOfWeek.Saturday || a.Fecha.DayOfWeek == DayOfWeek.Sunday
                ? "Fin de semana"
                : "Entre semana")
            .Select(g => (TipoDia: g.Key, Total: g.Count()));
    }
    // Devuelve el promedio de accidentes por día (ej. 124.5 accidentes/día)
    public double MediaAccidentesPorDia(IEnumerable<Accidente> accidentes)
    { 
        return accidentes
            .GroupBy(a => a.Fecha)
            .Select(g => g.Count())
            .Average();
    }
    // Accidentes con alcohol + droga
    public double AccidentesDrogaYAlcohol(IEnumerable<Accidente> accidentes)
    {
        return accidentes.Count(a => a.PositivoAlcohol && a.PositivoDroga);
    }
    public IEnumerable<(string RangoEdad, int Total)> RangosEdadPeatonesVulnerables(IEnumerable<Accidente> accidentes)
    {
        return accidentes
            .Where(a => a.TipoPersona == TipoPersona.Peatón)
            .GroupBy(a => a.RangoEdad)
            .Select(g => (RangoEdad: g.Key, Total: g.Count()))
            .OrderByDescending(x => x.Total).Take(3);
    }
    
    //Distritos con más positivos en alcohol
    public IEnumerable<(string Distrito, int Total)> DistritosConMasPositivosAlcohol(IEnumerable<Accidente> accidentes)
    {
        return accidentes
            .Where(a => a.PositivoAlcohol)
            .DistinctBy(a => a.NumExpediente)
            .GroupBy(a => a.Distrito)
            .Select(g => (Distrito: g.Key, Total: g.Count()))
            .OrderByDescending(x => x.Total)
            .Take(5);
    }

    //Accidentes por código de distrito
    public IEnumerable<(int CodigoDistrito, int Total)> AccidentesPorCodigoDistrito(IEnumerable<Accidente> accidentes)
    {
        return accidentes
            .DistinctBy(a => a.NumExpediente)
            .GroupBy(a => a.CodDistrito)
            .Select(g => (CodigoDistrito: g.Key, Total: g.Count()))
            .OrderBy(x => x.CodigoDistrito);
    }

    //Accidentes por año
    public IEnumerable<(int Anio, int Total)> AccidentesPorAnio(IEnumerable<Accidente> accidentes)
    {
        return accidentes
            .DistinctBy(a => a.NumExpediente)
            .GroupBy(a => a.Fecha.Year)
            .Select(g => (Anio: g.Key, Total: g.Count()))
            .OrderBy(x => x.Anio);
    }

    //Evolución mensual por año (Año, Mes, Total)
    public IEnumerable<(int Anio, int Mes, int Total)> EvolucionMensualPorAnio(IEnumerable<Accidente> accidentes)
    {
        return accidentes
            .DistinctBy(a => a.NumExpediente)
            .GroupBy(a => (Anio: a.Fecha.Year, Mes: a.Fecha.Month))
            .Select(g => (Anio: g.Key.Anio, Mes: g.Key.Mes, Total: g.Count()))
            .OrderBy(x => x.Anio)
            .ThenBy(x => x.Mes);
    }

    //Distrito con más accidentes por año
    public IEnumerable<(int Anio, string Distrito, int Total)> DistritoConMasAccidentesPorAnio(IEnumerable<Accidente> accidentes)
    {
        return accidentes
            .DistinctBy(a => a.NumExpediente)
            .GroupBy(a => a.Fecha.Year)
            .Select(grupoAnio => {
                var topDistrito = grupoAnio
                    .GroupBy(a => a.Distrito)
                    .Select(g => (Distrito: g.Key, Total: g.Count()))
                    .OrderByDescending(x => x.Total)
                    .First();

                return (Anio: grupoAnio.Key, Distrito: topDistrito.Distrito, Total: topDistrito.Total);
            })
            .OrderBy(x => x.Anio);
    }

    //Tendencia de alcohol por año
    public IEnumerable<(int Anio, int TotalPositivos)> TendenciaAlcoholPorAnio(IEnumerable<Accidente> accidentes)
    {
        return accidentes
            .Where(a => a is { TipoPersona: TipoPersona.Conductor, PositivoAlcohol: true })
            .GroupBy(a => a.Fecha.Year)
            .Select(g => (Anio: g.Key, TotalPositivos: g.Count()))
            .OrderBy(x => x.Anio);
    }

    // 7. Comparativa fin de semana vs entre semana por año
    public IEnumerable<(int Anio, string TipoDia, int Total)> ComparativaFindeVsLaboralPorAnio(IEnumerable<Accidente> accidentes)
    {
        return accidentes
            .DistinctBy(a => a.NumExpediente)
            .GroupBy(a => (
                Anio: a.Fecha.Year,
                TipoDia: (a.Fecha.DayOfWeek == DayOfWeek.Saturday || a.Fecha.DayOfWeek == DayOfWeek.Sunday) 
                    ? "Fin de semana" 
                    : "Entre semana"
            ))
            .Select(g => (Anio: g.Key.Anio, TipoDia: g.Key.TipoDia, Total: g.Count()))
            .OrderBy(x => x.Anio)
            .ThenBy(x => x.TipoDia);
    }

    //Hora pico por año
    public IEnumerable<(int Anio, int HoraPico, int Total)> HoraPicoPorAnio(IEnumerable<Accidente> accidentes)
    {
        return accidentes
            .DistinctBy(a => a.NumExpediente)
            .GroupBy(a => a.Fecha.Year)
            .Select(grupoAnio => {
                var topHora = grupoAnio
                    .GroupBy(a => a.Fecha.Hour)
                    .Select(g => (Hora: g.Key, Total: g.Count()))
                    .OrderByDescending(x => x.Total)
                    .First();

                return (Anio: grupoAnio.Key, HoraPico: topHora.Hora, Total: topHora.Total);
            })
            .OrderBy(x => x.Anio);
    }

    //Lesión más frecuente por año
    public IEnumerable<(int Anio, Gravedad lesion, int Total)> LesionMasFrecuentePorAnio(IEnumerable<Accidente> accidentes)
    {
        return accidentes
            .GroupBy(a => a.Fecha.Year)
            .Select(grupoAnio => {
                var topLesion = grupoAnio
                    .GroupBy(a => a.Gravedad)
                    .Select(g => (Lesion: g.Key, Total: g.Count()))
                    .OrderByDescending(x => x.Total)
                    .First();

                return (Anio: grupoAnio.Key, Lesion: topLesion.Lesion, Total: topLesion.Total);
            })
            .OrderBy(x => x.Anio);
    }

    //Evolución de peatones por año
    public IEnumerable<(int Anio, int TotalPeatones)> EvolucionPeatonesPorAnio(IEnumerable<Accidente> accidentes)
    {
        return accidentes
            .Where(a => a.TipoPersona == TipoPersona.Peatón)
            .GroupBy(a => a.Fecha.Year)
            .Select(g => (Anio: g.Key, TotalPeatones: g.Count()))
            .OrderBy(x => x.Anio);
    }
}