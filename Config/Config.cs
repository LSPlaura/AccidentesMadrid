namespace AccidentesMadrid.Config;

public static class Config
{
    public static readonly string BaseDirectory = AppContext.BaseDirectory;
    public static readonly string CsvsFolder = Path.Combine(BaseDirectory, "Data");
    public static readonly string AccidentesFolder = Path.Combine(CsvsFolder,"Accidentes");
    public static readonly string Csv2024Path = Path.Combine(AccidentesFolder, "2024-accidentes-trafico-detalle.csv");
    public static readonly string Csv2025Path = Path.Combine(AccidentesFolder, "2025-accidentes-trafico-detalle-csv.csv");
    public static readonly string Csv2026Path = Path.Combine(AccidentesFolder, "2026-accidentes-trafico-detalle-csv.csv");
}