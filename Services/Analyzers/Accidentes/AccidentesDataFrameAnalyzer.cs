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
        //toma la columna solo de numeros de expediente
        DataFrameColumn colExpediente = accidentes[Config.Config.NumExpediente];
        
        //itera sobre la columna
        for (long i = 0; i < accidentes.Rows.Count; i++)
        {
            //obtiene el nombre del expediente
            string id = colExpediente[i]?.ToString() ?? "";
            //lo añade al hashet que si no se habia metido devuelve true (sí, lo puedo insertar) o false en caso contrario (no, está duplicado)
            //si lo puedes insertar
            if (vistos.Add(id))
            {
                //añades el indice a la lista de indices
                indicesUnicos.Add(i);
            }
        }
        //Convierte la List<long> a una columna nativa del DataFrame del tipo long para poder friltrar
        var columnaIndices = new PrimitiveDataFrameColumn<long>("indices", indicesUnicos);
        //ahora el dataframe está filtrado (como un distinct)
        DataFrame accidentesUnicos = accidentes[columnaIndices];
        
        //Filtra por distrito, cuenta los accidentes en cada distrito (el dataframe ahora consta de dos columnas (distrito y distrito_Count).
        //Se ordenada descendentemente para obtener los distritos con más accidentes y se toman los 5 primeros
        DataFrame top5 = accidentesUnicos
            .GroupBy(Config.Config.Distrito)
            .Count()
            .OrderByDescending(Config.Config.Distrito +"_Count")
            .Head(5);
        
        //se mapea a tupla, luego de haber realizado las operaciones pesadas mediante el dataframe esta operacion no le resta eficiencia
        return top5.Rows.Select(fila => (
            Distrito: fila[0]?.ToString() ?? "Desconocido",
            Total: Convert.ToInt32(fila[1])
        ));
        //es el distinct eficiente? si, ya que al hashet tiene un algoritmo temporal o(1) (tiempo constante) además la carga en memoria es poca ya que se pasan datas primitivos como strings y long.
        //Por otro lado al haber recortado el dataframe las operaciones posteriores se realizarán sobre menos columnas y por lo tanto tardará menos.
        //Las gallinas qeu entran por las que salen, dicen :)
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

        DataFrame resultado = accidentesUnicos
            .GroupBy(Config.Config.TipoAccidente)
            .Count()
            .OrderByDescending(Config.Config.TipoAccidente + "_Count");

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

        DataFrame resultado = accidentesUnicos
            .GroupBy(Config.Config.EstadoMeteorologico)
            .Count()
            .OrderByDescending(Config.Config.EstadoMeteorologico + "_Count");

        return resultado.Rows.Select(fila => (
            EstadoMetereologico: fila[0]?.ToString() ?? "Desconocido",
            Total: Convert.ToInt32(fila[1])
        ));
    }

    //personas que se ven implicadas en un accidente por sexo
    public IEnumerable<(Sexo Sexo, int Total)> AccidentesPorSexo(DataFrame accidentes)
    {
        // El trabajo pesado de reducción (N filas -> K categorías) lo realiza el DataFrame en memoria contigua (en la ram los datos ordenados uno detras de otro sin saltos ni huecos).
        // La conversión con Enum.TryParse solo se ejecuta K veces (K <= 3 categorías de sexo), 
        // resultando en un impacto temporal prácticamente constante O(1) e imperceptible en la RAM.
        //mapear a tuplas tampoco empeora la eficiencia de la operación ya que las tuplas se almacenan en la pila
        //Se reservan en registros ultrarrápidos del procesador y se destruyen instantáneamente al salir del método sin ensuciar la RAM.
        //Además tampoco activa el garbage collector
        DataFrame resultado = accidentes
            .GroupBy(Config.Config.Sexo)
            .Count()
            .OrderByDescending(Config.Config.Sexo + "_Count");

        return resultado.Rows.Select(fila => (
            Sexo: Enum.TryParse<Sexo>(fila[0]?.ToString(), true, out var sexo) ? sexo : Sexo.Desconocido,
            Total: Convert.ToInt32(fila[1])
        ));
    }

    //personas que se ven implicadas en un accidente por rango de edad
    public IEnumerable<(string RangoEdad, int Total)> AccidentesPorRangoEdad(DataFrame accidentes)
    {
        DataFrame resultado = accidentes
            .GroupBy(Config.Config.RangoEdad)
            .Count()
            .OrderByDescending(Config.Config.RangoEdad + "_Count");

        return resultado.Rows.Select(fila => (
            RangoEdad: fila[0]?.ToString() ?? "Desconocido",
            Total: Convert.ToInt32(fila[1])
        ));
    }
    public int PositivosAlcohol(DataFrame accidentes)
    {
        DataFrame positivos = accidentes.Filter(
            accidentes[Config.Config.PositivaAlcohol].ElementwiseEquals("true")
        );
        return Convert.ToInt32(positivos.Rows.Count);
    }

    public int PositivosDrogas(DataFrame accidentes)
    {
        DataFrame positivos = accidentes.Filter(
            accidentes[Config.Config.PositivaDroga].ElementwiseEquals("true")
        );
        return Convert.ToInt32(positivos.Rows.Count);
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

        //El DataFrame agrupa primero por fecha (se reduce de N a ~365 filas)
        DataFrame porFecha = accidentesUnicos
            .GroupBy(Config.Config.Fecha)
            .Count();

        //Se agrupa por DayOfWeek mediante linq (operación O(365) ultra rápida)
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

        DataFrame top1 = accidentesUnicos
            .GroupBy(Config.Config.Hora)
            .Count()
            .OrderByDescending(Config.Config.Hora + "_Count")
            .Head(1);

        if (top1.Rows.Count == 0) return null;

        DataFrameRow fila = top1.Rows[0];
        string horaStr = fila[0]?.ToString() ?? "0";
        
        if (horaStr.Contains(':'))
        {
            horaStr = horaStr.Split(':')[0];
        }

        return (
            Hora: int.TryParse(horaStr, out var h) ? h : 0,
            Total: Convert.ToInt32(fila[1])
        );
    }

    public (TipoAccidente TipoAccidente, int Total)? LesionesMasFrecuentes(DataFrame accidentes)
    {
        DataFrame top1 = accidentes
            .GroupBy(Config.Config.Lesividad)
            .Count()
            .OrderByDescending(Config.Config.Lesividad+ "_Count")
            .Head(1);

        if (top1.Rows.Count == 0) return null;

        DataFrameRow fila = top1.Rows[0];

        return (
            TipoAccidente: Enum.TryParse<TipoAccidente>(fila[0]?.ToString(), true, out var tipo) ? tipo : TipoAccidente.OtrasCausas,
            Total: Convert.ToInt32(fila[1])
        );
    }

    public (string TipoVehiculo, int Total)? VehiculoMasImplicado(DataFrame accidentes)
    {
        DataFrame top1 = accidentes
            .GroupBy(Config.Config.TipoVehiculo)
            .Count()
            .OrderByDescending(Config.Config.TipoVehiculo + "_Count")
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
        
        DataFrame resultado = conductores
            .GroupBy(Config.Config.Sexo)
            .Count()
            .OrderByDescending(Config.Config.Sexo + "_Count");
        
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

        DataFrame resultado = peatones
            .GroupBy(Config.Config.Distrito)
            .Count()
            .OrderByDescending(Config.Config.Distrito + "_Count");
        
        return resultado.Rows.Select(fila => (
            Distrito: fila[0]?.ToString() ?? "Desconocido",
            Total: Convert.ToInt32(fila[1])
        ));
    }

    public IEnumerable<(string TipoDia, int Total)> ComparacionDiasLaboralesVsFinde(DataFrame accidentes)
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

        // Agrupamos los accidentes únicos por fecha y contamos cuántos ocurrieron cada día
        DataFrame porFecha = accidentesUnicos
            .GroupBy(Config.Config.Fecha)
            .Count();

        if (porFecha.Rows.Count == 0) return 0.0;

        // Calculamos el promedio sobre la columna de conteos (fecha_Count)
        return porFecha.Rows
            .Select(fila => Convert.ToDouble(fila[1]))
            .Average();
    }

    public double AccidentesDrogaYAlcohol(DataFrame accidentes)
    {
        //al realizar la operacion mediante un for la complejidad algoritma es o(n) lo que resulta más optimo que crear sub dataframes intermedios en memoria
        var vistos = new HashSet<string>();
    
        DataFrameColumn colExpediente = accidentes[Config.Config.NumExpediente];
        DataFrameColumn colAlcohol = accidentes[Config.Config.PositivaAlcohol];
        DataFrameColumn colDroga = accidentes[Config.Config.PositivaDroga];

        long accidentesConDoblePositivo = 0;

        for (long i = 0; i < accidentes.Rows.Count; i++)
        {
            string alc = colAlcohol[i]?.ToString() ?? "";
            string drog = colDroga[i]?.ToString() ?? "";

            bool esAlcohol = alc.Equals("true", StringComparison.OrdinalIgnoreCase);

            bool esDroga = drog.Equals("true", StringComparison.OrdinalIgnoreCase);
            
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
        
        DataFrame resultado = peatones
            .GroupBy(Config.Config.RangoEdad)
            .Count()
            .OrderByDescending(Config.Config.RangoEdad + "_Count")
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
            string alc = colAlcohol[i]?.ToString() ?? "";
            bool esAlcohol = alc.Equals("true", StringComparison.OrdinalIgnoreCase);
            if (esAlcohol)
            {
                string exp = colExpediente[i]?.ToString() ?? "";
                if (vistos.Add(exp))
                {
                    indicesUnicos.Add(i);
                }
            }
        }

        DataFrame accidentesUnicos = accidentes[new PrimitiveDataFrameColumn<long>("indices", indicesUnicos)];

        DataFrame resultado = accidentesUnicos
            .GroupBy(Config.Config.Distrito)
            .Count()
            .OrderByDescending(Config.Config.Distrito + "_Count")
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
        
        DataFrame resultado = accidentesUnicos
            .GroupBy(Config.Config.CodDistrito)
            .Count()
            .OrderBy(Config.Config.CodDistrito);

        return resultado.Rows.Select(fila => (
            CodigoDistrito: int.TryParse(fila[0]?.ToString(), out var cod) ? cod : 0,
            Total: Convert.ToInt32(fila[1])
        ));
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
            bool esAlcohol = "true".Equals(colAlcohol[i]?.ToString(), StringComparison.OrdinalIgnoreCase);

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
        // Se crea un diccionario que tiene como clave una tupla (Anio, Lesio) para acumular conteos en una sola pasada O(N)
        var conteos = new Dictionary<(int Anio, Gravedad Lesion), int>();

        //Se obtienen ambas columnas
        //extraer la referencia a la columna antes del bucle es un patrón de optimización
        //en caso contrario tiene que buscar internamente en su diccionario de nombres cuál es el índice numérico de la columna en cuestión
        //En un dataset de 300.000 filas, realizas 300.000 búsquedas por clave de texto y miles de asignaciones temporales en memoria RAM.
        //Al extraer la columna solo busca una vez en su diccionarion antes de entrar al bucle.
        //una vez en el bucle realiza un acceso indexado directo por posición (Dirección = Inicio + (i * tamaño)) a la memoria contigua de la columna, con complejidad O(1) (tiempo constante)
        DataFrameColumn colFecha = accidentes[Config.Config.Fecha];
        DataFrameColumn colLesividad = accidentes[Config.Config.Lesividad];

        //Se itara el dateframe original, como todos tienen el mismo numero de filas no habrá incongruencias entre los datos (tomar uno incorrecto)
        for (long i = 0; i < accidentes.Rows.Count; i++)
        {
            //Se va comprobando el columna deseada el año y la lesion
            int anio = DateTime.TryParse(colFecha[i]?.ToString(), out var f) ? f.Year : 0;
        
            if (Enum.TryParse<Gravedad>(colLesividad[i]?.ToString(), true, out var gravedad))
            {
                //se accede a la clave, en el caso de que no exista se crea la clave y se le asigna el valor por defecto
                //como el valor a guardar es un int en el caso de que se crease la clave se inicializaria con valor 0 por lo que no habria problema en ese caso y se puede sumar 1 al conteo.
                conteos[(Anio: anio, Lesion: gravedad)] = conteos.GetValueOrDefault((Anio: anio, Lesion: gravedad)) + 1;
            }
        }

        // Procesamos el diccionario agrupado por Año para extraer el Top 1 de cada año
        return conteos
            .GroupBy(dicClaveAnio => dicClaveAnio.Key.Anio)
            .Select(grupoAnio => grupoAnio.OrderByDescending(x => x.Value).First())
            .Select(top => (Anio: top.Key.Anio, Lesion: top.Key.Lesion, Total: top.Value))
            .OrderBy(x => x.Anio);
    }

    public IEnumerable<(int Anio, int TotalPeatones)> EvolucionPeatonesPorAnio(DataFrame accidentes)
    {
        DataFrame peatones = accidentes.Filter(
            accidentes[Config.Config.TipoPersona].ElementwiseEquals("Peatón")
        );

        DataFrame porFecha = peatones
            .GroupBy(Config.Config.Fecha)
            .Count();

        return porFecha.Rows
            .Select(fila => (
                Anio: DateTime.TryParse(fila[0]?.ToString(), out var f) ? f.Year : 0,
                Cantidad: Convert.ToInt32(fila[1])
            ))
            .GroupBy(x => x.Anio)
            .Select(g => (Anio: g.Key, TotalPeatones: g.Sum(x => x.Cantidad)))
            .OrderBy(x => x.Anio);
    }
}
