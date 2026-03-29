# 📊 Diagramas de Lenguaje Unificado de Modelado (UML)
## Sistema de Gestión de Proyectos - Assessment SENA

---

### 1. Diagrama de Casos de Uso
Describe las interacciones de los niveles de autorización con el sistema de Proyectos utilizando un diagrama de flujo adaptado validado para Mermaid.

```mermaid
flowchart LR
    %% Actores definidos como nodos circulares
    Admin((Administrador))
    User((Usuario Inscrito))

    %% Sistema (Subgrafo) con sus casos de uso en forma de píldora
    subgraph SENA Projects
        UC1([Crear / Gestionar Proyectos])
        UC2([Inscribirse a Proyecto])
        UC3([Gestionar Tareas CRUD])
        UC4([Cambiar Estado de Tarea])
        UC5([Ver Progreso Global])
    end

    %% Relaciones Administrador
    Admin --- UC1
    Admin --- UC3
    Admin --- UC5

    %% Relaciones Usuario
    User --- UC2
    User --- UC4
    User --- UC5
```

### 2. Diagrama de Clases
Muestra la estructura estática y las relaciones del Modelo de Dominio de Proyectos (Sintaxis Estricta de Mermaid).

```mermaid
classDiagram
    %% ==========================================
    %% CAPA DE DOMINIO (Entidades y Enums)
    %% ==========================================
    class ApplicationUser {
        +Guid Id
        +string UserName
        +string Email
        +string FullName
    }
    class Project {
        +Guid Id
        +string Name
        +string Description
        +ProjectStatus Status
        +bool IsDeleted
    }
    class TaskItem {
        +Guid Id
        +Guid ProjectId
        +string Title
        +TaskProgressStatus Status
        +Priority Priority
        +int Order
        +bool IsDeleted
    }
    class ProjectEnrollment {
        +Guid Id
        +Guid ProjectId
        +Guid UserId
        +DateTime EnrolledAt
    }
    class TaskProgressStatus {
        <<enumeration>>
        Todo
        InProgress
        Completed
    }
    class Priority {
        <<enumeration>>
        Low
        Medium
        High
    }

    %% ==========================================
    %% CAPA DE APLICACIÓN (Interfaces)
    %% ==========================================
    class IProjectService {
        <<interface>>
        +SearchProjects(status, page, size, userId)
        +EnrollUserAsync(projectId, userId)
        +GetProjectSummary(projectId)
    }
    class ITaskService {
        <<interface>>
        +CreateTask(request)
        +DeleteTask(taskId)
        +ReorderTask(taskId, request)
        +ChangeTaskStatus(taskId, status)
    }

    %% ==========================================
    %% CAPA DE INFRAESTRUCTURA (Base de Datos)
    %% ==========================================
    class AppDbContext {
        <<DbContext>>
        +DbSet Projects
        +DbSet Tasks
        +DbSet ProjectEnrollments
        +SaveChanges()
    }

    %% ==========================================
    %% CAPA WEB / API MVC (Controladores)
    %% ==========================================
    class ProjectsController {
        <<Controller>>
        -IProjectService _projectService
        +Index(statusEnum) IActionResult
        +Enroll(id) IActionResult
    }
    class TasksController {
        <<Controller>>
        -ITaskService _taskService
        +Index(projectId) IActionResult
        +ChangeStatus(id, status) IActionResult
    }

    %% ==========================================
    %% RELACIONES ENTRE LOS COMPONENTES
    %% ==========================================
    
    %% Relación de Composición (Entidades)
    Project "1" *-- "0..*" TaskItem : contiene
    Project "1" *-- "0..*" ProjectEnrollment : inscripciones
    ApplicationUser "1" *-- "0..*" ProjectEnrollment : se inscribe a
    
    %% Relación de Agregación (Enums)
    TaskItem o-- TaskProgressStatus : estado
    TaskItem o-- Priority : prioridad

    %% Relación de Persistencia
    AppDbContext ..> Project : guarda
    AppDbContext ..> TaskItem : guarda
    AppDbContext ..> ProjectEnrollment : guarda

    %% Inyección de Dependencias
    IProjectService ..> AppDbContext : inyecta
    ITaskService ..> AppDbContext : inyecta

    ProjectsController --> IProjectService : utiliza
    TasksController --> ITaskService : utiliza
```

### 3. Diagrama de Secuencia (Flujo de Cambio de Tareas)
Ilustra el proceso mediante el cual un Usuario cambia un estado (Sintaxis Estricta Mermaid).

```mermaid
sequenceDiagram
    participant UI as Browser View
    participant Ctr as TasksController
    participant Svc as TaskService
    participant DB as PostgreSQL
    
    Note over UI, DB: Asumiendo usuario autenticado
    UI->>Ctr: POST /Tasks/ChangeStatus/{id}
    Note right of UI: status: InProgress
    
    Ctr->>Svc: ChangeTaskStatus(taskId, status)
    Svc->>DB: Buscar TaskItem por ID
    DB-->>Svc: TaskItem encontrado
    Svc->>Svc: Actualizar Status = InProgress
    Svc->>DB: SaveChangesAsync()
    DB-->>Svc: OK
    Svc-->>Ctr: Result Dto
    Ctr-->>UI: RedirectToAction("Index")
```

### 4. Modelo de Base de Datos (E-R)
Sintaxis válida nativa de Entidad-Relación en Mermaid (`erDiagram`).

```mermaid
erDiagram
    APPLICATIONUSER ||--o{ PROJECTENROLLMENT : "se inscribe"
    PROJECT ||--o{ PROJECTENROLLMENT : "tiene inscritos"
    PROJECT ||--o{ TASKITEM : "contiene"
    
    APPLICATIONUSER {
        string Id PK
        string Email
        string FullName
    }

    PROJECT {
        string Id PK
        string Name
        string Description
        int Status
        bool IsDeleted
    }
    
    PROJECTENROLLMENT {
        string Id PK
        string ProjectId FK
        string UserId FK
        datetime EnrolledAt
    }

    TASKITEM {
        string Id PK
        string ProjectId FK
        string Title
        int Status
        int Priority
        int Order
        bool IsDeleted
    }
```
