using AccidentesMadrid.Models;
using AccidentesMadrid.Services.Analyzers.Accidentes.Common;
using Microsoft.Data.Analysis;

namespace AccidentesMadrid.Services.Analyzers.Accidentes;

public class AccidentesDataFrameAnalyzer : IAccidentesAnalyzer.IAccidentesAnalyzer<DataFrame>
{
    public int TotalAccidentes(DataFrame accidentes)
    {
        return (int)accidentes.Rows.Count;
    }

    public IEnumerable<(string Distrito, int Total)> AccidentesPorDistrito(DataFrame accidentes)
    {
        var vistos = new HashSet<string>();
        var indicesUnicos = new List<long>();
        DataFrameColumn colExpediente = accidentes[Config.Config.NumExpediente];
        
        for (long i = 0; i < accidentes.Rows.Count; i++)
        {
            string id = colExpediente[i]?.ToString() ?? "";
            if (vistos.Add(id))
            {
                indicesUnicos.Add(i);
            }
        }

        var columnaIndices = new PrimitiveDataFrameColumn<long>("indices", indicesUnicos);
        DataFrame accidentesUnicos = accidentes[columnaIndices];
        
        DataFrame agrupado = accidentesUnicos
            .GroupBy(Config.Config.Distrito)
            .Count();

        if (agrupado.Rows.Count == 0 || agrupado.Columns.Count < 2) return Enumerable.Empty<(string, int)>();

        string colConteo = agrupado.Columns[1].Name;

        DataFrame top5 = agrupado
            .OrderByDescending(colConteo)
            .Head(5);
        
        return top5.Rows.Select(fila => (
            Distrito: fila[0]?.ToString() ?? "Desconocido",
            Total: Convert.ToInt32(fila[1])
        ));
    }

    public IEnumerable<(TipoAccidente TipoAccidente, int Total)> AccidentesPorTipo(DataFrame accidentes)
    {
        var vistos = new HashSet<string>();
        var indicesUnicos = new List<long>();
        DataFrameColumn colExpediente = accidentes[Config.Config.NumExpediente];

        for (long i = 0; i < accidentes.Rows.Count; i++)
        {
            if (vistos.Add(colExpediente[i]?.ToString() ?? ""))
            {
                indicesUnicos.Add(i);
            }
        }

        DataFrame accidentesUnicos = accidentes[new PrimitiveDataFrameColumn<long>("indices", indicesUnicos)];

        DataFrame agrupado = accidentesUnicos
            .GroupBy(Config.Config.TipoAccidente)
            .Count();

        if (agrupado.Rows.Count == 0 || agrupado.Columns.Count < 2) return Enumerable.Empty<(TipoAccidente, int)>();

        string colConteo = agrupado.Columns[1].Name;

        DataFrame resultado = agrupado.OrderByDescending(colConteo);

        return resultado.Rows.Select(fila => (
            TipoAccidente: Enum.TryParse<TipoAccidente>(fila[0]?.ToString(), true, out var tipo) ? tipo : TipoAccidente.OtrasCausas,
            Total: Convert.ToInt32(fila[1])
        ));
    }

    public IEnumerable<(string EstadoMetereologico, int Total)> AccidentesPorEstadoMetereologico(DataFrame accidentes)
    {
        var vistos = new HashSet<string>();
        var indicesUnicos = new List<long>();
        DataFrameColumn colExpediente = accidentes[Config.Config.NumExpediente];

        for (long i = 0; i < accidentes.Rows.Count; i++)
        {
            if (vistos.Add(colExpediente[i]?.ToString() ?? ""))
            {
                indicesUnicos.Add(i);
            }
        }

        DataFrame accidentesUnicos = accidentes[new PrimitiveDataFrameColumn<long>("indices", indicesUnicos)];

        DataFrame agrupado = accidentesUnicos
            .GroupBy(Config.Config.EstadoMeteorologico)
            .Count();

        if (agrupado.Rows.Count == 0 || agrupado.Columns.Count < 2) return Enumerable.Empty<(string, int)>();

        string colConteo = agrupado.Columns[1].Name;

        DataFrame resultado = agrupado.OrderByDescending(colConteo);

        return resultado.Rows.Select(fila => (
            EstadoMetereologico: fila[0]?.ToString() ?? "Desconocido",
            Total: Convert.ToInt32(fila[1])
        ));
    }

