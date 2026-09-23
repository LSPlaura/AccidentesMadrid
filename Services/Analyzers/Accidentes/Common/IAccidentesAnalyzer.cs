using AccidentesMadrid.Models;

namespace AccidentesMadrid.Services.Analyzers.Accidentes.Common;

public interface IAccidentesAnalyzer
{
    public interface IAccidentesAnalyzer<TData>
{
    int TotalAccidentes(TData accidentes);
    IEnumerable<(string Distrito, int Total)> AccidentesPorDistrito(TData accidentes);
    IEnumerable<(TipoAccidente TipoAccidente, int Total)> AccidentesPorTipo(TData accidentes);
    IEnumerable<(string EstadoMetereologico, int Total)> AccidentesPorEstadoMetereologico(TData accidentes);
    IEnumerable<(Sexo Sexo, int Total)> AccidentesPorSexo(TData accidentes);
    IEnumerable<(string RangoEdad, int Total)> AccidentesPorRangoEdad(TData accidentes);
    int PositivosAlcohol(TData accidentes);
    int PositivosDrogas(TData accidentes);
    IEnumerable<(DayOfWeek Dia, int Total)> AccidentesPorDiaDeLaSemana(TData accidentes);
    IEnumerable<(int Mes, int Total)> AccidentesPorMes(TData accidentes);
    (int Hora, int Total)? HoraConMasAccidentes(TData accidentes);
    (TipoAccidente TipoAccidente, int Total)? LesionesMasFrecuentes(TData accidentes);
    (string TipoVehiculo, int Total)? VehiculoMasImplicado(TData accidentes);
    IEnumerable<string> AccidentesPeatones(TData accidentes);
    IEnumerable<(Sexo Sexo, int Total)> ProporcionHombreMujer(TData accidentes);
    IEnumerable<(string Distrito, int Total)> DistritosConMasPeatones(TData accidentes);
    IEnumerable<(string TipoDia, int Total)> ComparacionDiasLaboralesVsFinde(TData accidentes);
    double MediaAccidentesPorDia(TData accidentes);
    double AccidentesDrogaYAlcohol(TData accidentes);
    IEnumerable<(string RangoEdad, int Total)> RangosEdadPeatonesVulnerables(TData accidentes);
    IEnumerable<(string Distrito, int Total)> DistritosConMasPositivosAlcohol(TData accidentes);
    IEnumerable<(int CodigoDistrito, int Total)> AccidentesPorCodigoDistrito(TData accidentes);
    IEnumerable<(int Anio, int Total)> AccidentesPorAnio(TData accidentes);
    IEnumerable<(int Anio, int Mes, int Total)> EvolucionMensualPorAnio(TData accidentes);
    IEnumerable<(int Anio, string Distrito, int Total)> DistritoConMasAccidentesPorAnio(TData accidentes);
    IEnumerable<(int Anio, int TotalPositivos)> TendenciaAlcoholPorAnio(TData accidentes);
    IEnumerable<(int Anio, string TipoDia, int Total)> ComparativaFindeVsLaboralPorAnio(TData accidentes);
    IEnumerable<(int Anio, int HoraPico, int Total)> HoraPicoPorAnio(TData accidentes);
    IEnumerable<(int Anio, Gravedad Lesion, int Total)> LesionMasFrecuentePorAnio(TData accidentes);
    IEnumerable<(int Anio, int TotalPeatones)> EvolucionPeatonesPorAnio(TData accidentes);
}
}