using System.Diagnostics;
using AccidentesMadrid.Config;
using AccidentesMadrid.Models;
using AccidentesMadrid.Repositories.Accidentes;
using AccidentesMadrid.Services.Analyzers.Accidentes;
using AccidentesMadrid.Services.File.Csv.Accidentes;
using AccidentesMadrid.Storages.DataFrames;
using AccidentesMadrid.Storages.Files.Csv.Reader.Accidentes;
using AccidentesMadrid.Storages.Files.Csv.Writer.Accidentes;
using Microsoft.Data.Analysis;

namespace AccidentesMadrid;

class Program
{
    static async Task Main(string[] args)
    {
        // ==========================================
        // 1. CARGA DE DATOS PARA LINQ
        // ==========================================
        var reader = new AccidentesReader();
        var writer = new AccidentesWriter();
        var repository = new AccidentesRepository();
        var fileService = new AccidentesFileService(reader, writer, repository);

        string[] csvPaths = [
            Config.Config.Csv2024Path, 
            Config.Config.Csv2025Path, 
            Config.Config.Csv2026Path
        ];

        await fileService.Import(csvPaths);
        var accidentesLinq = fileService.GetAll();
        var linqAnalyzer = new AccidentesLinqAnalyzer();

        Console.WriteLine("==================================================");
        Console.WriteLine("        EJECUCIÓN DE LAS 30 CONSULTAS (LINQ)      ");
        Console.WriteLine("==================================================");

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        // 1. TotalAccidentes
        Console.WriteLine($"\n1. Total de Accidentes: {linqAnalyzer.TotalAccidentes(accidentesLinq)}");

        // 2. AccidentesPorDistrito
        Console.WriteLine("\n2. Accidentes por Distrito (Top 5):");
        foreach (var (distrito, total) in linqAnalyzer.AccidentesPorDistrito(accidentesLinq))
            Console.WriteLine($"   - {distrito}: {total}");

        // 3. AccidentesPorTipo
        Console.WriteLine("\n3. Accidentes por Tipo:");
        foreach (var (tipo, total) in linqAnalyzer.AccidentesPorTipo(accidentesLinq))
            Console.WriteLine($"   - {tipo}: {total}");

        // 4. AccidentesPorEstadoMetereologico
        Console.WriteLine("\n4. Accidentes por Estado Meteorológico:");
        foreach (var (estado, total) in linqAnalyzer.AccidentesPorEstadoMetereologico(accidentesLinq))
            Console.WriteLine($"   - {estado}: {total}");

        // 5. AccidentesPorSexo
        Console.WriteLine("\n5. Accidentes por Sexo:");
        foreach (var (sexo, total) in linqAnalyzer.AccidentesPorSexo(accidentesLinq))
            Console.WriteLine($"   - {sexo}: {total}");

        // 6. AccidentesPorRangoEdad
        Console.WriteLine("\n6. Accidentes por Rango de Edad:");
        foreach (var (rango, total) in linqAnalyzer.AccidentesPorRangoEdad(accidentesLinq))
            Console.WriteLine($"   - {rango}: {total}");

        // 7. PositivosAlcohol
        Console.WriteLine($"\n7. Positivos en Alcohol: {linqAnalyzer.PositivosAlcohol(accidentesLinq)}");

        // 8. PositivosDrogas
        Console.WriteLine($"\n8. Positivos en Drogas: {linqAnalyzer.PositivosDrogas(accidentesLinq)}");

        // 9. AccidentesPorDiaDeLaSemana
        Console.WriteLine("\n9. Accidentes por Día de la Semana:");
        foreach (var (dia, total) in linqAnalyzer.AccidentesPorDiaDeLaSemana(accidentesLinq))
            Console.WriteLine($"   - {dia}: {total}");

        // 10. AccidentesPorMes
        Console.WriteLine("\n10. Accidentes por Mes:");
        foreach (var (mes, total) in linqAnalyzer.AccidentesPorMes(accidentesLinq))
            Console.WriteLine($"   - Mes {mes}: {total}");

        // 11. HoraConMasAccidentes
        var horaPico = linqAnalyzer.HoraConMasAccidentes(accidentesLinq);
        Console.WriteLine($"\n11. Hora con más accidentes: {horaPico.Hora}:00 hrs ({horaPico.Total} accidentes)");

    // 12. LesionesMasFrecuentes
        var lesionFrecuente = linqAnalyzer.LesionesMasFrecuentes(accidentesLinq);
        Console.WriteLine($"\n12. Tipo de accidente/lesión más frecuente: {lesionFrecuente.TipoAccidente} ({lesionFrecuente.Total})");

    // 13. VehiculoMasImplicado
        var vehiculoPico = linqAnalyzer.VehiculoMasImplicado(accidentesLinq);
        Console.WriteLine($"\n13. Vehículo más implicado: {vehiculoPico.TipoVehiculo} ({vehiculoPico.Total})");
        // 14. AccidentesPeatones
        var expedientesPeatones = linqAnalyzer.AccidentesPeatones(accidentesLinq);
        Console.WriteLine($"\n14. Total expedientes con peatones implicados: {expedientesPeatones.Count()} (Mostrando primeros 3: {string.Join(", ", expedientesPeatones.Take(3))})");

        // 15. ProporcionHombreMujer
        Console.WriteLine("\n15. Proporción Hombre/Mujer (Conductores):");
        foreach (var (sexo, total) in linqAnalyzer.ProporcionHombreMujer(accidentesLinq))
            Console.WriteLine($"   - {sexo}: {total}");

        // 16. DistritosConMasPeatones
        Console.WriteLine("\n16. Distritos con más peatones implicados:");
        foreach (var (distrito, total) in linqAnalyzer.DistritosConMasPeatones(accidentesLinq))
            Console.WriteLine($"   - {distrito}: {total}");

        // 17. ComparacionDiasLaboralesVsFinde
        Console.WriteLine("\n17. Comparación Entre Semana vs Fin de Semana:");
        foreach (var (tipoDia, total) in linqAnalyzer.ComparacionDiasLaboralesVsFinde(accidentesLinq))
            Console.WriteLine($"   - {tipoDia}: {total}");

        // 18. MediaAccidentesPorDia
        Console.WriteLine($"\n18. Media de accidentes por día: {linqAnalyzer.MediaAccidentesPorDia(accidentesLinq):F2}");

        // 19. AccidentesDrogaYAlcohol
        Console.WriteLine($"\n19. Total accidentes con positivo en Droga y Alcohol simultáneo: {linqAnalyzer.AccidentesDrogaYAlcohol(accidentesLinq)}");

        // 20. RangosEdadPeatonesVulnerables
        Console.WriteLine("\n20. Rangos de edad de peatones más vulnerables:");
        foreach (var (rango, total) in linqAnalyzer.RangosEdadPeatonesVulnerables(accidentesLinq))
            Console.WriteLine($"   - {rango}: {total}");

        // 21. DistritosConMasPositivosAlcohol
        Console.WriteLine("\n21. Distritos con más positivos en alcohol:");
        foreach (var (distrito, total) in linqAnalyzer.DistritosConMasPositivosAlcohol(accidentesLinq))
            Console.WriteLine($"   - {distrito}: {total}");

        // 22. AccidentesPorCodigoDistrito
        Console.WriteLine("\n22. Accidentes por Código de Distrito:");
        foreach (var (cod, total) in linqAnalyzer.AccidentesPorCodigoDistrito(accidentesLinq))
            Console.WriteLine($"   - Cód {cod}: {total}");

        // 23. AccidentesPorAnio
        Console.WriteLine("\n23. Accidentes por Año:");
        foreach (var (anio, total) in linqAnalyzer.AccidentesPorAnio(accidentesLinq))
            Console.WriteLine($"   - {anio}: {total}");

        // 24. EvolucionMensualPorAnio
        Console.WriteLine("\n24. Evolución Mensual por Año:");
        foreach (var (anio, mes, total) in linqAnalyzer.EvolucionMensualPorAnio(accidentesLinq))
            Console.WriteLine($"   - {anio} / Mes {mes}: {total}");

        // 25. DistritoConMasAccidentesPorAnio
        Console.WriteLine("\n25. Distrito con más accidentes por año:");
        foreach (var (anio, distrito, total) in linqAnalyzer.DistritoConMasAccidentesPorAnio(accidentesLinq))
            Console.WriteLine($"   - {anio}: {distrito} ({total})");

        // 26. TendenciaAlcoholPorAnio
        Console.WriteLine("\n26. Tendencia de positivos en alcohol por año:");
        foreach (var (anio, total) in linqAnalyzer.TendenciaAlcoholPorAnio(accidentesLinq))
            Console.WriteLine($"   - {anio}: {total} positivos");

        // 27. ComparativaFindeVsLaboralPorAnio
        Console.WriteLine("\n27. Comparativa Fin de Semana vs Laboral por Año:");
        foreach (var (anio, tipoDia, total) in linqAnalyzer.ComparativaFindeVsLaboralPorAnio(accidentesLinq))
            Console.WriteLine($"   - {anio} / {tipoDia}: {total}");

        // 28. HoraPicoPorAnio
        Console.WriteLine("\n28. Hora pico con más accidentes por año:");
        foreach (var (anio, horaPicoAnio, total) in linqAnalyzer.HoraPicoPorAnio(accidentesLinq))
            Console.WriteLine($"   - {anio}: {horaPicoAnio}:00 hrs ({total} accidentes)");

        // 29. LesionMasFrecuentePorAnio
        Console.WriteLine("\n29. Lesión más frecuente por año:");
        foreach (var (anio, lesion, total) in linqAnalyzer.LesionMasFrecuentePorAnio(accidentesLinq))
            Console.WriteLine($"   - {anio}: {lesion} ({total})");

        // 30. EvolucionPeatonesPorAnio
        Console.WriteLine("\n30. Evolución de peatones implicados por año:");
        foreach (var (anio, total) in linqAnalyzer.EvolucionPeatonesPorAnio(accidentesLinq))
            Console.WriteLine($"   - {anio}: {total} peatones");
        
        stopwatch.Stop();
        Console.WriteLine($"Tiempo transcurrido: {stopwatch.ElapsedMilliseconds} ms");
        // ==========================================
        // 2. CARGA Y EJECUCIÓN CON DATAFRAME
        // ==========================================
        Console.WriteLine("\n\n==================================================");
        Console.WriteLine("    EJECUCIÓN DE LAS 30 CONSULTAS (DATAFRAME)    ");
        Console.WriteLine("==================================================");

        stopwatch = new Stopwatch();
        stopwatch.Start();
        
        var dfReader = new DataFrameReader();
        var dfAnalyzer = new AccidentesDataFrameAnalyzer();
        var writerAccidentes = new AccidentesWriter();
        // if (!File.Exists(Config.Config.CsvTodosPath))
        // {
        //  //   if (!await writerAccidentes.Load(accidentesLinq, Config.Config.CsvTodosPath)) return;
        // }
        
         DataFrame dataframe = dfReader.LoadDataFrame(Config.Config.CsvTodosPath);
              // 1. TotalAccidentes
            Console.WriteLine($"\n1. Total de Accidentes: {dfAnalyzer.TotalAccidentes(dataframe)}");

            // 2. AccidentesPorDistrito
            Console.WriteLine("\n2. Accidentes por Distrito (Top 5):");
            foreach (var (distrito, total) in dfAnalyzer.AccidentesPorDistrito(dataframe))
                Console.WriteLine($"   - {distrito}: {total}");

            // 3. AccidentesPorTipo
            Console.WriteLine("\n3. Accidentes por Tipo:");
            foreach (var (tipo, total) in dfAnalyzer.AccidentesPorTipo(dataframe))
                Console.WriteLine($"   - {tipo}: {total}");

            // 4. AccidentesPorEstadoMetereologico
            Console.WriteLine("\n4. Accidentes por Estado Meteorológico:");
            foreach (var (estado, total) in dfAnalyzer.AccidentesPorEstadoMetereologico(dataframe))
                Console.WriteLine($"   - {estado}: {total}");

            // 5. AccidentesPorSexo
            Console.WriteLine("\n5. Accidentes por Sexo:");
            foreach (var (sexo, total) in dfAnalyzer.AccidentesPorSexo(dataframe))
                Console.WriteLine($"   - {sexo}: {total}");

            // 6. AccidentesPorRangoEdad
            Console.WriteLine("\n6. Accidentes por Rango de Edad:");
            foreach (var (rango, total) in dfAnalyzer.AccidentesPorRangoEdad(dataframe))
                Console.WriteLine($"   - {rango}: {total}");

            // 7. PositivosAlcohol
            Console.WriteLine($"\n7. Positivos en Alcohol: {dfAnalyzer.PositivosAlcohol(dataframe)}");

            // 8. PositivosDrogas
            Console.WriteLine($"\n8. Positivos en Drogas: {dfAnalyzer.PositivosDrogas(dataframe)}");

            // 9. AccidentesPorDiaDeLaSemana
            Console.WriteLine("\n9. Accidentes por Día de la Semana:");
            foreach (var (dia, total) in dfAnalyzer.AccidentesPorDiaDeLaSemana(dataframe))
                Console.WriteLine($"   - {dia}: {total}");

            // 10. AccidentesPorMes
            Console.WriteLine("\n10. Accidentes por Mes:");
            foreach (var (mes, total) in dfAnalyzer.AccidentesPorMes(dataframe))
                Console.WriteLine($"   - Mes {mes}: {total}");

            // 11. HoraConMasAccidentes
            var horaPicoDf = dfAnalyzer.HoraConMasAccidentes(dataframe);
            Console.WriteLine($"\n11. Hora con más accidentes: {(horaPicoDf.HasValue ? $"{horaPicoDf.Value.Hora}:00 hrs ({horaPicoDf.Value.Total} accidentes)" : "Sin datos")}");

            // 12. LesionesMasFrecuentes
            var lesionDf = dfAnalyzer.LesionesMasFrecuentes(dataframe);
            Console.WriteLine($"\n12. Tipo de accidente/lesión más frecuente: {(lesionDf.HasValue ? $"{lesionDf.Value.TipoAccidente} ({lesionDf.Value.Total})" : "Sin datos")}");

            // 13. VehiculoMasImplicado
            var vehiculoDf = dfAnalyzer.VehiculoMasImplicado(dataframe);
            Console.WriteLine($"\n13. Vehículo más implicado: {(vehiculoDf.HasValue ? $"{vehiculoDf.Value.TipoVehiculo} ({vehiculoDf.Value.Total})" : "Sin datos")}");

            // 14. AccidentesPeatones
            var expPeatonesDf = dfAnalyzer.AccidentesPeatones(dataframe);
            Console.WriteLine($"\n14. Total expedientes con peatones implicados: {expPeatonesDf.Count()} (Mostrando primeros 3: {string.Join(", ", expPeatonesDf.Take(3))})");

            // 15. ProporcionHombreMujer
            Console.WriteLine("\n15. Proporción Hombre/Mujer (Conductores):");
            foreach (var (sexo, total) in dfAnalyzer.ProporcionHombreMujer(dataframe))
                Console.WriteLine($"   - {sexo}: {total}");

            // 16. DistritosConMasPeatones
            Console.WriteLine("\n16. Distritos con más peatones implicados:");
            foreach (var (distrito, total) in dfAnalyzer.DistritosConMasPeatones(dataframe))
                Console.WriteLine($"   - {distrito}: {total}");

            // 17. ComparacionDiasLaboralesVsFinde
            Console.WriteLine("\n17. Comparación Entre Semana vs Fin de Semana:");
            foreach (var (tipoDia, total) in dfAnalyzer.ComparacionDiasLaboralesVsFinde(dataframe))
                Console.WriteLine($"   - {tipoDia}: {total}");

            // 18. MediaAccidentesPorDia
            Console.WriteLine($"\n18. Media de accidentes por día: {dfAnalyzer.MediaAccidentesPorDia(dataframe):F2}");

            // 19. AccidentesDrogaYAlcohol
            Console.WriteLine($"\n19. Total accidentes con positivo en Droga y Alcohol simultáneo: {dfAnalyzer.AccidentesDrogaYAlcohol(dataframe)}");

            // 20. RangosEdadPeatonesVulnerables
            Console.WriteLine("\n20. Rangos de edad de peatones más vulnerables:");
            foreach (var (rango, total) in dfAnalyzer.RangosEdadPeatonesVulnerables(dataframe))
                Console.WriteLine($"   - {rango}: {total}");

            // 21. DistritosConMasPositivosAlcohol
            Console.WriteLine("\n21. Distritos con más positivos en alcohol:");
            foreach (var (distrito, total) in dfAnalyzer.DistritosConMasPositivosAlcohol(dataframe))
                Console.WriteLine($"   - {distrito}: {total}");

            // 22. AccidentesPorCodigoDistrito
            Console.WriteLine("\n22. Accidentes por Código de Distrito:");
            foreach (var (cod, total) in dfAnalyzer.AccidentesPorCodigoDistrito(dataframe))
                Console.WriteLine($"   - Cód {cod}: {total}");

            // 23. AccidentesPorAnio
            Console.WriteLine("\n23. Accidentes por Año:");
            foreach (var (anio, total) in dfAnalyzer.AccidentesPorAnio(dataframe))
                Console.WriteLine($"   - {anio}: {total}");

            // 24. EvolucionMensualPorAnio
            Console.WriteLine("\n24. Evolución Mensual por Año:");
            foreach (var (anio, mes, total) in dfAnalyzer.EvolucionMensualPorAnio(dataframe))
                Console.WriteLine($"   - {anio} / Mes {mes}: {total}");

            // 25. DistritoConMasAccidentesPorAnio
            Console.WriteLine("\n25. Distrito con más accidentes por año:");
            foreach (var (anio, distrito, total) in dfAnalyzer.DistritoConMasAccidentesPorAnio(dataframe))
                Console.WriteLine($"   - {anio}: {distrito} ({total})");

            // 26. TendenciaAlcoholPorAnio
            Console.WriteLine("\n26. Tendencia de positivos en alcohol por año:");
            foreach (var (anio, total) in dfAnalyzer.TendenciaAlcoholPorAnio(dataframe))
                Console.WriteLine($"   - {anio}: {total} positivos");

            // 27. ComparativaFindeVsLaboralPorAnio
            Console.WriteLine("\n27. Comparativa Fin de Semana vs Laboral por Año:");
            foreach (var (anio, tipoDia, total) in dfAnalyzer.ComparativaFindeVsLaboralPorAnio(dataframe))
                Console.WriteLine($"   - {anio} / {tipoDia}: {total}");

            // 28. HoraPicoPorAnio
            Console.WriteLine("\n28. Hora pico con más accidentes por año:");
            foreach (var (anio, horaPicoAnio, total) in dfAnalyzer.HoraPicoPorAnio(dataframe))
                Console.WriteLine($"   - {anio}: {horaPicoAnio}:00 hrs ({total} accidentes)");

            // 29. LesionMasFrecuentePorAnio
            Console.WriteLine("\n29. Lesión más frecuente por año:");
            foreach (var (anio, lesion, total) in dfAnalyzer.LesionMasFrecuentePorAnio(dataframe))
                Console.WriteLine($"   - {anio}: {lesion} ({total})");

            // 30. EvolucionPeatonesPorAnio
            Console.WriteLine("\n30. Evolución de peatones implicados por año:");
            foreach (var (anio, total) in dfAnalyzer.EvolucionPeatonesPorAnio(dataframe))
                Console.WriteLine($"   - {anio}: {total} peatones");
            stopwatch.Stop();
            Console.WriteLine($"Tiempo transcurrido: {stopwatch.ElapsedMilliseconds} ms");
        }
    
}