    public IEnumerable<(Sexo Sexo, int Total)> AccidentesPorSexo(DataFrame accidentes)
    {
        DataFrame agrupado = accidentes
            .GroupBy(Config.Config.Sexo)
            .Count();

        if (agrupado.Rows.Count == 0 || agrupado.Columns.Count < 2) return Enumerable.Empty<(Sexo, int)>();

        string colConteo = agrupado.Columns[1].Name;

        DataFrame resultado = agrupado.OrderByDescending(colConteo);

        return resultado.Rows.Select(fila => (
            Sexo: Enum.TryParse<Sexo>(fila[0]?.ToString(), true, out var sexo) ? sexo : Sexo.Desconocido,
            Total: Convert.ToInt32(fila[1])
        ));
    }

    public IEnumerable<(string RangoEdad, int Total)> AccidentesPorRangoEdad(DataFrame accidentes)
    {
        DataFrame agrupado = accidentes
            .GroupBy(Config.Config.RangoEdad)
            .Count();

        if (agrupado.Rows.Count == 0 || agrupado.Columns.Count < 2) return Enumerable.Empty<(string, int)>();

        string colConteo = agrupado.Columns[1].Name;

        DataFrame resultado = agrupado.OrderByDescending(colConteo);

        return resultado.Rows.Select(fila => (
            RangoEdad: fila[0]?.ToString() ?? "Desconocido",
            Total: Convert.ToInt32(fila[1])
        ));
    }

    public int PositivosAlcohol(DataFrame accidentes)
    {
        DataFrameColumn colAlcohol = accidentes[Config.Config.PositivaAlcohol];
        int conteo = 0;
        for (long i = 0; i < accidentes.Rows.Count; i++)
        {
            if (EsPositivo(colAlcohol[i])) conteo++;
        }
        return conteo;
    }

    public int PositivosDrogas(DataFrame accidentes)
    {
        DataFrameColumn colDroga = accidentes[Config.Config.PositivaDroga];
        int conteo = 0;
        for (long i = 0; i < accidentes.Rows.Count; i++)
        {
            if (EsPositivo(colDroga[i])) conteo++;
        }
        return conteo;
    }

    public IEnumerable<(DayOfWeek Dia, int Total)> AccidentesPorDiaDeLaSemana(DataFrame accidentes)
    {
        var vistos = new HashSet<string>();
        var indicesUnicos = new List<long>();
        DataFrameColumn colExpediente = accidentes[Config.Config.NumExpediente];

        for (long i = 0; i < accidentes.Rows.Count; i++)
        {
            if (vistos.Add(colExpediente[i]?.ToString() ?? ""))
            {
                indicesUnicos.Add(i);
            }
        }

        DataFrame accidentesUnicos = accidentes[new PrimitiveDataFrameColumn<long>("indices", indicesUnicos)];

        DataFrame porFecha = accidentesUnicos
            .GroupBy(Config.Config.Fecha)
            .Count();

        return porFecha.Rows
            .Select(fila => (
                Dia: DateTime.TryParse(fila[0]?.ToString(), out var f) ? f.DayOfWeek : DayOfWeek.Sunday,
                Cantidad: Convert.ToInt32(fila[1])
            ))
            .GroupBy(x => x.Dia)
            .Select(g => (
                Dia: g.Key,
                Total: g.Sum(x => x.Cantidad)
            ))
            .OrderByDescending(x => x.Total);
    }

    public IEnumerable<(int Mes, int Total)> AccidentesPorMes(DataFrame accidentes)
    {
        var vistos = new HashSet<string>();
        var indicesUnicos = new List<long>();
        DataFrameColumn colExpediente = accidentes[Config.Config.NumExpediente];

        for (long i = 0; i < accidentes.Rows.Count; i++)
        {
            if (vistos.Add(colExpediente[i]?.ToString() ?? ""))
            {
                indicesUnicos.Add(i);
            }
        }

        DataFrame accidentesUnicos = accidentes[new PrimitiveDataFrameColumn<long>("indices", indicesUnicos)];
        
        DataFrame porFecha = accidentesUnicos
            .GroupBy(Config.Config.Fecha)
            .Count();
        
        return porFecha.Rows
            .Select(fila => (
                Mes: DateTime.TryParse(fila[0]?.ToString(), out var f) ? f.Month : DateTime.MinValue.Month,
                Cantidad: Convert.ToInt32(fila[1])
            ))
            .GroupBy(x => x.Mes)
            .Select(g => (
                Mes: g.Key,
                Total: g.Sum(x => x.Cantidad)
            ))
            .OrderByDescending(x => x.Total);
    }

