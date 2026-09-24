# Accidentes de tráfico en Madrid

Aplicación de consola desarrollada en C# para analizar los accidentes de tráfico registrados en Madrid durante los años 2024, 2025 y 2026.

El proyecto implementa las mismas consultas utilizando dos enfoques diferentes:

- **LINQ sobre objetos `Accidente`**.
- **Microsoft.Data.Analysis mediante `DataFrame`**.

El objetivo principal es comparar ambos enfoques en cuanto a diseño, resultados y tiempo de ejecución.

> Los resultados incluidos en este documento corresponden a una ejecución concreta del programa con 130.864 registros.

---

## Índice

1. [Objetivos](#objetivos)
2. [Tecnologías utilizadas](#tecnologías-utilizadas)
3. [Estructura del proyecto](#estructura-del-proyecto)
4. [Instrucciones de uso](#instrucciones-de-uso)
5. [Funcionamiento general](#funcionamiento-general)
6. [Justificación del diseño](#justificación-del-diseño)
7. [Consultas implementadas](#consultas-implementadas)
8. [Tiempos de ejecución](#tiempos-de-ejecución)
9. [Análisis de resultados](#análisis-de-resultados)
10. [Diferencias entre LINQ y DataFrame](#diferencias-entre-linq-y-dataframe)
11. [Limitaciones detectadas](#limitaciones-detectadas)
12. [Mejoras futuras](#mejoras-futuras)
13. [Conclusiones](#conclusiones)

---

## Objetivos

Los objetivos principales de la aplicación son:

- Importar información de accidentes desde varios ficheros CSV.
- Transformar los datos de texto en un modelo de dominio tipado.
- Almacenar los registros en memoria.
- Ejecutar 30 consultas analíticas.
- Comparar los resultados obtenidos mediante LINQ y `DataFrame`.
- Medir el tiempo de ejecución de ambos enfoques.
- Analizar las diferencias de rendimiento y detectar posibles errores de implementación.

Las consultas estudian aspectos como:

- Accidentes por distrito.
- Tipos de accidente.
- Meteorología.
- Sexo y edad de las personas implicadas.
- Alcohol y drogas.
- Días de la semana y meses.
- Horas con más accidentes.
- Accidentes con peatones.
- Evolución anual y mensual.
- Diferencias entre días laborables y fines de semana.

---

## Tecnologías utilizadas

- **Lenguaje:** C#
- **Framework:** .NET 10
- **Tipo de aplicación:** Aplicación de consola
- **Librería de análisis:** `Microsoft.Data.Analysis`
- **Versión de la librería:** 0.23.0
- **Formato de datos:** CSV
- **Codificación:** UTF-8
- **Separador de columnas:** `;`

La dependencia principal está declarada en el fichero `AccidentesMadrid.csproj`:

```xml
<PackageReference Include="Microsoft.Data.Analysis" Version="0.23.0" />
```

---

## Estructura del proyecto

```text
Config/
  Config.cs

Dtos/
  AccidenteDto.cs

Mappers/
  AccidenteMapper.cs

Models/
  Accidente.cs
  Enums/
    Gravedad.cs
    Sexo.cs
    TipoAccidente.cs
    TipoPersona.cs

Repositories/
  Accidentes/
    AccidentesRepository.cs
    IAccidentesRepository.cs
  Common/
    IMemoryRepository.cs

Services/
  Analyzers/
    Accidentes/
      AccidentesDataFrameAnalyzer.cs
      AccidentesLinqAnalyzer.cs
      Common/
        IAccidentesAnalyzer.cs
  File/
    Common/
      IFileService.cs
    Csv/
      Accidentes/
        AccidentesFileService.cs
        IAccidentesFileService.cs

Storages/
  DataFrames/
    DataFrameReader.cs
    Common/
      IDataFrameReader.cs
  Files/
    Common/
      IReader.cs
      IWriter.cs
    Csv/
      Reader/
        Accidentes/
          AccidentesReader.cs
          IAccidentesReader.cs
      Writer/
        Accidentes/
          AccidentesWriter.cs
          IAccidentesWriter.cs

Program.cs
AccidentesMadrid.csproj
```

### `Models`

Contiene el modelo principal `Accidente` y los enumerados usados para representar valores controlados:

- `Gravedad`
- `Sexo`
- `TipoAccidente`
- `TipoPersona`

El modelo utiliza tipos adecuados para cada campo:

- `DateTime` para la fecha.
- `TimeSpan` para la hora.
- `int` para números y códigos.
- `bool` para positivos de alcohol y drogas.
- Enumerados para sexo, persona, gravedad y tipo de accidente.

Esto permite trabajar con datos tipados en LINQ y evita realizar conversiones continuamente durante las consultas.

### `Dtos`

`AccidenteDto` representa los datos tal y como aparecen en el fichero CSV. Todos sus campos son cadenas de texto porque el CSV contiene inicialmente información sin transformar.

El DTO permite separar:

- El formato externo del fichero.
- El modelo interno utilizado por la aplicación.

### `Mappers`

`AccidenteMapper` transforma un `AccidenteDto` en un objeto `Accidente`.

Durante esta transformación se realizan varias operaciones:

- Conversión de fechas.
- Conversión de horas.
- Conversión de números.
- Conversión de valores `S`, `N`, `SI` o `NO` a booleanos.
- Conversión de textos a enumerados.
- Normalización de valores desconocidos.
- Validación de rangos de edad.
- Conversión de códigos de lesividad a gravedad.

Esta decisión centraliza la lógica de transformación en un único lugar.

### `Repositories`

`AccidentesRepository` mantiene los accidentes en una lista en memoria.

El repositorio expone únicamente las operaciones necesarias:

- Obtener todos los accidentes.
- Añadir varios accidentes.

La utilización de una interfaz permite cambiar la implementación en el futuro por otra basada en una base de datos, un fichero o una colección diferente.

### `Storages`

Esta capa contiene la lectura y escritura de datos.

Se separan dos tipos de almacenamiento:

- Lectura y escritura de ficheros CSV.
- Lectura de datos en formato `DataFrame`.

El lector de accidentes utiliza `IAsyncEnumerable<Accidente>` para procesar las líneas del fichero de forma asíncrona.

### `Services`

Contiene la lógica de negocio:

- `AccidentesFileService` coordina la importación y exportación.
- `AccidentesLinqAnalyzer` ejecuta las consultas mediante LINQ.
- `AccidentesDataFrameAnalyzer` ejecuta consultas utilizando `DataFrame`.

Ambos analizadores intentan implementar el mismo contrato definido en `IAccidentesAnalyzer<TData>`.

---

## Instrucciones de uso

### Requisitos previos

Es necesario disponer de:

- .NET 10 SDK.
- Los ficheros CSV de accidentes.
- Acceso a una terminal o entorno compatible con aplicaciones .NET.

Se puede comprobar la versión instalada mediante:

```bash
dotnet --version
```

### Ubicación de los ficheros CSV

Los ficheros deben situarse en:

```text
Data/Accidentes/
```

Con los siguientes nombres:

```text
2024-accidentes-trafico-detalle.csv
2025-accidentes-trafico-detalle-csv.csv
2026-accidentes-trafico-detalle-csv.csv
```

El programa también utiliza el siguiente fichero combinado:

```text
Todos-accidentes-trafico-detalle-csv.csv
```

Este fichero se genera automáticamente si todavía no existe.

La ruta se construye en `Config/Config.cs` utilizando `AppContext.BaseDirectory`, por lo que los ficheros deben estar disponibles también en el directorio de ejecución de la aplicación.

### Importante sobre `.gitignore`

La carpeta `Data/` está excluida mediante `.gitignore`:

```gitignore
/Data/
```

Por tanto, los CSV no forman parte necesariamente del repositorio y deben proporcionarse localmente antes de ejecutar la aplicación.

### Restaurar dependencias

Desde la raíz del proyecto:

```bash
dotnet restore
```

### Compilar

```bash
dotnet build
```

### Ejecutar

```bash
dotnet run
```

También se puede ejecutar en modo Release:

```bash
dotnet run --configuration Release
```

### Flujo de ejecución

`Program.cs` realiza las siguientes operaciones:

1. Crea el lector, escritor, repositorio y servicio de ficheros.
2. Importa los CSV de 2024, 2025 y 2026.
3. Almacena los objetos `Accidente` en memoria.
4. Ejecuta las 30 consultas mediante LINQ.
5. Mide el tiempo de ejecución de las consultas LINQ.
6. Genera el fichero combinado si no existe.
7. Carga el fichero combinado en un `DataFrame`.
8. Ejecuta las mismas 30 consultas mediante `DataFrame`.
9. Mide el tiempo de la fase DataFrame.
10. Muestra los resultados por consola.

---

## Justificación del diseño

### Separación por capas

El proyecto separa la lectura, transformación, almacenamiento y análisis de datos.

Esta decisión facilita:

- Mantener cada clase con una responsabilidad concreta.
- Sustituir una implementación sin modificar el resto.
- Probar las distintas partes por separado.
- Comparar LINQ y `DataFrame` sin cambiar la forma de importar los datos.

### Uso de DTO

El CSV proporciona todos los datos como texto. Por eso se utiliza `AccidenteDto` como representación intermedia.

El DTO evita que la lectura del fichero tenga que conocer directamente todos los detalles del modelo de dominio.

El flujo es:

```text
CSV -> AccidenteDto -> Accidente
```

### Uso de `record`

El modelo `Accidente` se define como un `record` porque representa datos y no una entidad con comportamiento complejo.

Los `record` proporcionan:

- Comparación por valor.
- Sintaxis compacta.
- Inmutabilidad de sus propiedades.
- Adecuación para transportar información.

### Uso de enumerados

Los enumerados permiten evitar trabajar directamente con cadenas para valores controlados.

Por ejemplo:

```csharp
Sexo.Hombre
TipoPersona.Peatón
Gravedad.Leve
```

Esto mejora la legibilidad y reduce errores tipográficos.

### Normalización de datos

Durante el mapeo se normalizan datos potencialmente inconsistentes:

- Textos con espacios.
- Diferentes representaciones de booleanos.
- Ausencia de valores.
- Categorías desconocidas.
- Diferencias entre `peatón` y `peaton`.
- Códigos de gravedad con ceros iniciales.

Cuando un valor no se puede interpretar, se utiliza un valor por defecto como:

- `Desconocido`.
- `Se desconoce`.
- `DateTime.MinValue`.
- `TimeSpan.Zero`.
- `false`.

Esta decisión evita que un registro incorrecto detenga toda la importación.

### Procesamiento por lotes

`AccidentesFileService` utiliza lotes de 1.000 elementos:

```csharp
Task Import(IEnumerable<string> paths, int batchSize = 1000)
```

Los lotes reducen el número de operaciones de inserción en el repositorio y permiten controlar mejor el uso de memoria.

Aunque finalmente todos los datos se almacenan en memoria, el procesamiento por lotes facilita modificar el repositorio en el futuro para trabajar con una base de datos.

### Uso de `IAsyncEnumerable`

El lector usa:

```csharp
File.ReadLinesAsync(path)
```

Esto permite leer el fichero progresivamente sin cargar inicialmente todo su contenido como una única cadena.

Es una decisión adecuada para ficheros grandes, aunque la posterior acumulación en el repositorio hace que todos los objetos terminen estando en memoria.

### Dos estrategias de análisis

El proyecto implementa dos analizadores:

- `AccidentesLinqAnalyzer`
- `AccidentesDataFrameAnalyzer`

La finalidad es comparar dos formas de procesar datos:

- Objetos tipados en memoria mediante LINQ.
- Estructuras tabulares mediante `DataFrame`.

Para que la comparación sea válida, ambos analizadores deberían aplicar exactamente:

- La misma deduplicación.
- La misma conversión de fechas.
- La misma interpretación de horas.
- Los mismos filtros.
- La misma definición de accidente, persona y lesión.

En la ejecución actual esto no se cumple completamente, por lo que los tiempos y resultados deben interpretarse con cautela.

---

# Accidentes de tráfico en Madrid

Aplicación de consola desarrollada en C# y .NET 10 para importar y analizar datos de accidentes de tráfico registrados en Madrid durante los años 2024, 2025 y 2026.

El proyecto ejecuta un conjunto de 30 consultas analíticas utilizando dos enfoques diferentes:

- **LINQ**, trabajando con objetos de dominio `Accidente`.
- **DataFrame**, utilizando la librería `Microsoft.Data.Analysis`.

El objetivo es comparar la estructura, los resultados y el rendimiento de ambas alternativas.

> Los resultados documentados corresponden a una ejecución con 130.864 registros.

---

## Índice

1. [Objetivos](#objetivos)
2. [Tecnologías utilizadas](#tecnologías-utilizadas)
3. [Estructura del proyecto](#estructura-del-proyecto)
4. [Formato y ubicación de los datos](#formato-y-ubicación-de-los-datos)
5. [Instrucciones de uso](#instrucciones-de-uso)
6. [Ejecución con Docker](#ejecución-con-docker)
7. [Funcionamiento de la aplicación](#funcionamiento-de-la-aplicación)
8. [Justificación del diseño](#justificación-del-diseño)
9. [Consultas implementadas](#consultas-implementadas)
10. [Tiempos de ejecución](#tiempos-de-ejecución)
11. [Análisis de resultados](#análisis-de-resultados)
12. [Limitaciones detectadas](#limitaciones-detectadas)
13. [Mejoras futuras](#mejoras-futuras)
14. [Conclusiones](#conclusiones)

---

## Objetivos

Los objetivos principales del proyecto son:

- Importar datos desde varios ficheros CSV.
- Transformar los valores de texto a un modelo de dominio tipado.
- Almacenar los registros en memoria.
- Ejecutar 30 consultas analíticas.
- Comparar LINQ y `DataFrame`.
- Medir los tiempos de ejecución.
- Analizar las diferencias entre ambos enfoques.
- Ejecutar la aplicación tanto localmente como dentro de un contenedor Docker.

Las consultas estudian:

- Distribución de accidentes por distrito.
- Tipo de accidente.
- Estado meteorológico.
- Sexo y rango de edad.
- Positivos en alcohol y drogas.
- Días de la semana y meses.
- Horas con más accidentes.
- Accidentes con peatones.
- Evolución anual y mensual.
- Diferencias entre días laborables y fines de semana.
- Vehículos más implicados.
- Gravedad y lesividad.

---

## Tecnologías utilizadas

- **Lenguaje:** C#
- **Framework:** .NET 10
- **Tipo de aplicación:** Aplicación de consola
- **Librería de análisis:** `Microsoft.Data.Analysis`
- **Versión:** 0.23.0
- **Formato de datos:** CSV
- **Codificación:** UTF-8
- **Separador de columnas:** `;`
- **Contenedores:** Docker

La dependencia principal está declarada en `AccidentesMadrid.csproj`:

```xml
<PackageReference Include="Microsoft.Data.Analysis" Version="0.23.0" />
```

---

## Estructura del proyecto

```text
Config/
  Config.cs

Dtos/
  AccidenteDto.cs

Mappers/
  AccidenteMapper.cs

Models/
  Accidente.cs
  Enums/
    Gravedad.cs
    Sexo.cs
    TipoAccidente.cs
    TipoPersona.cs

Repositories/
  Accidentes/
    AccidentesRepository.cs
    IAccidentesRepository.cs
  Common/
    IMemoryRepository.cs

Services/
  Analyzers/
    Accidentes/
      AccidentesDataFrameAnalyzer.cs
      AccidentesLinqAnalyzer.cs
      Common/
        IAccidentesAnalyzer.cs
  File/
    Common/
      IFileService.cs
    Csv/
      Accidentes/
        AccidentesFileService.cs
        IAccidentesFileService.cs

Storages/
  DataFrames/
    DataFrameReader.cs
    Common/
      IDataFrameReader.cs
  Files/
    Common/
      IReader.cs
      IWriter.cs
    Csv/
      Reader/
        Accidentes/
          AccidentesReader.cs
          IAccidentesReader.cs
      Writer/
        Accidentes/
          AccidentesWriter.cs
          IAccidentesWriter.cs

AccidentesMadrid.csproj
AccidentesMadrid.slnx
Dockerfile
.dockerignore
.gitignore
Program.cs
README.md
```

### Modelos

La carpeta `Models` contiene el modelo principal `Accidente` y los enumerados utilizados en el proyecto:

- `Gravedad`
- `Sexo`
- `TipoAccidente`
- `TipoPersona`

El modelo `Accidente` utiliza tipos adecuados para cada campo:

- `DateTime` para la fecha.
- `TimeSpan` para la hora.
- `int` para números y códigos.
- `bool` para positivos de alcohol y drogas.
- Enumerados para categorías controladas.

### DTO

`AccidenteDto` representa una fila del CSV antes de realizar las conversiones. Sus propiedades son cadenas de texto porque los datos procedentes del fichero todavía no están tipados.

El flujo de transformación es:

```text
CSV -> AccidenteDto -> Accidente
```

### Mapper

`AccidenteMapper` transforma un `AccidenteDto` en un objeto `Accidente`.

Durante este proceso se realizan las siguientes operaciones:

- Conversión de fechas.
- Conversión de horas.
- Conversión de valores numéricos.
- Conversión de textos a enumerados.
- Conversión de valores como `S`, `N`, `SI` o `NO` a booleanos.
- Normalización de valores desconocidos.
- Validación de rangos de edad.
- Conversión de códigos de lesividad a gravedad.

### Repositorio

`AccidentesRepository` almacena los accidentes en una lista en memoria.

Sus operaciones principales son:

- Obtener todos los accidentes.
- Añadir varios accidentes.

La interfaz del repositorio permite sustituirlo en el futuro por otra implementación, como una base de datos o un almacenamiento persistente.

### Servicios de ficheros

`AccidentesFileService` coordina:

- La importación de los CSV.
- El almacenamiento en el repositorio.
- La exportación de los datos.
- La obtención de todos los accidentes cargados.

### Analizadores

El proyecto contiene dos analizadores:

- `AccidentesLinqAnalyzer`
- `AccidentesDataFrameAnalyzer`

Ambos intentan realizar las mismas consultas, pero utilizando estructuras de datos diferentes.

---

## Formato y ubicación de los datos

La aplicación espera encontrar los ficheros CSV en:

```text
Data/Accidentes/
```

Los nombres esperados son:

```text
2024-accidentes-trafico-detalle.csv
2025-accidentes-trafico-detalle-csv.csv
2026-accidentes-trafico-detalle-csv.csv
```

El fichero combinado utilizado para el análisis con `DataFrame` es:

```text
Todos-accidentes-trafico-detalle-csv.csv
```

Este fichero se genera automáticamente cuando no existe.

La aplicación obtiene las rutas mediante `Config/Config.cs`:

```csharp
public static readonly string AccidentesFolder =
    Path.Combine(CsvsFolder, "Accidentes");
```

La carpeta `Data` aparece excluida de Git mediante `.gitignore`:

```gitignore
/Data/
```

Por tanto, los datos deben estar disponibles localmente antes de ejecutar el programa.

---

## Instrucciones de uso

### Requisitos

Para ejecutar el proyecto directamente se necesita:

- .NET 10 SDK.
- Los ficheros CSV en la ubicación esperada.
- Una terminal compatible.

Para comprobar la versión de .NET instalada:

```bash
dotnet --version
```

### Restaurar dependencias

Desde la raíz del proyecto:

```bash
dotnet restore
```

### Compilar

```bash
dotnet build
```

Para compilar en modo Release:

```bash
dotnet build --configuration Release
```

### Ejecutar

```bash
dotnet run
```

También se puede ejecutar en modo Release:

```bash
dotnet run --configuration Release
```

### Flujo de ejecución

El programa realiza las siguientes operaciones:

1. Crea el lector CSV.
2. Crea el escritor CSV.
3. Crea el repositorio en memoria.
4. Importa los ficheros de 2024, 2025 y 2026.
5. Convierte cada fila a un objeto `Accidente`.
6. Ejecuta las 30 consultas con LINQ.
7. Mide el tiempo de ejecución de LINQ.
8. Genera el CSV combinado si no existe.
9. Carga el CSV combinado en un `DataFrame`.
10. Ejecuta las 30 consultas con `DataFrame`.
11. Mide el tiempo de ejecución de `DataFrame`.
12. Muestra los resultados en la consola.

---

## Ejecución con Docker

El proyecto incluye un `Dockerfile` multi-stage para compilar y ejecutar la aplicación dentro de un contenedor.

### Requisitos

Es necesario instalar y tener iniciado:

- Docker Desktop en Windows o macOS.
- Docker Engine en Linux.

El daemon de Docker debe estar funcionando antes de ejecutar cualquier comando de construcción o ejecución.

Se puede comprobar con:

```bash
docker info
```

Si este comando devuelve información del servidor Docker, el motor está disponible.

### Dockerfile

El `Dockerfile` utiliza dos imágenes diferentes:

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
```

para restaurar dependencias, compilar y publicar el proyecto, y:

```dockerfile
FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
```

para ejecutar la aplicación final.

Esta separación permite que la imagen final no incluya todas las herramientas del SDK y sea más ligera que una imagen utilizada también para compilar.

### Construir la imagen

Desde la raíz del proyecto, donde se encuentran `Dockerfile` y `AccidentesMadrid.csproj`, ejecutar:

```bash
docker build -t accidentes-madrid .
```

El nombre `accidentes-madrid` es una etiqueta local para la imagen y puede cambiarse si se desea.

Para forzar una compilación sin utilizar la caché:

```bash
docker build --no-cache -t accidentes-madrid .
```

### Ejecutar el contenedor

Una vez construida la imagen:

```bash
docker run --rm accidentes-madrid
```

La opción `--rm` elimina automáticamente el contenedor cuando finaliza la ejecución.

### Ejecutar montando la carpeta de datos

Como la carpeta `Data` puede no estar incluida en el repositorio, se recomienda montarla explícitamente en el contenedor.

#### PowerShell en Windows

```powershell
docker run --rm `
  -v "${PWD}/Data:/app/Data" `
  accidentes-madrid
```

#### Linux y macOS

```bash
docker run --rm \
  -v "$(pwd)/Data:/app/Data" \
  accidentes-madrid
```

Este montaje conecta la carpeta local:

```text
Data/
```

con la carpeta del contenedor:

```text
/app/Data/
```

La aplicación buscará los CSV en:

```text
/app/Data/Accidentes/
```

Por tanto, en el equipo local deben existir:

```text
Data/Accidentes/2024-accidentes-trafico-detalle.csv
Data/Accidentes/2025-accidentes-trafico-detalle-csv.csv
Data/Accidentes/2026-accidentes-trafico-detalle-csv.csv
```

### Ejecutar en segundo plano

Si se desea ejecutar el contenedor en segundo plano:

```bash
docker run -d --name accidentes-madrid-container accidentes-madrid
```

Para ver la salida de la aplicación:

```bash
docker logs -f accidentes-madrid-container
```

Para detener el contenedor:

```bash
docker stop accidentes-madrid-container
```

Para eliminarlo posteriormente:

```bash
docker rm accidentes-madrid-container
```

### Descargar las imágenes manualmente

Si la construcción falla al descargar las imágenes de .NET, se pueden descargar previamente:

```bash
docker pull mcr.microsoft.com/dotnet/runtime:10.0
docker pull mcr.microsoft.com/dotnet/sdk:10.0
```

Después se puede repetir la construcción:

```bash
docker build -t accidentes-madrid .
```

---

## Solución de problemas con Docker

### Error de conexión con el daemon

Si aparece un mensaje similar a:

```text
Cannot connect to the Docker daemon
```

significa que Docker Desktop o Docker Engine no está iniciado.

En Windows o macOS:

1. Abrir Docker Desktop.
2. Esperar a que termine de iniciarse.
3. Ejecutar:

```bash
docker info
```

En Linux:

```bash
sudo systemctl start docker
docker info
```

### Error `TLS handshake timeout`

Si aparece un error como:

```text
TLS handshake timeout
```

o:

```text
failed to resolve source metadata
```

el problema suele estar relacionado con la conexión entre Docker y Microsoft Container Registry, no con el código C# ni con el `Dockerfile`.

Se puede probar:

```bash
docker pull mcr.microsoft.com/dotnet/runtime:10.0
```

Las causas habituales son:

- Problemas de conexión a Internet.
- Proxy no configurado.
- VPN activa.
- Firewall.
- DNS incorrecto.
- Red corporativa o educativa que bloquea el registro.
- Problemas temporales de conectividad.

En Docker Desktop se debe revisar la configuración de proxy en:

```text
Settings → Resources → Proxies
```

También se puede probar temporalmente otra red, por ejemplo una conexión compartida desde un teléfono móvil.

### Error `Archivo no encontrado`

Si la aplicación muestra:

```text
Archivo no encontrado
```

hay que comprobar:

1. Que existe la carpeta local `Data/Accidentes`.
2. Que los CSV tienen los nombres correctos.
3. Que el volumen se ha montado correctamente.
4. Que se está ejecutando el comando desde la raíz del proyecto.

Se puede comprobar el contenido de la carpeta local con:

```bash
ls Data/Accidentes
```

En PowerShell:

```powershell
Get-ChildItem Data/Accidentes
```

### Error relacionado con el proyecto

El nombre correcto del proyecto es:

```text
AccidentesMadrid.csproj
```

El ensamblado final es:

```text
AccidentesMadrid.dll
```

El `Dockerfile` debe utilizar exactamente esos nombres:

```dockerfile
COPY ["AccidentesMadrid.csproj", "./"]
```

y:

```dockerfile
ENTRYPOINT ["dotnet", "AccidentesMadrid.dll"]
```

---

## Justificación del diseño

### Separación de responsabilidades

El proyecto separa:

- Lectura de ficheros.
- Transformación de datos.
- Almacenamiento.
- Lógica de análisis.
- Escritura de resultados.

Esto facilita el mantenimiento y permite cambiar una parte sin afectar directamente al resto.

### Uso de DTO

El DTO representa el formato externo de los datos y evita acoplar el lector CSV directamente al modelo de dominio.

### Uso de `record`

`Accidente` se define como `record` porque representa información y no una entidad con comportamiento complejo.

Además, los `record` proporcionan comparación por valor y una sintaxis compacta.

### Uso de enumerados

Los enumerados evitan depender de cadenas arbitrarias para valores controlados como:

- Sexo.
- Tipo de persona.
- Gravedad.
- Tipo de accidente.

### Normalización

El mapper normaliza los datos antes de almacenarlos:

- Elimina espacios innecesarios.
- Convierte números.
- Interpreta booleanos.
- Normaliza categorías.
- Gestiona datos desconocidos.

### Procesamiento por lotes

La importación utiliza lotes de 1.000 elementos:

```csharp
Task Import(IEnumerable<string> paths, int batchSize = 1000)
```

Esto reduce el número de operaciones de inserción en el repositorio y permite adaptar el código a un almacenamiento persistente en el futuro.

### Uso de `IAsyncEnumerable`

El lector utiliza `File.ReadLinesAsync`, lo que permite procesar progresivamente las líneas del fichero sin cargar inicialmente todo el archivo como una única cadena.

### Comparación entre LINQ y DataFrame

Se implementan dos analizadores para comparar:

- Una colección de objetos tipados.
- Una estructura tabular.

Sin embargo, para que la comparación sea completamente válida ambos analizadores deben utilizar:

- La misma definición de accidente.
- La misma deduplicación.
- La misma interpretación de fechas y horas.
- Los mismos filtros.
- La misma lógica de agrupación.

En la implementación actual existen algunas diferencias que deben tenerse en cuenta al interpretar los resultados.

---
## Consultas implementadas

La aplicación ejecuta 30 consultas agrupadas en varias categorías.

### Distribución general

1. Total de accidentes.
2. Accidentes por distrito.
3. Accidentes por tipo.
4. Accidentes por estado meteorológico.
5. Accidentes por sexo.
6. Accidentes por rango de edad.

### Factores de riesgo

7. Positivos en alcohol.
8. Positivos en drogas.
19. Positivos simultáneos en alcohol y drogas.
21. Distritos con más positivos en alcohol.
26. Tendencia anual de positivos en alcohol.

### Distribución temporal

9. Accidentes por día de la semana.
10. Accidentes por mes.
11. Hora con más accidentes.
17. Comparación entre días laborables y fines de semana.
18. Media de accidentes por día.
23. Accidentes por año.
24. Evolución mensual por año.
27. Comparativa anual entre días laborables y fines de semana.
28. Hora punta por año.

### Personas vulnerables

14. Expedientes con peatones implicados.
16. Distritos con más peatones.
20. Rangos de edad más frecuentes entre peatones.
30. Evolución anual de peatones.

### Información territorial y de vehículos

13. Vehículo más implicado.
22. Accidentes por código de distrito.
25. Distrito con más accidentes por año.

### Lesividad

12. Tipo de accidente o lesión más frecuente.
29. Lesión más frecuente por año.

---

## Tiempos de ejecución

La ejecución proporcionada obtuvo los siguientes resultados:

| Fase | Tiempo |
|---|---:|
| Consultas LINQ | 1.380 ms |
| Consultas DataFrame | 8.732 ms |
| Registros procesados | 130.864 |

La diferencia aproximada es:

```text
8.732 ms - 1.380 ms = 7.352 ms
```

En esta ejecución, la fase DataFrame tarda aproximadamente 6,3 veces más que la fase LINQ.

### Lectura de ficheros

El programa no muestra un tiempo independiente para la lectura de los CSV.

El cronómetro de LINQ comienza después de esta instrucción:

```csharp
await fileService.Import(csvPaths);
```

Por tanto, los 1.380 ms no incluyen la importación inicial de los ficheros.

En cambio, el cronómetro de DataFrame se inicia antes de:

- Crear el `DataFrameReader`.
- Generar el fichero combinado si no existe.
- Cargar el CSV combinado.
- Ejecutar las consultas DataFrame.

Por tanto, ambas mediciones no son completamente equivalentes.

Para obtener mediciones separadas sería necesario utilizar cronómetros independientes:

```text
1. Lectura e importación de CSV.
2. Generación del CSV combinado.
3. Carga del CSV en DataFrame.
4. Consultas LINQ.
5. Consultas DataFrame.
```

### Tiempo LINQ

El tiempo LINQ fue:

```text
1.380 ms
```

LINQ trabaja directamente sobre objetos `Accidente` que ya están transformados:

- Las fechas son `DateTime`.
- Las horas son `TimeSpan`.
- Los tipos son enumerados.
- Los booleanos son `bool`.
- Los números son `int`.

Por esta razón, las consultas no necesitan volver a convertir la mayoría de los valores.

### Tiempo DataFrame

El tiempo DataFrame fue:

```text
8.732 ms
```

Aunque los `DataFrame` pueden ser eficientes para operaciones tabulares, en este caso se utilizan principalmente con columnas de texto y se realizan muchas conversiones manuales.

Además, varias consultas:

- Recorren todas las filas.
- Crean un `HashSet`.
- Crean una lista de índices.
- Construyen un nuevo `PrimitiveDataFrameColumn`.
- Construyen otro `DataFrame`.
- Agrupan los datos.
- Convierten los valores de nuevo a texto o números.

Estas operaciones se repiten para numerosas consultas.

Por tanto, el código no está aprovechando completamente las ventajas de un procesamiento columnar.

---

## Análisis de resultados

### Número total de registros

Ambos enfoques obtienen:

```text
130.864 registros
```

Esto indica que la carga inicial y el número de filas del fichero combinado coinciden.

Sin embargo, este valor representa filas del fichero, no necesariamente accidentes únicos. El dataset puede contener varias personas relacionadas con el mismo expediente.

### Distritos con más accidentes

Los distritos con más registros son:

- Carabanchel.
- Puente de Vallecas.
- Chamartín.
- Salamanca.
- Ciudad Lineal.

Estos resultados coinciden entre LINQ y DataFrame, lo que indica que en esta consulta ambos enfoques están produciendo resultados equivalentes.

### Tipos de accidente

La categoría más frecuente es `ColisionDoble`, seguida de `Alcance` y `ChoqueObstaculo`.

Los valores coinciden en ambos enfoques, aunque el orden de algunos elementos cambia. Esto parece deberse a diferencias en la ordenación, ya que no siempre se utiliza el mismo criterio de ordenación secundaria.

### Estado meteorológico

La mayoría de los accidentes se producen con tiempo despejado:

```text
Despejado: 39.311
```

También existe un número importante de registros cuyo estado meteorológico es desconocido.

Esto no significa necesariamente que el tiempo despejado provoque más accidentes. Puede reflejar que:

- Hay más horas de circulación con tiempo despejado.
- Los datos meteorológicos no están disponibles para todos los registros.
- La distribución temporal de los accidentes no es uniforme.

### Sexo y edad

Los hombres representan la mayoría de los registros asociados a personas implicadas:

```text
Hombres: 78.820
Mujeres: 38.090
Desconocido: 13.954
```

El rango con más registros conocidos es el de 45 a 49 años, aunque existe también una cantidad elevada de edades desconocidas.

La presencia de valores desconocidos debe tenerse en cuenta antes de extraer conclusiones estadísticas definitivas.

### Alcohol y drogas

Se obtienen:

```text
Positivos en alcohol: 3.572
Positivos en drogas: 455
Positivos simultáneos: 53
```

Los casos simultáneos son una parte reducida del conjunto total.

Carabanchel y Puente de Vallecas aparecen entre los distritos con más positivos de alcohol, aunque esto puede estar relacionado también con el volumen general de accidentes de esos distritos.

### Distribución semanal

Los días laborables concentran más registros que los fines de semana:

```text
Entre semana: 98.645
Fin de semana: 32.219
```

Esto puede deberse a que durante los días laborables existe:

- Mayor movilidad por trabajo y estudios.
- Más desplazamientos recurrentes.
- Mayor tráfico en horas punta.

### Distribución mensual

Agosto presenta un número especialmente reducido de registros:

```text
Agosto: 2.287
```

Esto puede corresponder a una menor actividad registrada, a una diferencia en la fuente de datos o a una cobertura incompleta.

Por este motivo, los meses no deben compararse sin comprobar previamente que todos contienen el mismo nivel de cobertura.

### Evolución anual

Los registros son:

```text
2024: 20.698
2025: 21.688
2026: 13.144
```

El año 2026 parece incompleto porque solamente aparecen datos hasta julio. Por tanto, no debe interpretarse como una reducción anual definitiva frente a 2024 o 2025.

### Peatones

Se obtienen 3.928 expedientes con peatones implicados.

El distrito con más peatones es Centro, seguido de Carabanchel y Puente de Vallecas.

El rango de edad más frecuente entre peatones vulnerables es:

```text
Más de 74 años: 538
```

Este resultado puede indicar una mayor vulnerabilidad de las personas mayores, aunque sería necesario conocer el número total de peatones por rango de edad para calcular tasas y no únicamente frecuencias absolutas.

---

## Por qué DataFrame obtiene peor tiempo

La hipótesis principal es que las operaciones con `DataFrame` no están planteadas de forma óptima ni son completamente equivalentes a las de LINQ.

Aunque el procesamiento tabular puede ser más eficiente en determinados escenarios, utilizar un `DataFrame` no garantiza automáticamente un mejor rendimiento.

En este caso se observan varios problemas.

### 1. Se reconstruyen datos únicos repetidamente

Muchas consultas repiten este patrón:

```csharp
var vistos = new HashSet<string>();
var indicesUnicos = new List<long>();
```

Después se recorre todo el `DataFrame` para obtener índices únicos y se crea una nueva estructura:

```csharp
DataFrame accidentesUnicos =
    accidentes[new PrimitiveDataFrameColumn<long>("indices", indicesUnicos)];
```

Este proceso se repite en muchas consultas. Sería más eficiente crear una única versión deduplicada al principio y reutilizarla.

### 2. Se crean muchos `DataFrame` temporales

Operaciones como:

```csharp
Filter(...)
GroupBy(...)
OrderByDescending(...)
Head(...)
```

pueden crear estructuras temporales y provocar copias o recorridos adicionales.

Al ejecutarse 30 consultas, el coste acumulado puede ser superior al de varias operaciones LINQ sobre una lista ya cargada en memoria.

### 3. Todas las columnas se cargan como texto

El lector utiliza:

```csharp
Type[] dataTypes = Enumerable.Repeat(typeof(string), 19).ToArray();
```

Esto obliga a convertir constantemente:

- Fechas.
- Horas.
- Códigos numéricos.
- Valores booleanos.

Por ejemplo:

```csharp
DateTime.TryParse(...)
int.TryParse(...)
Convert.ToInt32(...)
```

Una solución más eficiente sería cargar cada columna con su tipo real.

### 4. Se repite el parseo de fechas

Varias consultas convierten la misma fecha repetidamente:

```csharp
DateTime.TryParse(fila[0]?.ToString(), out var f)
```

Sería mejor crear una columna tipada de fecha y columnas derivadas:

- Año.
- Mes.
- Día de la semana.
- Hora.
- Tipo de día.

Así las consultas solo utilizarían columnas ya preparadas.

### 5. La deduplicación no es consistente

LINQ utiliza `DistinctBy(a => a.NumExpediente)` en determinadas consultas, pero no en todas.

DataFrame también aplica deduplicación en algunas consultas y la omite en otras.

Por tanto, algunos resultados cuentan:

- Accidentes únicos.
- Personas implicadas.
- Filas del CSV.

Estas unidades no son equivalentes.

### 6. El lector CSV no es robusto

El lector DataFrame elimina todas las comillas del fichero:

```csharp
File.ReadAllText(path).Replace("\"", "");
```

Esto no es un parser CSV completo. Si una columna contiene un separador dentro de comillas, el contenido podría dividirse incorrectamente.

La lectura también debería tratar correctamente:

- Comillas escapadas.
- Separadores dentro de valores.
- Saltos de línea.
- Valores vacíos.

### 7. El coste medido no es exactamente el mismo

La medición de LINQ empieza después de importar los CSV.

La medición de DataFrame incluye parte de la preparación y carga del fichero combinado.

Por tanto, antes de concluir que DataFrame es más lento habría que medir por separado:

- Lectura.
- Transformación.
- Escritura del CSV combinado.
- Carga en DataFrame.
- Consultas.

---

## Tabla comparativa

| Característica | LINQ | DataFrame |
|---|---|---|
| Tiempo medido | 1.380 ms | 8.732 ms |
| Tipo de datos | Objetos tipados | Principalmente cadenas |
| Conversión de fechas | Realizada durante la importación | Repetida durante las consultas |
| Deduplicación | `DistinctBy` en varias consultas | `HashSet` y columnas de índices repetidas |
| Uso de memoria | Lista de objetos | DataFrames temporales |
| Resultados | Generalmente coherentes | Algunas discrepancias importantes |
| Facilidad de lectura | Alta | Media |
| Aprovechamiento de columnas tipadas | Alto | Bajo en la implementación actual |
| Coste de preparación | Bajo después de importar | Alto por filtros y conversiones |
| Adecuación actual | Mejor para este volumen y diseño | Necesita optimización y correcciones |

La conclusión no es que LINQ sea siempre más rápido que un `DataFrame`. La conclusión es que **esta implementación concreta de LINQ está mejor preparada para las operaciones realizadas que esta implementación concreta de `DataFrame`**.

---

## Limitaciones detectadas

### Conteo de filas frente a accidentes únicos

`TotalAccidentes` en DataFrame utiliza:

```csharp
return (int)accidentes.Rows.Count;
```

Esto cuenta filas, que pueden representar personas implicadas, no necesariamente expedientes únicos.

Para comparar con las consultas que utilizan `DistinctBy`, debería contarse el número de `NumExpediente` distintos.

### Nombre y comportamiento de algunas consultas

La consulta `LesionesMasFrecuentes` no está definida de forma coherente:

- LINQ agrupa por `TipoAccidente`.
- DataFrame agrupa por `Lesividad`.
- La interfaz devuelve `TipoAccidente`.

La intención debería definirse claramente antes de comparar resultados.

### Uso de valores por defecto

Los valores no interpretables se convierten en:

- `DateTime.MinValue`.
- `TimeSpan.Zero`.
- `0`.
- `false`.
- Valores `Desconocido`.

Esto evita errores, pero puede introducir grupos artificiales y afectar a los resultados estadísticos.

### Año 2026 incompleto

El año 2026 solo contiene registros hasta julio. Las comparaciones anuales deben tener en cuenta que no se trata de un año completo.

---

## Mejoras futuras

### 1. Medir cada fase por separado

Se recomienda utilizar cronómetros independientes para:

```text
Lectura de los CSV
Conversión a objetos
Escritura del fichero combinado
Carga del DataFrame
Ejecución LINQ
Ejecución DataFrame
```

### 2. Crear un DataFrame tipado

Las columnas deberían cargarse utilizando tipos adecuados:

- Fecha: `DateTime`.
- Hora: `TimeSpan` o número entero.
- Código de distrito: `int`.
- Booleanos: `bool`.
- Tipo de accidente: categoría o texto normalizado.

### 3. Deduplicar una sola vez

Debe crearse una colección única por expediente al inicio:

```text
DataFrame original
        ↓
DataFrame deduplicado
        ↓
Todas las consultas
```

No debería recalcularse la lista de índices únicos en cada método.

### 4. Crear columnas derivadas

Antes de ejecutar las consultas se podrían generar:

- `Anio`.
- `Mes`.
- `DiaSemana`.
- `Hora`.
- `EsFinDeSemana`.
- `EsPositivoAlcohol`.
- `EsPositivoDroga`.

Esto evitaría repetir conversiones.

### 5. Unificar la definición de cada métrica

Cada consulta debería especificar si cuenta:

- Filas.
- Personas.
- Vehículos.
- Expedientes.
- Accidentes únicos.

La misma definición debe utilizarse tanto en LINQ como en DataFrame.

### 6. Corregir la consulta de lesiones

Hay que decidir si la consulta representa:

- Tipo de accidente.
- Código de lesividad.
- Gravedad.

Después se debe modificar la interfaz y ambos analizadores para que devuelvan el mismo tipo de información.

### 7. Usar un parser CSV real

La lectura debería sustituir el uso de `Split(';')` y `Replace("\"", "")` por una librería especializada o un parser que soporte correctamente el formato CSV.

### 8. Crear pruebas de equivalencia

Cada consulta debería tener una prueba que compare los resultados LINQ y DataFrame.

Por ejemplo:

```text
El total por distrito obtenido con LINQ
debe ser igual al total por distrito obtenido con DataFrame.
```

Esto permitiría localizar rápidamente las consultas que no son equivalentes.

### 9. Repetir las mediciones

Una única ejecución no es suficiente para realizar una comparación definitiva.

Se recomienda:

- Ejecutar en modo Release.
- Ejecutar varias iteraciones.
- Ignorar la primera ejecución por efectos de calentamiento.
- Calcular media, mínimo y máximo.
- Ejecutar ambos enfoques sobre exactamente los mismos datos preparados.

---

## Conclusiones

La aplicación cumple el objetivo de importar y analizar los datos de accidentes mediante LINQ y `DataFrame`.

Los resultados principales muestran que:

- Se procesan 130.864 registros.
- Carabanchel y Puente de Vallecas aparecen entre los distritos con más accidentes.
- Las colisiones dobles son uno de los tipos más frecuentes.
- Los accidentes son más numerosos entre semana.
- Los positivos de alcohol son más frecuentes que los positivos de drogas.
- Las personas mayores aparecen como uno de los grupos más vulnerables entre los peatones.
- Los datos de 2026 parecen incompletos.

En cuanto al rendimiento, LINQ obtiene:

```text
1.380 ms
```

mientras que DataFrame obtiene:

```text
8.732 ms
```

Por tanto, en esta ejecución LINQ es claramente más rápido.

La explicación más probable es que las operaciones DataFrame se han implementado de forma poco eficiente y no siempre realizan las mismas operaciones que LINQ. Se repiten deduplicaciones, conversiones de fechas, filtros, agrupaciones y creación de estructuras temporales. Además, todas las columnas se cargan como texto, lo que impide aprovechar completamente las ventajas de un procesamiento tabular tipado.

Por ello, considero que el resultado obtenido no demuestra que `DataFrame` sea menos eficiente que LINQ en términos generales. Lo que demuestra es que la implementación actual de las consultas mediante `DataFrame` no está aprovechando correctamente las ventajas de este tipo de estructura y, además, no siempre está realizando exactamente las mismas operaciones que la versión basada en LINQ.

Un `DataFrame` puede ser muy eficiente cuando se trabaja con grandes volúmenes de datos, columnas correctamente tipadas, operaciones vectorizadas y transformaciones realizadas de forma masiva. Sin embargo, en esta implementación se combinan operaciones propias de un procesamiento tabular con numerosos recorridos manuales fila a fila, conversiones de texto, creación de estructuras temporales y deduplicaciones repetidas. En consecuencia, el coste de preparación y transformación puede superar ampliamente el beneficio esperado del uso de `DataFrame`.