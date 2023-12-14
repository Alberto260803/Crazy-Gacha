-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Servidor: 127.0.0.1
-- Tiempo de generación: 13-12-2023 a las 21:59:09
-- Versión del servidor: 10.4.28-MariaDB
-- Versión de PHP: 8.2.4

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de datos: `crazy_gacha`
--

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `premios`
--

CREATE TABLE `premios` (
  `id` int(255) NOT NULL,
  `nombre` varchar(1000) NOT NULL,
  `rareza` varchar(1000) NOT NULL,
  `recompensa` float NOT NULL,
  `linkImagen` mediumtext NOT NULL,
  `linkAudio` mediumtext NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `premios`
--

INSERT INTO `premios` (`id`, `nombre`, `rareza`, `recompensa`, `linkImagen`, `linkAudio`) VALUES
(1, 'Huevo', 'Ninguna', 0, 'https://i.imgur.com/xGPM1Ye.png', 'https://drive.google.com/uc?id=1pfAQKsV0Snw6F4aFySuMyb5DjeTgvHkT'),
(2, 'Шайлушай', 'Legendaria', 0, 'https://i.imgur.com/ActZHMg.jpg', 'https://drive.google.com/uc?id=1Bi3einKd4OVttmfQpqw5gxCQMLEcFzt8'),
(3, 'Koala y Calamar dorado', 'Común', 0, 'https://i.imgur.com/RRGcm9o.jpg', 'https://drive.google.com/uc?id=11EnE0BtPBBseM3W5n-bJRO04pwwYPDq4'),
(4, 'Bueyes jugando al tenis', 'Común', 0, 'https://i.imgur.com/jlYUZeF.jpg', 'https://drive.google.com/uc?id=1e_xUXjUh330-P7PkHAjHiHxCYMrTVMRv'),
(5, 'Caracol de Van Gogh bien fresco', 'Rara', 0, 'https://i.imgur.com/G4p08cv.jpg', 'https://drive.google.com/uc?id=1_DFoEOya1djntvYc1E3JWbAxwjlSnAiS'),
(6, 'Canguro del futuro', 'Especial', 0, 'https://i.imgur.com/qXtUP5P.jpg', 'https://drive.google.com/uc?id=1S1-Nu99UjNYEOJL6R7--Rz1DDQtNEeTO'),
(7, 'Canguro en el apocalipsis', 'Épica', 0, 'https://i.imgur.com/4S2lcQY.jpg', 'https://drive.google.com/uc?id=1WjhPsMPKxzDupJ7Dsi89unvDwrsOvKsH'),
(8, 'Reptiles futurísticos', 'Rara', 0, 'https://i.imgur.com/ZjkB3r8.jpg', 'https://drive.google.com/uc?id=14ESBrln6rmJp-ERnRiZEsa4N5XKkCFcz'),
(9, 'Erizo escribiendo en máquina de escribir', 'Común', 0, 'https://i.imgur.com/WVT01le.jpg', 'https://drive.google.com/uc?id=1YetX9nGb4Ku6uajS0hKsm6ZnNqGI-msw'),
(10, 'Unicornio y Cerdo preparados para la guerra', 'Especial', 0, 'https://i.imgur.com/zgvNJE3.jpg', 'https://drive.google.com/uc?id=1WndhqHyVZVZO5ekcCiPQ8D0C0G0zxnpY'),
(11, 'Tafalera', 'Común', 0, 'https://i.imgur.com/4IEB9AC.jpg', 'https://drive.google.com/uc?id=1oCeB6U_EhXJf2k-_KNI1a3E3VfQZlm4g'),
(12, 'Culebra de dientes amarillos y Cucaracha del semen negro', 'Legendaria', 0, 'https://i.imgur.com/QTbvKEj.jpg', 'https://drive.google.com/uc?id=1yqcQL9FSQPre2SaS3Xi3XpWiO9du-i3r'),
(13, 'Lemur mirando la ciudad por la noche', 'Épica', 0, 'https://i.imgur.com/fSw4Q9R.jpg', 'https://drive.google.com/uc?id=1uxVVvRofL_YnSrTLnaDOxZNefQaEbwGb'),
(14, 'El Hijo del Mata', 'Especial', 0, 'https://i.imgur.com/3kmYVQC.jpg', 'https://drive.google.com/uc?id=10RNPNztUydGCgNQjZsF1WEJoIJa6S3YV'),
(15, 'Lagarto difunde un mensaje', 'Legendaria', 0, 'https://i.imgur.com/sHooUtG.jpg', 'https://drive.google.com/uc?id=1kEPaCxjeUMGOv4hjbhHUugziqjQ4rpMw'),
(16, 'JBalvin acariciando a una ardilla en un barco', 'Especial', 0, 'https://i.imgur.com/ELYCJXr.jpg', 'https://drive.google.com/uc?id=1tZbWQFryjVEEdIiiy5fGg1Wob2eTd_QA');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `tienda`
--

CREATE TABLE `tienda` (
  `idMejora` int(255) NOT NULL,
  `nombreMejora` varchar(6000) NOT NULL,
  `precio` float NOT NULL,
  `tipoMejora` varchar(1000) NOT NULL,
  `cantidad` int(255) NOT NULL DEFAULT 0,
  `idUsuario` int(255) DEFAULT NULL,
  `linkImagenMejora` mediumtext NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `tienda`
--

INSERT INTO `tienda` (`idMejora`, `nombreMejora`, `precio`, `tipoMejora`, `cantidad`, `idUsuario`, `linkImagenMejora`) VALUES
(1, '+1 click', 100, 'Clicks', 0, NULL, 'https://i.imgur.com/v90qTWX.jpg');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `usuario`
--

CREATE TABLE `usuario` (
  `id` int(255) NOT NULL,
  `nombre` varchar(6500) NOT NULL,
  `correo` mediumtext NOT NULL,
  `password` varchar(6500) NOT NULL,
  `dinero` int(255) NOT NULL,
  `foto_perfil` blob NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Índices para tablas volcadas
--

--
-- Indices de la tabla `premios`
--
ALTER TABLE `premios`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `tienda`
--
ALTER TABLE `tienda`
  ADD PRIMARY KEY (`idMejora`),
  ADD KEY `idUsuario` (`idUsuario`);

--
-- Indices de la tabla `usuario`
--
ALTER TABLE `usuario`
  ADD PRIMARY KEY (`id`);

--
-- AUTO_INCREMENT de las tablas volcadas
--

--
-- AUTO_INCREMENT de la tabla `premios`
--
ALTER TABLE `premios`
  MODIFY `id` int(255) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=17;

--
-- AUTO_INCREMENT de la tabla `tienda`
--
ALTER TABLE `tienda`
  MODIFY `idMejora` int(255) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=31;

--
-- AUTO_INCREMENT de la tabla `usuario`
--
ALTER TABLE `usuario`
  MODIFY `id` int(255) NOT NULL AUTO_INCREMENT;

--
-- Restricciones para tablas volcadas
--

--
-- Filtros para la tabla `tienda`
--
ALTER TABLE `tienda`
  ADD CONSTRAINT `fk_tienda_usuario` FOREIGN KEY (`idUsuario`) REFERENCES `usuario` (`id`) ON DELETE CASCADE ON UPDATE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