    public (int Hora, int Total)? HoraConMasAccidentes(DataFrame accidentes)
    {
        var vistos = new HashSet<string>();
        var conteosPorHora = new Dictionary<int, int>();

        DataFrameColumn colExpediente = accidentes[Config.Config.NumExpediente];
        DataFrameColumn colHora = accidentes[Config.Config.Hora];

        for (long i = 0; i < accidentes.Rows.Count; i++)
        {
            string exp = colExpediente[i]?.ToString() ?? "";
            if (vistos.Add(exp))
            {
                string horaStr = colHora[i]?.ToString() ?? "0";
                if (horaStr.Contains(':')) horaStr = horaStr.Split(':')[0];
            
                if (int.TryParse(horaStr, out int hora))
                {
                    conteosPorHora[hora] = conteosPorHora.GetValueOrDefault(hora) + 1;
                }
            }
        }

        if (conteosPorHora.Count == 0) return null;

        var top = conteosPorHora.OrderByDescending(x => x.Value).First();
        return (Hora: top.Key, Total: top.Value);
    }

    public (TipoAccidente TipoAccidente, int Total)? LesionesMasFrecuentes(DataFrame accidentes)
    {
        DataFrame agrupado = accidentes
            .GroupBy(Config.Config.Lesividad)
            .Count();

        if (agrupado.Rows.Count == 0 || agrupado.Columns.Count < 2) return null;

        string colConteo = agrupado.Columns[1].Name;

        DataFrame top1 = agrupado
            .OrderByDescending(colConteo)
            .Head(1);

        if (top1.Rows.Count == 0) return null;

        DataFrameRow fila = top1.Rows[0];
        string valLimpio = fila[0]?.ToString()?.Replace(" ", "").Replace("/", "").Replace("-", "") ?? "";

        return (
            TipoAccidente: Enum.TryParse<TipoAccidente>(valLimpio, true, out var tipo) ? tipo : TipoAccidente.OtrasCausas,
            Total: Convert.ToInt32(fila[1])
        );
    }

    public (string TipoVehiculo, int Total)? VehiculoMasImplicado(DataFrame accidentes)
    {
        DataFrame agrupado = accidentes
            .GroupBy(Config.Config.TipoVehiculo)
            .Count();

        if (agrupado.Rows.Count == 0 || agrupado.Columns.Count < 2) return null;

        string colConteo = agrupado.Columns[1].Name;

        DataFrame top1 = agrupado
            .OrderByDescending(colConteo)
            .Head(1);

        if (top1.Rows.Count == 0) return null;

        DataFrameRow fila = top1.Rows[0];

        return (
            TipoVehiculo: fila[0]?.ToString() ?? "Desconocido",
            Total: Convert.ToInt32(fila[1])
        );
    }

    public IEnumerable<string> AccidentesPeatones(DataFrame accidentes)
    {
        var vistos = new HashSet<string>();
        var expedientesPeatones = new List<string>();

        DataFrameColumn colExpediente = accidentes[Config.Config.NumExpediente];
        DataFrameColumn colTipoPersona = accidentes[Config.Config.TipoPersona];

        for (long i = 0; i < accidentes.Rows.Count; i++)
        {
            string tipo = colTipoPersona[i]?.ToString() ?? "";
            string exp = colExpediente[i]?.ToString() ?? "";

            if (tipo.Equals("Peatón", StringComparison.OrdinalIgnoreCase) && vistos.Add(exp))
            {
                expedientesPeatones.Add(exp);
            }
        }

        return expedientesPeatones;
    }

    public IEnumerable<(Sexo Sexo, int Total)> ProporcionHombreMujer(DataFrame accidentes)
    {
        DataFrame conductores = accidentes.Filter(
            accidentes[Config.Config.TipoPersona].ElementwiseEquals("Conductor")
        );
        
        DataFrame agrupado = conductores
            .GroupBy(Config.Config.Sexo)
            .Count();

        if (agrupado.Rows.Count == 0 || agrupado.Columns.Count < 2) return Enumerable.Empty<(Sexo, int)>();

        string colConteo = agrupado.Columns[1].Name;

        DataFrame resultado = agrupado.OrderByDescending(colConteo);

        return resultado.Rows.Select(fila => (
            Sexo: Enum.TryParse<Sexo>(fila[0]?.ToString(), true, out var sexo) ? sexo : Sexo.Desconocido,
            Total: Convert.ToInt32(fila[1])
        ));
    }

