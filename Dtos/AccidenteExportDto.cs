namespace AccidentesMadrid.Dtos;

public class AccidenteExportDto
{
    public record AccidenteDto(
        string NumExpediente,         
        DateTime Fecha,             
        TimeSpan Hora,               
        string Localizacion,    
        int Numero,
        int CodDistrito, 
        string Distrito,
        string TipoAccidente,
        string EstadoMeteorologico, 
        string TipoVehiculo, 
        string TipoPersona,
        string RangoEdad, 
        string Sexo,
        string CodLesividad,
        string Lesividad,
        string? CoordenadaXUtm,       
        string? CoordenadaYUtm, 
        bool PositivaAlcohol, 
        bool PositivaDroga   
    );
}