using AccidentesMadrid.Models;

namespace AccidentesMadrid.Services.Accidentes;

public class AccidentesLinqAnalyzer()
{
    // Total de accidentes
    int TotalAccidentes(IEnumerable<Accidente> accidentes)
    {
        return accidentes.Count();
    }
    // Accidentes por distrito (top 5)
    IEnumerable<(string Distrito, int Total)> AccidentesPorDistrito(IEnumerable<Accidente> accidentes)
    {
        return accidentes
            .GroupBy(a => a.Distrito)
            .Select(g => (Distrito: g.Key, Total: g.Count()))
            .OrderByDescending(x => x.Total)
            .Take(5);
    }
    // Accidentes por tipo
    IEnumerable<(TipoAccidente TipoAccidente, int Total)> AccidentesPorTipo(IEnumerable<Accidente> accidentes)
    {
        return accidentes
            .GroupBy(a => a.TipoAccidente)
            .Select(g => (Tipo: g.Key, Total: g.Count()));
    }
    
    //Accidentes por estado meteorológico
    IEnumerable<(string EstadoMetereologico, int Total)> AccidentesPorEstadoMetereologico(IEnumerable<Accidente> accidentes)
    {
        return accidentes
            .GroupBy(a => a.EstadoMeteorologico)
            .Select(g => (EstadoMeteorologico: g.Key, Total: g.Count()));
    }
    
    // Accidentes por sexo
    IEnumerable<(Sexo Sexo, int Total)> AccidentesPorSexo(IEnumerable<Accidente> accidentes)
    {
        return accidentes
            .GroupBy(a => a.Sexo)
            .Select(g => (Sexo: g.Key, Total: g.Count()));
    }
    // Accidentes por rango de edad
    IEnumerable<(string RangoEdad, int Total)> AccidentesPorRangoEdad(IEnumerable<Accidente> accidentes)
    {
        return accidentes
            .GroupBy(a => a.RangoEdad)
            .Select(g => (RangoEdad: g.Key, Total: g.Count()));
    }
    // Positivos en alcohol
    int PositivosAlcohol(IEnumerable<Accidente> accidentes)
    {
        return accidentes.Count(a => !a.PositivoAlcohol);
    }
    // Positivos en drogas
    int PositivosDrogas(IEnumerable<Accidente> accidentes)
    {
        return accidentes.Count(a => !a.PositivoDroga);
    }
    // Accidentes por día de la semana
    IEnumerable<(DayOfWeek Dia, int Total)> AccidentesPorDiaDeLaSemana(IEnumerable<Accidente> accidentes)
    {
        return accidentes
            .GroupBy(a => a.Fecha.DayOfWeek)
            .Select(g => (Dia: g.Key, Total: g.Count()))
            .OrderByDescending(d => d.Dia);
    }
    // Accidentes por mes
    IEnumerable<(int Mes, int Total)> AccidentesPorMes(IEnumerable<Accidente> accidentes)
    {
        return accidentes
            .GroupBy(a => a.Fecha.Month)
            .Select(g => (Mes: g.Key, Total: g.Count()))
            .OrderBy(m => m.Mes);
    }
    // Hora con más accidentes
    (int Hora, int Total) HoraConMasAccidentes(IEnumerable<Accidente> accidentes)
    {
        return accidentes
            .GroupBy(a => a.Hora.Hours)
            .Select(g => (Hora: g.Key, Total: g.Count()))
            .MaxBy(a => a.Total);
    }
    // Lesiones más frecuentes
    (TipoAccidente TipoAccidente, int Total) LesionesMasFrecuentes(IEnumerable<Accidente> accidentes)
    {
        return accidentes
            .GroupBy(a => a.TipoAccidente)
            .Select(g => (Tipo: g.Key, Total: g.Count()))
            .MaxBy(a => a.Total);
    }
    // Tipo de vehículo más implicado
    (string TipoVehiculo, int Total) VehiculoMasImplicado(IEnumerable<Accidente> accidentes)
    {
        return accidentes
            .GroupBy(a => a.TipoVehiculo)
            .Select(g => (Tipo: g.Key, Total: g.Count()))
            .MaxBy(a => a.Total);
    }
    // Accidentes con peatones
    IEnumerable<Accidente> AccidentesPeatones(IEnumerable<Accidente> accidentes)
    {
        return accidentes
        .Where(a => a.TipoPersona == TipoPersona.Peatón)
        .DistinctBy(a => a.NumExpediente);
    }
    // Proporción hombre/mujer
    IEnumerable<(Sexo Sexo, int Total)> ProporcionHombreMujer(IEnumerable<Accidente> accidentes)
    {
        return accidentes
            .Where(a => a.TipoPersona == TipoPersona.Conductor)
            .GroupBy(a => a.Sexo)
            .Select(g => (Sexo: g.Key, Total: g.Count()));
    }
    // Distritos con más peatones
    IEnumerable<(string Districto, int Total)> DistritosConMasPeatones(IEnumerable<Accidente> accidentes)
    {
        return accidentes
            .GroupBy(a => a.Distrito)
            .Select(g => (Distrito: g.Key, Total: g.Count(a => a.TipoPersona == TipoPersona.Peatón)))
            .OrderByDescending(d => d.Total);
    }
    // Fin de semana vs entre semana
    IEnumerable<(string TipoDia, int Total)> ComparacionDiasLaboralesVsFinde(IEnumerable<Accidente> accidentes)
    {
        return accidentes
            .GroupBy(a => a.Fecha.DayOfWeek == DayOfWeek.Saturday || a.Fecha.DayOfWeek == DayOfWeek.Sunday
                ? "Fin de semana"
                : "Entre semana")
            .Select(g => (TipoDia: g.Key, Total: g.Count()));
    }
    // Devuelve el promedio de accidentes por día (ej. 124.5 accidentes/día)
    double MediaAccidentesPorDia(IEnumerable<Accidente> accidentes)
    { 
        return accidentes
            .GroupBy(a => a.Fecha)
            .Select(g => g.Count())
            .Average();
    }
    // Accidentes con alcohol + droga
    double AccidentesDrogaYAlcohol(IEnumerable<Accidente> accidentes)
    {
        return accidentes.Count(a => a.PositivoAlcohol && a.PositivoDroga);
    }
    IEnumerable<(string RangoEdad, int Total)> RangosEdadPeatonesVulnerables(IEnumerable<Accidente> accidentes)
    {
        return accidentes
            .Where(a => a.TipoPersona == TipoPersona.Peatón)
            .GroupBy(a => a.RangoEdad)
            .Select(g => (RangoEdad: g.Key, Total: g.Count()))
            .OrderByDescending(x => x.Total);
    }
    