    public IEnumerable<(string Distrito, int Total)> DistritosConMasPeatones(DataFrame accidentes)
    {
        DataFrame peatones = accidentes.Filter(
            accidentes[Config.Config.TipoPersona].ElementwiseEquals("Peatón")
        );

        DataFrame agrupado = peatones
            .GroupBy(Config.Config.Distrito)
            .Count();

        if (agrupado.Rows.Count == 0 || agrupado.Columns.Count < 2) return Enumerable.Empty<(string, int)>();

        string colConteo = agrupado.Columns[1].Name;

        DataFrame resultado = agrupado.OrderByDescending(colConteo);

        return resultado.Rows.Select(fila => (
            Distrito: fila[0]?.ToString() ?? "Desconocido",
            Total: Convert.ToInt32(fila[1])
        ));
    }

    public IEnumerable<(string TipoDia, int Total)> ComparacionDiasLaboralesVsFinde(DataFrame accidentes)
    {
        DataFrame porFecha = accidentes
            .GroupBy(Config.Config.Fecha)
            .Count();

        return porFecha.Rows
            .Select(fila => (
                TipoDia: DateTime.TryParse(fila[0]?.ToString(), out var f) && 
                         (f.DayOfWeek == DayOfWeek.Saturday || f.DayOfWeek == DayOfWeek.Sunday)
                    ? "Fin de semana"
                    : "Entre semana",
                Cantidad: Convert.ToInt32(fila[1])
            ))
            .GroupBy(x => x.TipoDia)
            .Select(g => (
                TipoDia: g.Key,
                Total: g.Sum(x => x.Cantidad)
            ))
            .OrderByDescending(x => x.Total);
    }

    public double MediaAccidentesPorDia(DataFrame accidentes)
    {
        DataFrame porFecha = accidentes
            .GroupBy(Config.Config.Fecha)
            .Count();

        if (porFecha.Rows.Count == 0) return 0.0;

        return porFecha.Rows
            .Select(fila => Convert.ToDouble(fila[1]))
            .Average();
    }

    public double AccidentesDrogaYAlcohol(DataFrame accidentes)
    {
        var vistos = new HashSet<string>();
    
        DataFrameColumn colExpediente = accidentes[Config.Config.NumExpediente];
        DataFrameColumn colAlcohol = accidentes[Config.Config.PositivaAlcohol];
        DataFrameColumn colDroga = accidentes[Config.Config.PositivaDroga];

        long accidentesConDoblePositivo = 0;

        for (long i = 0; i < accidentes.Rows.Count; i++)
        {
            bool esAlcohol = EsPositivo(colAlcohol[i]);
            bool esDroga = EsPositivo(colDroga[i]);
            
            if (esAlcohol && esDroga)
            {
                string exp = colExpediente[i]?.ToString() ?? "";
            
                if (vistos.Add(exp))
                {
                    accidentesConDoblePositivo++;
                }
            }
        }
        return Convert.ToDouble(accidentesConDoblePositivo);
    }

    public IEnumerable<(string RangoEdad, int Total)> RangosEdadPeatonesVulnerables(DataFrame accidentes)
    {
        DataFrame peatones = accidentes.Filter(
            accidentes[Config.Config.TipoPersona].ElementwiseEquals("Peatón")
        );

        DataFrame agrupado = peatones
            .GroupBy(Config.Config.RangoEdad)
            .Count();

        if (agrupado.Rows.Count == 0 || agrupado.Columns.Count < 2) return Enumerable.Empty<(string, int)>();

        string colConteo = agrupado.Columns[1].Name;

        DataFrame resultado = agrupado
            .OrderByDescending(colConteo)
            .Head(3);

        return resultado.Rows.Select(fila => (
            RangoEdad: fila[0]?.ToString() ?? "Desconocido",
            Total: Convert.ToInt32(fila[1])
        ));
    }

