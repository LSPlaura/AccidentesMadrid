namespace AccidentesMadrid.Config;

public static class Config
{
    public static readonly string BaseDirectory = AppContext.BaseDirectory;
    public static readonly string CsvsFolder = Path.Combine(BaseDirectory, "Data");
    public static readonly string AccidentesFolder = Path.Combine(CsvsFolder,"Accidentes");
    public static readonly string Csv2024Path = Path.Combine(AccidentesFolder, "2024-accidentes-trafico-detalle.csv");
    public static readonly string Csv2025Path = Path.Combine(AccidentesFolder, "2025-accidentes-trafico-detalle-csv.csv");
    public static readonly string Csv2026Path = Path.Combine(AccidentesFolder, "2026-accidentes-trafico-detalle-csv.csv");
    public static readonly string NumExpediente = "num_expediente";
    public static readonly string Fecha = "fecha";
    public static readonly string Hora = "hora";
    public static readonly string Localizacion = "localizacion";
    public static readonly string Numero = "numero";
    public static readonly string CodDistrito = "cod_distrito";
    public static readonly string Distrito = "distrito";
    public static readonly string TipoAccidente = "tipo_accidente";
    public static readonly string EstadoMeteorologico = "estado_meteorológico";
    public static readonly string TipoVehiculo = "tipo_vehiculo";
    public static readonly string TipoPersona = "tipo_persona";
    public static readonly string RangoEdad = "rango_edad";
    public static readonly string Sexo = "sexo";
    public static readonly string CodLesividad = "cod_lesividad";
    public static readonly string Lesividad = "lesividad";
    public static readonly string CoordenadaXUtm = "coordenada_x_utm";
    public static readonly string CoordenadaYUtm = "coordenada_y_utm";
    public static readonly string PositivaAlcohol = "positiva_alcohol";
    public static readonly string PositivaDroga = "positiva_droga";
}