-- Este script se ejecuta automáticamente en el contenedor MySQL al inicio.

-- Habilitar el flag local_infile en el servidor. 
-- Esto es crucial para que el script de carga masiva funcione dentro del contenedor.
SET GLOBAL local_infile = 1;

GRANT FILE ON *.* TO 'ApiUser'@'%';

FLUSH PRIVILEGES;
