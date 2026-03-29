# 📘 Manual de Usuario
## Sistema de Gestión de Proyectos - Assessment SENA

---

### 1. Requisitos del Sistema
Para instalar y ejecutar este sistema de forma local, el desarrollador o administrador debe contar con el siguiente software en su máquina:

*   **SDK .NET 8.0:** Requerido para compilar y ejecutar el servidor web C# ([Descargar aquí](https://dotnet.microsoft.com/download/dotnet/8.0)).
*   **PostgreSQL:** O configuración remota de base de datos relacional (como `Supabase`).

> **Nota:** A diferencia de SPA basadas en npm, en esta arquitectura monolítica *NO es necesario instalar Node.js* y todo el frontend ya está construido en la arquitectura.

### 2. Pasos de Instalación y Ejecución

#### 2.1 Configuración de la Base de Datos
1.  En el backend (carpeta `CoursePlatform.Api/`), abra el archivo `appsettings.json`.
2.  Confirme o modifique la cadena de text en la propiedad `ConnectionStrings:DefaultConnection` si usa su propia instancia local de PostgreSQL.

#### 2.2 Ejecución del Sistema Web
1.  Abra una terminal directamente dentro de la carpeta `CoursePlatform.Api/` (o en la raíz del SLN).
2.  Despliegue estructuralmente la base de datos EF Core (generar tablas de Identity, Proyectos y Permisos):
    ```bash
    dotnet ef database update --project ../CoursePlatform.Infrastructure --startup-project .
    ```
3.  Inicie el servidor embebido `Kestrel` nativamente con el comando general de .Net:
    ```bash
    dotnet run
    ```
4.  Abra su navegador y acceda automáticamente al panel del puerto nativo generado:
    `http://localhost:5119/Projects` (puede variar si el sistema asignó otro puerto).

### 3. Descripción de las Funcionalidades

#### Pantalla de Autenticación
*   **Registro Automático:** Para realizar pruebas como estudiante o miembro del proyecto, cree una cuenta rápida en `/Auth/Register`. La cuenta es generada internamente bajo el rol restrictivo general **User**.
*   **Seguridad:** Las credenciales están encriptadas con algoritmos robustos (Bcrypt integrados) y la sesión es persistida por Cookies del sistema nativo.

#### Gestión y Navegación de Proyectos
*   **Bandeja de Proyectos:** El menú principal le mostrará `Proyectos Activos`. Aquí se presenta: 
   *    Cantidad de Tareas Totales dentro del proyecto.
   *    Un medidor visual (*ProgressBar*) que calcula de 0 a 100% las tareas en estado "Completado".
*   **Inscribirse a Proyecto (`Enroll`):** Los usuarios **Usuario** deben hacer clic en el botón estrella `Inscribirse` de aquellos proyectos activos en los que vayan a participar y ver sus tareas.

#### Tablero Avanzado de Gestión de Tareas (Kanban list)
*   Al entrar a un proyecto estando inscrito, visualizará un *layout oscuro Premium*.
*   **Para Administradores:** Aparecerán botones para agregar subtareas, reordenarlas jerárquicamente con flechas direccionales (`▲` `▼`), así como opciones de borrado (`🗑`) y edición de títulos/prioridad.
*   **Para Usuarios Inscritos:** En la misma pantalla, podrán utilizar dinámicamente cuadros de selección simple (Dropdowns nativos) para seleccionar el avance explícito de cada asignación:
    1.  `○ Por Hacer` (Básico - Gris)
    2.  `⏳ En Proceso` (Activo - Naranja)
    3.  `✔ Completada` (Finalizado - Verde).
    Al modificar el selector, el servidor actualizará silenciosamente la vista completa mostrando los contadores automáticos reformulándose con los nuevos cambios de estadística en tiempo real.

### 4. Capturas de Pantalla de Referencia
*Por políticas de formato Markdown estas son representaciones para ilustración textual del diseño.*

#### Tablero de Tareas con Estados Dinámicos en Oscuro
> | ID / Prioridad | Nombre de la Tarea  | Estado Atual | Opciones (Dropwdown) |
> | :---  | :--- | :--- | :--- |
> | #1 Alta | Configurar Base de Datos | **✔ Completada** (Verde) | `[✔ Completada] v` |
> | #2 Normal | Renderizado de Razor Views | **⏳ En Proceso** (Naranja) | `[⏳ En Proceso] v` |

### 5. Resolución de Problemas (Troubleshooting)
*   **El API rechaza conexión / Address ya está en uso:** Asegúrese de cerrar la terminal previa o usar `ps` y matar el hilo en Linux / `taskkill` en Windows antes de correr `dotnet run` por segunda vez.
*   **La sesión de Identity caduca o se borra aleatoriamente:** Durante el desarrollo local asegúrese de no borrar sus cookies; pruebe en una capa general sin modo incógnito múltiple.
