# API de Gestión de Ventas

Este es un proyecto de API desarrollado en C# que permite la gestión de ventas, usuarios y productos.

## Tablas del Proyecto

El proyecto cuenta con tres tablas principales:

- VENTA
- USUARIO
- PRODUCTO

## Funcionalidades Principales

### 1. Ingreso de Usuario Nuevo

Permite el ingreso de un nuevo usuario al sistema.

### 2. Ingreso de Producto Nuevo

Permite el ingreso de un nuevo producto al sistema.

### 3. Ingreso de Venta Nueva

Permite el registro de una nueva venta, relacionada con un usuario y un producto. Si el usuario está inhabilitado, se rechaza la operación.

### 4. Validaciones

- Los campos ingresados al insertar, editar o eliminar datos son validados para asegurarse de que no estén vacíos.
- Si ocurre un fallo en alguna operación, se controla mediante un bloque try.

### 5. Cambio de Estado de Usuario

Permite cambiar el estado de un usuario. El estado 0 significa inhabilitado, y 1 habilitado.

### 6. Estado de Ventas

Una venta no puede ser eliminada, solo puede cambiar su estado. 0 significa anulada, 1 significa realizada (por defecto, las nuevas ventas se crean con estado 1).

### 7. Listado de Usuarios

Permite mostrar todos los nombres de usuario y su estado, sin revelar la contraseña. El estado se expresa en texto.

### 8. Listado de Productos

Puede listar todos los productos o buscar uno específico por su ID.

### 9. Listado de Ventas

Permite listar todas las ventas e incluye información como el nombre del usuario, el producto, el precio, la cantidad, el total y el estado en formato de texto (0 anulada y 1 realizada).

### 10. Búsqueda de Ventas

Permite buscar ventas por estado o por usuario. Si no se especifica ningún campo de búsqueda, se muestran todas las ventas realizadas.