    public IEnumerable<(string Distrito, int Total)> DistritosConMasPositivosAlcohol(DataFrame accidentes)
    {
        var vistos = new HashSet<string>();
        var indicesUnicos = new List<long>();

        DataFrameColumn colExpediente = accidentes[Config.Config.NumExpediente];
        DataFrameColumn colAlcohol = accidentes[Config.Config.PositivaAlcohol];

        for (long i = 0; i < accidentes.Rows.Count; i++)
        {
            if (EsPositivo(colAlcohol[i]))
            {
                string exp = colExpediente[i]?.ToString() ?? "";
                if (vistos.Add(exp))
                {
                    indicesUnicos.Add(i);
                }
            }
        }

        if (indicesUnicos.Count == 0) return Enumerable.Empty<(string, int)>();

        DataFrame accidentesUnicos = accidentes[new PrimitiveDataFrameColumn<long>("indices", indicesUnicos)];

        DataFrame agrupado = accidentesUnicos
            .GroupBy(Config.Config.Distrito)
            .Count();

        if (agrupado.Rows.Count == 0 || agrupado.Columns.Count < 2) return Enumerable.Empty<(string, int)>();

        string colConteo = agrupado.Columns[1].Name;

        DataFrame resultado = agrupado
            .OrderByDescending(colConteo)
            .Head(5);

        return resultado.Rows.Select(fila => (
            Distrito: fila[0]?.ToString() ?? "Desconocido",
            Total: Convert.ToInt32(fila[1])
        ));
    }

    public IEnumerable<(int CodigoDistrito, int Total)> AccidentesPorCodigoDistrito(DataFrame accidentes)
    {
        var vistos = new HashSet<string>();
        var indicesUnicos = new List<long>();
        DataFrameColumn colExpediente = accidentes[Config.Config.NumExpediente];

        for (long i = 0; i < accidentes.Rows.Count; i++)
        {
            if (vistos.Add(colExpediente[i]?.ToString() ?? ""))
            {
                indicesUnicos.Add(i);
            }
        }

        DataFrame accidentesUnicos = accidentes[new PrimitiveDataFrameColumn<long>("indices", indicesUnicos)];
    
        DataFrame agrupado = accidentesUnicos
            .GroupBy(Config.Config.CodDistrito)
            .Count();

        return agrupado.Rows
            .Select(fila => (
                CodigoDistrito: int.TryParse(fila[0]?.ToString(), out var cod) ? cod : 0,
                Total: Convert.ToInt32(fila[1])
            ))
            .OrderBy(x => x.CodigoDistrito);
    }

    public IEnumerable<(int Anio, int Total)> AccidentesPorAnio(DataFrame accidentes)
    {
        var vistos = new HashSet<string>();
        var indicesUnicos = new List<long>();
        DataFrameColumn colExpediente = accidentes[Config.Config.NumExpediente];

        for (long i = 0; i < accidentes.Rows.Count; i++)
        {
            if (vistos.Add(colExpediente[i]?.ToString() ?? ""))
            {
                indicesUnicos.Add(i);
            }
        }

        DataFrame accidentesUnicos = accidentes[new PrimitiveDataFrameColumn<long>("indices", indicesUnicos)];

        DataFrame porFecha = accidentesUnicos
            .GroupBy(Config.Config.Fecha)
            .Count();

        return porFecha.Rows
            .Select(fila => (
                Anio: DateTime.TryParse(fila[0]?.ToString(), out var f) ? f.Year : 0,
                Cantidad: Convert.ToInt32(fila[1])
            ))
            .GroupBy(x => x.Anio)
            .Select(g => (Anio: g.Key, Total: g.Sum(x => x.Cantidad)))
            .OrderBy(x => x.Anio);
    }

    public IEnumerable<(int Anio, int Mes, int Total)> EvolucionMensualPorAnio(DataFrame accidentes)
    {
        var vistos = new HashSet<string>();
        var indicesUnicos = new List<long>();
        DataFrameColumn colExpediente = accidentes[Config.Config.NumExpediente];

        for (long i = 0; i < accidentes.Rows.Count; i++)
        {
            if (vistos.Add(colExpediente[i]?.ToString() ?? ""))
            {
                indicesUnicos.Add(i);
            }
        }

        DataFrame accidentesUnicos = accidentes[new PrimitiveDataFrameColumn<long>("indices", indicesUnicos)];

        DataFrame porFecha = accidentesUnicos
            .GroupBy(Config.Config.Fecha)
            .Count();

        return porFecha.Rows
            .Select(fila => {
                DateTime.TryParse(fila[0]?.ToString(), out var f);
                return (Anio: f.Year, Mes: f.Month, Cantidad: Convert.ToInt32(fila[1]));
            })
            .GroupBy(x => (x.Anio, x.Mes))
            .Select(g => (Anio: g.Key.Anio, Mes: g.Key.Mes, Total: g.Sum(x => x.Cantidad)))
            .OrderBy(x => x.Anio)
            .ThenBy(x => x.Mes);
    }

