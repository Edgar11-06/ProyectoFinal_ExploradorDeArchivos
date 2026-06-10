# 🗂️ Sistema Multimedia - Explorador de Archivos Avanzado

## 📖 Descripción

Sistema Multimedia es una aplicación de escritorio desarrollada en **C# .NET 8 (Windows Forms)** que funciona como un explorador de archivos avanzado con herramientas integradas para la gestión, visualización, edición y análisis de diferentes tipos de contenido multimedia y documental.

El proyecto fue desarrollado como proyecto final académico y combina funcionalidades de exploración de archivos con módulos especializados para:

- Gestión de archivos y carpetas.
- Reproducción de audio y video.
- Captura de imágenes y video desde cámara.
- Edición de imágenes.
- Visualización y edición de documentos.
- Geolocalización de fotografías.
- Validación y limpieza de datos.
- Integración con bases de datos.
- Envío de archivos por correo electrónico.
- Generación de reportes y análisis de información.

---

# ✨ Características Principales

## 📁 Explorador de Archivos

- Navegación por unidades y directorios.
- Historial de navegación.
- Visualización detallada de archivos.
- Renombrado de archivos y carpetas.
- Eliminación de elementos.
- Propiedades de archivos.
- Menús contextuales.
- Generación de miniaturas.

### Modos de visualización

- Lista
- Detalles
- Iconos pequeños
- Iconos medianos
- Iconos grandes
- Vista tipo explorador

---

## 🎵 Reproductor Multimedia

### Audio

- Reproducción de archivos MP3.
- Control de reproducción.
- Gestión de metadatos musicales.
- Grabación de audio.

### Video

- Reproducción de video mediante VLC.
- Soporte para múltiples formatos.
- Controles de reproducción.

---

## 📷 Cámara y Captura Multimedia

- Detección de dispositivos de video.
- Captura de fotografías.
- Grabación de video.
- Gestión de imágenes obtenidas.

---

## 🖼️ Editor de Imágenes

Herramientas integradas para:

- Redimensionar imágenes.
- Insertar texto.
- Aplicar modificaciones básicas.
- Guardar imágenes editadas.
- Visualización previa.

---

## 🌎 Geolocalización de Fotografías

Módulo especializado para:

- Lectura de metadatos GPS.
- Visualización de ubicación geográfica.
- Integración con mapas mediante WebView2.
- Análisis de imágenes georreferenciadas.

---

## 📄 Visor y Editor de Documentos

Permite trabajar con:

### Documentos Word

- Apertura
- Visualización
- Conversión

### PDF

- Visualización
- Exportación

### Excel

- Conversión a HTML
- Visualización estructurada

### Navegación Web

- Integración mediante WebView2.

---

## 📊 Validación y Limpieza de Datos

Módulo independiente para análisis de datos.

### Funcionalidades

- Validación de registros.
- Limpieza de datos inconsistentes.
- Identificación de errores.
- Exportación de resultados.
- Generación de reportes.

### Tipos de datos soportados

- CSV
- TXT
- JSON
- XML

---

## 🔄 Data Fusion

Herramienta para consolidación de información proveniente de múltiples fuentes.

### Importación de datos

- CSV
- TXT
- JSON
- XML

### Bases de datos soportadas

- SQL Server
- PostgreSQL
- MariaDB / MySQL

### Capacidades

- Agrupamiento de registros.
- Transformación de datos.
- Resúmenes estadísticos.
- Consolidación de información.

---

## 📧 Envío de Archivos por Correo

El sistema permite:

- Adjuntar archivos directamente.
- Configuración SMTP.
- Envío desde la interfaz gráfica.
- Compartición rápida de documentos.

---

# 🏗️ Arquitectura del Proyecto

La solución está organizada en varios proyectos independientes.

## SistemaMultimedia

Proyecto principal de la aplicación.

Responsabilidades:

