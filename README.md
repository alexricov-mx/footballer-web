# Footballer Web App - Frontend

⚽ **Aplicación web moderna para la gestión de torneos de fútbol**

Una aplicación web construida con **Blazor Server** que proporciona una interfaz intuitiva y responsive para la gestión de torneos de fútbol, equipos y estadísticas.

## 🚀 Características

- **🎠 Carousel Dinámico**: Homepage con carousel de imágenes que cambia cada 4 segundos
- **📊 Gestión de Contenido**: Contenido dinámico cargado desde API
- **📱 Diseño Responsivo**: Optimizado para desktop, tablet y móvil
- **⚡ Blazor Server**: Interactividad en tiempo real
- **🎨 UI Moderna**: Diseño limpio con animaciones suaves
- **🔄 Auto-actualización**: Contenido editable sin recompilación

## 🛠️ Tecnologías

- **Framework**: Blazor Server (.NET 10.0 Preview)
- **Lenguaje**: C# 13 + Razor Pages
- **Frontend**: HTML5, CSS3, JavaScript
- **Estilos**: Bootstrap 5 + CSS personalizado
- **Iconos**: Open Iconic
- **API**: Integración RESTful con footballer-api

## 📁 Estructura del Proyecto

```
FootballerWeb/
├── Pages/
│   ├── Index.razor              # Homepage con carousel
│   ├── _Layout.cshtml           # Layout principal
│   └── Auth/
│       └── Login.razor          # Página de login
├── Services/
│   └── ContentService.cs       # Servicio para consumir API
├── DTOs/
│   └── AuthDTOs.cs             # Data Transfer Objects
├── wwwroot/
│   ├── css/
│   │   ├── homepage.css        # Estilos del carousel y features
│   │   └── site.css           # Estilos generales
│   └── js/
│       └── homepage.js         # JavaScript para animaciones
└── Program.cs                  # Configuración de la aplicación
```

## 🎨 Características del Carousel

### 🖼️ **Imágenes Optimizadas**
- **Resolución**: 2000x1200px mínimo
- **Formato**: JPG optimizado para web
- **Tamaño**: Máximo 800KB por imagen
- **Aspect Ratio**: 16:9 o 5:3
- **Contenido**: Estadios, jugadores, celebraciones

### ⏱️ **Funcionalidades**
- **Auto-play**: Cambio automático cada 4 segundos
- **Controles manuales**: Flechas de navegación
- **Indicadores**: Puntos clicables para navegación directa
- **Responsive**: Adaptable a todos los tamaños de pantalla
- **Smooth transitions**: Transiciones suaves entre imágenes

### 📱 **Diseño Responsivo**
- **Desktop**: Pantalla completa con efectos parallax
- **Tablet (768px)**: Controles adaptados
- **Móvil (576px)**: Altura optimizada, controles táctiles
- **Móvil pequeño (400px)**: Completamente optimizado

## 🏆 Sección de Características

**3 Características Principales** (editables desde API):

1. **🏆 Gestión de Torneos** - Fixtures automáticos y clasificaciones
2. **👥 Administración de Equipos** - Gestión completa de equipos
3. **📊 Estadísticas Avanzadas** - Métricas y reportes detallados

## 🚀 Instalación y Ejecución

