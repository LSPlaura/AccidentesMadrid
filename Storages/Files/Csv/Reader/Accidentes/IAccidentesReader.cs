using AccidentesMadrid.Models;
using AccidentesMadrid.Storages.Files.Common;

namespace AccidentesMadrid.Storages.Files.Csv.Reader.Accidentes;

//patrón storage de csv simplificado para que solo importe
public interface IAccidentesReader : IReader<Accidente>
{
    
}