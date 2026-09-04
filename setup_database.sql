-- Script de configuración para SMART ERP
-- Base de datos: BD_RESERMA
-- Usuario: RESERMA

-- Crear base de datos
CREATE DATABASE IF NOT EXISTS BD_RESERMA
CHARACTER SET utf8mb4
COLLATE utf8mb4_unicode_ci;

-- Crear usuario RESERMA
CREATE USER IF NOT EXISTS 'RESERMA'@'localhost' IDENTIFIED BY 'Reserma2024*';

-- Otorgar todos los privilegios al usuario RESERMA en la base de datos BD_RESERMA
GRANT ALL PRIVILEGES ON BD_RESERMA.* TO 'RESERMA'@'localhost';

-- Aplicar cambios
FLUSH PRIVILEGES;

-- Mostrar mensaje de éxito
SELECT 'Base de datos BD_RESERMA creada exitosamente' AS Mensaje;
SELECT 'Usuario RESERMA creado con contraseña: Reserma2024*' AS Mensaje;