### Prerrequisitos
- .NET 10.0 SDK (Preview) o superior
- [Footballer API](https://github.com/alexricov-mx/footballer-api) ejecutándose
- Visual Studio 2025 o VS Code

### Pasos de instalación

1. **Clonar el repositorio**
   ```bash
   git clone https://github.com/alexricov-mx/footballer-web.git
   cd footballer-web
   ```

2. **Restaurar dependencias**
   ```bash
   dotnet restore
   ```

3. **Configurar la API**
   
   Asegúrate de que la API esté ejecutándose en:
   ```
   https://localhost:8090
   ```

4. **Compilar el proyecto**
   ```bash
   dotnet build
   ```

5. **Ejecutar la aplicación**
   ```bash
   dotnet run
   ```

6. **Acceder a la aplicación**
   - Web App: `https://localhost:7003`
   - HTTP: `http://localhost:7004`

## ⚙️ Configuración

### Configurar URL de la API

En `Program.cs`, configura la URL base de la API:

```csharp
builder.Services.AddHttpClient<IContentService, ContentService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:8090/");
});
```

### Variables de Entorno

```bash
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_HTTPS_PORT=7003
ASPNETCORE_HTTP_PORT=7004
```

## 🎯 Funcionalidades Principales

### 🏠 **Homepage**
- Carousel automático con 4 imágenes de fútbol profesionales
- Timer configurable (4 segundos por defecto)
- Navegación manual con indicadores y flechas
- Sección de características cargada dinámicamente
- Scroll suave entre secciones
- Animaciones de entrada escalonadas

### 🔧 **Servicios**
- **ContentService**: Consumo de API para contenido dinámico
- **HttpClient**: Configurado para comunicación con backend
- **Inyección de dependencias**: Servicios registrados automáticamente

### 📊 **DTOs**
```csharp
public class HomepageContent
{
    public HeroSection Hero { get; set; }
    public CarouselData Carousel { get; set; }
    public List<Feature> Features { get; set; }
    public DateTime LastUpdated { get; set; }
}
```

## 🎨 Personalización

### Cambiar Imágenes del Carousel

Las imágenes se cargan desde la API. Para cambiarlas, edita:
```
footballer-api/Data/homepage-content.json
```

### Modificar Estilos

- **Carousel**: `wwwroot/css/homepage.css`
- **Generales**: `wwwroot/css/site.css`
- **Animaciones**: `wwwroot/js/homepage.js`

### Agregar Nuevas Páginas

1. Crear archivo `.razor` en `Pages/`
2. Agregar rutas con `@page "/ruta"`
3. Registrar servicios necesarios en `Program.cs`

## 🔧 Desarrollo

### Estructura de Componentes Razor

```razor
@page "/"
@inject IContentService ContentService

<h1>@content.Hero.Title</h1>

@code {
    private HomepageContent? content;
    
    protected override async Task OnInitializedAsync()
    {
        content = await ContentService.GetHomepageContentAsync();
    }
}
```

### Estilos CSS Responsivos

```css
/* Desktop */
.hero-title { font-size: 4.5rem; }

/* Tablet */
@media (max-width: 991px) {
    .hero-title { font-size: 3.5rem; }
}

/* Mobile */
@media (max-width: 767px) {
    .hero-title { font-size: 2.8rem; }
}
```

## 🤝 Contribuir

1. Fork el proyecto
2. Crear una rama feature (`git checkout -b feature/nueva-funcionalidad`)
3. Commit los cambios (`git commit -am 'Agregar nueva funcionalidad'`)
4. Push a la rama (`git push origin feature/nueva-funcionalidad`)
5. Crear un Pull Request

## 📄 Licencia

Este proyecto está bajo la Licencia MIT - ver el archivo [LICENSE](LICENSE) para detalles.

## 👨‍💻 Autor

**Alejandro Rico Vázquez** - [@alexricov-mx](https://github.com/alexricov-mx)

## 🔗 Enlaces Relacionados

- [Footballer API - Backend](https://github.com/alexricov-mx/footballer-api)
- [Demo en vivo](https://localhost:7003) (cuando esté desplegado)

## 📸 Screenshots

### 🏠 Homepage con Carousel
![Homepage](https://via.placeholder.com/800x400/1e3c72/ffffff?text=Homepage+Carousel)

### 📱 Vista Móvil
![Mobile View](https://via.placeholder.com/400x600/2a5298/ffffff?text=Mobile+Responsive)

### 🏆 Sección de Características  
![Features](https://via.placeholder.com/800x300/f8f9fc/2c3e50?text=Dynamic+Features)

---

⚽ **¡Hecho con pasión por el fútbol y la tecnología!** ⚽

## 🏃‍♂️ Quick Start

```bash
# Clonar ambos repositorios
git clone https://github.com/alexricov-mx/footballer-api.git
git clone https://github.com/alexricov-mx/footballer-web.git

# Ejecutar API (Terminal 1)
cd footballer-api
dotnet run

# Ejecutar Web App (Terminal 2)  
cd footballer-web
dotnet run

# Abrir navegador
# https://localhost:7003
```