    public IEnumerable<(int Anio, string Distrito, int Total)> DistritoConMasAccidentesPorAnio(DataFrame accidentes)
    {
        var vistos = new HashSet<string>();
        var conteos = new Dictionary<(int Anio, string Distrito), int>();

        DataFrameColumn colExpediente = accidentes[Config.Config.NumExpediente];
        DataFrameColumn colFecha = accidentes[Config.Config.Fecha];
        DataFrameColumn colDistrito = accidentes[Config.Config.Distrito];

        for (long i = 0; i < accidentes.Rows.Count; i++)
        {
            string exp = colExpediente[i]?.ToString() ?? "";
            
            if (vistos.Add(exp))
            {
                int anio = DateTime.TryParse(colFecha[i]?.ToString(), out var f) ? f.Year : 0;
                string distrito = colDistrito[i]?.ToString() ?? "Desconocido";

                var clave = (Anio: anio, Distrito: distrito);
                conteos[clave] = conteos.GetValueOrDefault(clave) + 1;
            }
        }

        return conteos
            .GroupBy(kvp => kvp.Key.Anio)
            .Select(grupoAnio => grupoAnio.OrderByDescending(x => x.Value).First())
            .Select(top => (Anio: top.Key.Anio, Distrito: top.Key.Distrito, Total: top.Value))
            .OrderBy(x => x.Anio);
    }

    public IEnumerable<(int Anio, int TotalPositivos)> TendenciaAlcoholPorAnio(DataFrame accidentes)
    {
        DataFrameColumn colTipo = accidentes[Config.Config.TipoPersona];
        DataFrameColumn colAlcohol = accidentes[Config.Config.PositivaAlcohol];
        DataFrameColumn colFecha = accidentes[Config.Config.Fecha];

        var conteosPorAnio = new Dictionary<int, int>();

        for (long i = 0; i < accidentes.Rows.Count; i++)
        {
            bool esConductor = "Conductor".Equals(colTipo[i]?.ToString(), StringComparison.OrdinalIgnoreCase);
            bool esAlcohol = EsPositivo(colAlcohol[i]);

            if (esConductor && esAlcohol)
            {
                int anio = DateTime.TryParse(colFecha[i]?.ToString(), out var f) ? f.Year : 0;
                conteosPorAnio[anio] = conteosPorAnio.GetValueOrDefault(anio) + 1;
            }
        }

        return conteosPorAnio
            .Select(kvp => (Anio: kvp.Key, TotalPositivos: kvp.Value))
            .OrderBy(x => x.Anio);
    }

    public IEnumerable<(int Anio, string TipoDia, int Total)> ComparativaFindeVsLaboralPorAnio(DataFrame accidentes)
    {
        DataFrame porFecha = accidentes
            .GroupBy(Config.Config.Fecha)
            .Count();

        return porFecha.Rows
            .Select(fila => {
                DateTime.TryParse(fila[0]?.ToString(), out var f);
                string tipoDia = (f.DayOfWeek == DayOfWeek.Saturday || f.DayOfWeek == DayOfWeek.Sunday)
                    ? "Fin de semana"
                    : "Entre semana";
                return (Anio: f.Year, TipoDia: tipoDia, Cantidad: Convert.ToInt32(fila[1]));
            })
            .GroupBy(x => (x.Anio, x.TipoDia))
            .Select(g => (Anio: g.Key.Anio, TipoDia: g.Key.TipoDia, Total: g.Sum(x => x.Cantidad)))
            .OrderBy(x => x.Anio)
            .ThenBy(x => x.TipoDia);
    }

