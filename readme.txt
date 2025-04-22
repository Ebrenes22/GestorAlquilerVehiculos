SC-701-PAW-G2-L

1. Integrantes finales del grupo:
   - María José Valverde Peña
   - Valeria de los Ángeles Solano Vargas
   - Alejandro Denver Romero
   - Eliecer José Brenes Madrigal

2. Enlace del repositorio:
   https://github.com/Ebrenes22/GestorAlquilerVehiculos.git

3. Especificación básica del proyecto:

a. Arquitectura del proyecto:
   - El sistema sigue una arquitectura basada en ASP.NET Core MVC.
   - Se divide en módulos como: autenticación de usuarios, gestión de reservas, mantenimiento de flota, reportes, notificaciones, y administración.
   - La base de datos se maneja mediante Entity Framework Core con relaciones entre entidades como Usuario, Vehículo, Reserva, ClienteReserva, etc.
   - Se implementan roles y permisos para separar las funcionalidades entre administradores y clientes.
   - El frontend está compuesto por vistas Razor que utilizan Bootstrap y una plantilla moderna responsiva.

b. Libraries o paquetes de NuGet utilizados:
   - Microsoft.EntityFrameworkCore.SqlServer
   - Microsoft.EntityFrameworkCore.Tools
   - Microsoft.AspNetCore.Identity
   - Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation
   - Bootstrap (estilos y estructura visual)
   - SweetAlert2 (para alertas visuales en frontend)
   - jQuery (interacción y validaciones)
   - Otros assets de frontend (iconos, Owl Carousel, animaciones, AOS, etc.)

c. Principios de SOLID y patrones de diseño utilizados:
   - **S - Principio de Responsabilidad Única:** Cada clase como `ReservasController`, `ClienteReservasController`, etc. tiene una única responsabilidad.
   - **O - Principio de Abierto/Cerrado:** La arquitectura está diseñada para permitir la extensión de nuevas funcionalidades sin modificar el código existente.
   - **L - Principio de Sustitución de Liskov:** Las entidades del sistema como `Reserva`, `Vehiculo` o `Usuario` pueden ser utilizadas sin alterar la lógica de herencia o interfaz.
   - **I - Principio de Segregación de Interfaces:** Se utilizan interfaces y servicios por separado para acceso a datos, validaciones y lógica de negocio.
   - **D - Principio de Inversión de Dependencias:** La inyección de dependencias se usa ampliamente a través de los constructores para desacoplar la lógica de negocio del acceso a datos.
   - **Patrones implementados:** 
       - MVC (Model-View-Controller)
       - Repositorio para acceso a datos (en algunos módulos)
       - Unit of Work de forma implícita a través de DbContext
       - Validaciones mediante DataAnnotations y lógica de servidor