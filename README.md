# 📝 App de Notas — .NET MAUI

## Descripción
Aplicación de notas móvil desarrollada con .NET MAUI 8.0

## Tecnologías
- .NET MAUI 8.0
- SQLite (sqlite-net-pcl)
- Patrón MVVM con INotifyPropertyChanged
- Shell Navigation

## Requisitos para compilar
- Visual Studio 2022 v17.8+
- .NET 8.0 SDK
- Android SDK API 33+
- Workload MAUI instalado

## Compilación
1. Clonar el repositorio
2. Abrir `Notas.sln` en Visual Studio
3. Seleccionar target Android
4. Ejecutar con F5

## Funcionalidades
- ✅ Crear, editar y eliminar notas
- ✅ Persistencia local con SQLite
- ✅ Búsqueda en tiempo real
- ✅ Fijar notas importantes (pin)
- ✅ Orden por fecha y pin
- ✅ Confirmación antes de eliminar
- ✅ Validación de título obligatorio
- ✅ Navegación entre pantallas

## Arquitectura
- **MVVM**: ViewModels separan lógica de la UI
- **SQLite**: Persistencia local con sqlite-net-pcl
- **Shell**: Navegación entre pantallas con paso de parámetros

## Decisiones de diseño
- Se eligió MAUI como alternativa cross-platform
- INotifyPropertyChanged en lugar de LiveData (equivalente MAUI)
- ObservableCollection para actualización reactiva de la lista

## Screenshots


## APK
Ver carpeta `release/`
```

### 3. Informe breve — estructura
```
Arquitectura usada:
Patrón MVVM con .NET MAUI 8.0. ViewModels manejan 
toda la lógica, las vistas solo observan propiedades 
mediante INotifyPropertyChanged y ObservableCollection.
Persistencia con SQLite via sqlite-net-pcl.
Navegación con Shell pasando el id de la nota.

Retos enfrentados:
- ObservableCollection no refrescaba en Android al editar,
  se resolvió implementando INotifyPropertyChanged en el modelo.
- El FAB no respondía en móvil por superposición de la 
  CollectionView, se resolvió con ZIndex y Button nativo.

Mejoras que agregaría:
- Exportar/importar notas en JSON
- Notificaciones recordatorio por nota
- Etiquetas o categorías
- Modo oscuro/claro configurable
- Sincronización en la nube