    //Distritos con más positivos en alcohol
   public IEnumerable<(string Distrito, int Total)> DistritosConMasPositivosAlcohol(IEnumerable<Accidente> accidentes)
    {
        return accidentes
            .Where(a => a.PositivoAlcohol)
            .GroupBy(a => a.Distrito)
            .Select(g => (Distrito: g.Key, Total: g.Count()))
            .OrderByDescending(x => x.Total)
            .Take(5);
    }

    //Accidentes por código de distrito
    public IEnumerable<(int CodigoDistrito, int Total)> AccidentesPorCodigoDistrito(IEnumerable<Accidente> accidentes)
    {
        return accidentes
            .GroupBy(a => a.CodDistrito)
            .Select(g => (CodigoDistrito: g.Key, Total: g.Count()))
            .OrderBy(x => x.CodigoDistrito);
    }

    //Accidentes por año
    public IEnumerable<(int Anio, int Total)> AccidentesPorAnio(IEnumerable<Accidente> accidentes)
    {
        return accidentes
            .GroupBy(a => a.Fecha.Year)
            .Select(g => (Anio: g.Key, Total: g.Count()))
            .OrderBy(x => x.Anio);
    }

    //Evolución mensual por año (Año, Mes, Total)
    public IEnumerable<(int Anio, int Mes, int Total)> EvolucionMensualPorAnio(IEnumerable<Accidente> accidentes)
    {
        return accidentes
            .GroupBy(a => (Anio: a.Fecha.Year, Mes: a.Fecha.Month))
            .Select(g => (Anio: g.Key.Anio, Mes: g.Key.Mes, Total: g.Count()))
            .OrderBy(x => x.Anio)
            .ThenBy(x => x.Mes);
    }

    //Distrito con más accidentes por año
    public IEnumerable<(int Anio, string Distrito, int Total)> DistritoConMasAccidentesPorAnio(IEnumerable<Accidente> accidentes)
    {
        return accidentes
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
            .Where(a => a.PositivoAlcohol)
            .GroupBy(a => a.Fecha.Year)
            .Select(g => (Anio: g.Key, TotalPositivos: g.Count()))
            .OrderBy(x => x.Anio);
    }

    // 7. Comparativa fin de semana vs entre semana por año
    public IEnumerable<(int Anio, string TipoDia, int Total)> ComparativaFindeVsLaboralPorAnio(IEnumerable<Accidente> accidentes)
    {
        return accidentes
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