- Explorador de archivos.
- Reproducción multimedia.
- Cámara.
- Editor de imágenes.
- Integración entre módulos.
- Envío de correo.

---

## VisorEditorDocumentos

Módulo encargado de:

- Visualización de documentos.
- Conversión de formatos.
- Exportación.
- Gestión de PDF, Word y Excel.

---

## FotoGeolocalizada

Módulo especializado para:

- Lectura de metadatos GPS.
- Geolocalización de imágenes.
- Integración con mapas.

---

## DataValidationModule

Módulo para:

- Validación de datos.
- Limpieza de información.
- Generación de reportes.

---

## Imagenes.Shared

Biblioteca compartida para:

- Procesamiento de imágenes.
- Utilidades reutilizables.
- Componentes comunes.

---

# 🛠️ Tecnologías Utilizadas

## Lenguaje

- C#

## Framework

- .NET 8

## Interfaz Gráfica

- Windows Forms

## Bases de Datos

- SQL Server
- PostgreSQL
- MariaDB
- MySQL

## Librerías Externas

### Multimedia

- NAudio
- NAudio.Lame
- LibVLCSharp
- VideoLAN.LibVLC
- DirectShowLib

### Procesamiento de Imágenes

- OpenCvSharp

### Documentos

- DocumentFormat.OpenXml
- PdfiumViewer
- HtmlAgilityPack

### Navegación Web

- Microsoft.Web.WebView2

### Bases de Datos

- Microsoft.Data.SqlClient
- Npgsql
- MySqlConnector

### Metadatos

- TagLibSharp

---

# 📂 Estructura del Proyecto

```text
SistemaMultimedia.sln
│
├── SistemaMultimedia
├── VisorEditorDocumentos
├── FotoGeolocalizada
├── DataValidationModule
└── Imagenes.Shared
```

---

# ⚙️ Requisitos

- Windows 10 o superior
- Visual Studio 2022
- .NET 8 SDK

---

# 🚀 Instalación

## 1. Clonar repositorio

```bash
git clone https://github.com/Edgar11-06/ProyectoFinal_ExploradorDeArchivos.git
```

## 2. Abrir solución

```text
SistemaMultimedia.sln
```

## 3. Restaurar dependencias

```bash
dotnet restore
```

## 4. Compilar

```bash
dotnet build
```

## 5. Ejecutar

Seleccionar el proyecto principal SistemaMultimedia y ejecutar.

---

# 📷 Funcionalidades Destacadas

| Función | Descripción |
|----------|------------|
| Explorador de archivos | Navegación completa del sistema |
| Reproductor MP3 | Audio integrado |
| Reproductor de video | VLC integrado |
| Cámara | Captura y grabación |
| Editor de imágenes | Modificación básica |
| Visor PDF | Lectura de documentos |
| Word/Excel | Conversión y visualización |
| Geolocalización | Mapas y coordenadas GPS |
| Data Fusion | Integración de fuentes de datos |
| Validación de datos | Limpieza y análisis |
| Correo electrónico | Compartición de archivos |

---

# 🔒 Manejo de Errores

- Captura de excepciones.
- Validación de entradas.
- Verificación de archivos.
- Comprobación de permisos.
- Mensajes descriptivos para el usuario.

---

# 📈 Posibles Mejoras Futuras

- Soporte para almacenamiento en la nube.
- Sincronización con OneDrive y Google Drive.
- Búsqueda avanzada.
- Etiquetado automático de archivos.
- Inteligencia artificial para clasificación documental.
- Exportación de reportes avanzados.
- Modo oscuro.
- Sistema de plugins.

---

# 👨‍💻 Autor

**Edgar**

Proyecto desarrollado como trabajo final académico para la asignatura de desarrollo de software.

---

# 📄 Licencia

Este proyecto tiene fines educativos y académicos.

Su uso, modificación y distribución quedan sujetos a las políticas establecidas por la institución educativa correspondiente.