    public IEnumerable<(int Anio, int HoraPico, int Total)> HoraPicoPorAnio(DataFrame accidentes)
    {
        var vistos = new HashSet<string>();
        var conteos = new Dictionary<(int Anio, int Hora), int>();

        DataFrameColumn colExpediente = accidentes[Config.Config.NumExpediente];
        DataFrameColumn colFecha = accidentes[Config.Config.Fecha];
        DataFrameColumn colHora = accidentes[Config.Config.Hora];

        for (long i = 0; i < accidentes.Rows.Count; i++)
        {
            string exp = colExpediente[i]?.ToString() ?? "";
        
            if (vistos.Add(exp))
            {
                int anio = DateTime.TryParse(colFecha[i]?.ToString(), out var f) ? f.Year : 0;
            
                string horaStr = colHora[i]?.ToString() ?? "0";
                if (horaStr.Contains(':')) horaStr = horaStr.Split(':')[0];
                int hora = int.TryParse(horaStr, out var h) ? h : 0;

                var clave = (Anio: anio, Hora: hora);
                conteos[clave] = conteos.GetValueOrDefault(clave) + 1;
            }
        }

        return conteos
            .GroupBy(kvp => kvp.Key.Anio)
            .Select(grupoAnio => grupoAnio.OrderByDescending(x => x.Value).First())
            .Select(top => (Anio: top.Key.Anio, HoraPico: top.Key.Hora, Total: top.Value))
            .OrderBy(x => x.Anio);
    }

    public IEnumerable<(int Anio, Gravedad Lesion, int Total)> LesionMasFrecuentePorAnio(DataFrame accidentes)
    {
        var conteos = new Dictionary<(int Anio, Gravedad Lesion), int>();

        DataFrameColumn colFecha = accidentes[Config.Config.Fecha];
        DataFrameColumn colLesividad = accidentes[Config.Config.Lesividad];

        long totalFilas = accidentes.Rows.Count;

        for (long i = 0; i < totalFilas; i++)
        {
            object? valFecha = colFecha[i];
            object? valLesidad = colLesividad[i];

            if (valFecha == null || valLesidad == null) continue;

            int anio = 0;
            if (valFecha is DateTime dt)
            {
                anio = dt.Year;
            }
            else if (DateTime.TryParse(valFecha.ToString(), out var f))
            {
                anio = f.Year;
            }

            if (anio == 0) continue;

            string textoLimpio = valLesidad.ToString()?.Replace(" ", "").Replace("/", "").Replace("-", "") ?? "";

            if (Enum.TryParse<Gravedad>(textoLimpio, true, out var gravedad))
            {
                var clave = (Anio: anio, Lesion: gravedad);
                conteos[clave] = conteos.GetValueOrDefault(clave) + 1;
            }
        }

        return conteos
            .GroupBy(kvp => kvp.Key.Anio)
            .Select(grupoAnio => grupoAnio.OrderByDescending(x => x.Value).First())
            .Select(top => (Anio: top.Key.Anio, Lesion: top.Key.Lesion, Total: top.Value))
            .OrderBy(x => x.Anio);
    }

    public IEnumerable<(int Anio, int TotalPeatones)> EvolucionPeatonesPorAnio(DataFrame accidentes)
    {
        DataFrame peatones = accidentes.Filter(
            accidentes[Config.Config.TipoPersona].ElementwiseEquals("Peatón")
        );

        var conteosPorAnio = new Dictionary<int, int>();
        DataFrameColumn colFecha = peatones[Config.Config.Fecha];

        for (long i = 0; i < peatones.Rows.Count; i++)
        {
            if (DateTime.TryParse(colFecha[i]?.ToString(), out var f))
            {
                conteosPorAnio[f.Year] = conteosPorAnio.GetValueOrDefault(f.Year) + 1;
            }
        }

        return conteosPorAnio
            .Select(kvp => (Anio: kvp.Key, TotalPeatones: kvp.Value))
            .OrderBy(x => x.Anio);
    }

    private static bool EsPositivo(object? valor)
    {
        if (valor == null) return false;
        string str = valor.ToString()?.Trim() ?? "";
        return str.Equals("S", StringComparison.OrdinalIgnoreCase) ||
               str.Equals("SI", StringComparison.OrdinalIgnoreCase) ||
               str.Equals("1", StringComparison.OrdinalIgnoreCase) ||
               str.Equals("True", StringComparison.OrdinalIgnoreCase);
    }
}