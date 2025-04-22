SC-701-PAW-G2-L

1. Integrantes finales del grupo:
   - Mar�a Jos� Valverde Pe�a
   - Valeria de los �ngeles Solano Vargas
   - Alejandro Denver Romero
   - Eliecer Jos� Brenes Madrigal

2. Enlace del repositorio:
   https://github.com/Ebrenes22/GestorAlquilerVehiculos.git

3. Especificaci�n b�sica del proyecto:

a. Arquitectura del proyecto:
   - El sistema sigue una arquitectura basada en ASP.NET Core MVC.
   - Se divide en m�dulos como: autenticaci�n de usuarios, gesti�n de reservas, mantenimiento de flota, reportes, notificaciones, y administraci�n.
   - La base de datos se maneja mediante Entity Framework Core con relaciones entre entidades como Usuario, Veh�culo, Reserva, ClienteReserva, etc.
   - Se implementan roles y permisos para separar las funcionalidades entre administradores y clientes.
   - El frontend est� compuesto por vistas Razor que utilizan Bootstrap y una plantilla moderna responsiva.

b. Libraries o paquetes de NuGet utilizados:
   - Microsoft.EntityFrameworkCore.SqlServer
   - Microsoft.EntityFrameworkCore.Tools
   - Microsoft.AspNetCore.Identity
   - Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation
   - Bootstrap (estilos y estructura visual)
   - SweetAlert2 (para alertas visuales en frontend)
   - jQuery (interacci�n y validaciones)
   - Otros assets de frontend (iconos, Owl Carousel, animaciones, AOS, etc.)

c. Principios de SOLID y patrones de dise�o utilizados:
   - **S - Principio de Responsabilidad �nica:** Cada clase como `ReservasController`, `ClienteReservasController`, etc. tiene una �nica responsabilidad.
   - **O - Principio de Abierto/Cerrado:** La arquitectura est� dise�ada para permitir la extensi�n de nuevas funcionalidades sin modificar el c�digo existente.
   - **L - Principio de Sustituci�n de Liskov:** Las entidades del sistema como `Reserva`, `Vehiculo` o `Usuario` pueden ser utilizadas sin alterar la l�gica de herencia o interfaz.
   - **I - Principio de Segregaci�n de Interfaces:** Se utilizan interfaces y servicios por separado para acceso a datos, validaciones y l�gica de negocio.
   - **D - Principio de Inversi�n de Dependencias:** La inyecci�n de dependencias se usa ampliamente a trav�s de los constructores para desacoplar la l�gica de negocio del acceso a datos.
   - **Patrones implementados:** 
       - MVC (Model-View-Controller)
       - Repositorio para acceso a datos (en algunos m�dulos)
       - Unit of Work de forma impl�cita a trav�s de DbContext
       - Validaciones mediante DataAnnotations y l�gica de servidor