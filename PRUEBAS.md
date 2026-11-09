# Documento de Pruebas de Seguridad — Microcursos Univalle

## Resumen

Este documento describe las pruebas de seguridad realizadas sobre el portal
público y de suscriptores de **Microcursos Univalle**, verificando que las
restricciones de acceso y las validaciones del lado del servidor funcionen
correctamente.

---

## 1. Control de acceso por rol

### 1.1 Acceso al panel de administración

| # | Escenario | Pasos | Resultado esperado | OK |
|---|-----------|-------|-------------------|----|
| 1 | Usuario anónimo intenta acceder a /Cursos | Navegar a `/Cursos/Index` sin iniciar sesión | Redirección a /Cuenta/Ingresar | Sí |
| 2 | Suscriptor intenta acceder a /Cursos | Iniciar sesión como suscriptor, ir a `/Cursos/Index` | Redirección a /Cuenta/Denegado | Sí |
| 3 | Administrador accede a /Cursos | Iniciar sesión como admin, ir a `/Cursos/Index` | Muestra el listado de cursos | Sí |

### 1.2 Acceso a Mis Cursos

| # | Escenario | Pasos | Resultado esperado | OK |
|---|-----------|-------|-------------------|----|
| 4 | Usuario anónimo intenta acceder a /Inscripciones/MisCursos | Navegar sin sesión | Redirección a /Cuenta/Ingresar | Sí |
| 5 | Administrador intenta acceder a /Inscripciones/MisCursos | Iniciar sesión como admin | Redirección a /Cuenta/Denegado | Sí |
| 6 | Suscriptor accede a Mis Cursos | Iniciar sesión como suscriptor | Muestra los cursos inscritos | Sí |

---

## 2. Verificación de inscripción

### 2.1 Ver curso (solo inscritos)

| # | Escenario | Pasos | Resultado esperado | OK |
|---|-----------|-------|-------------------|----|
| 7 | Suscriptor no inscrito intenta ver un curso | Ir a `/Inscripciones/VerCurso/{id}` sin inscripción | TempData["Error"] + redirección a /Home/Detalle | Sí |
| 8 | Suscriptor inscrito ve el curso | Inscribirse, luego ir a VerCurso | Muestra las lecciones del curso | Sí |
| 9 | Curso no publicado | Admin despublica el curso, suscriptor intenta VerCurso | NotFound (404) | Sí |

### 2.2 Ver lección (solo inscritos)

| # | Escenario | Pasos | Resultado esperado | OK |
|---|-----------|-------|-------------------|----|
| 10 | Suscriptor no inscrito intenta ver una lección | Ir a `/Inscripciones/VerLeccion/{id}` sin inscripción | TempData["Error"] + redirección a /Home/Index | Sí |
| 11 | Suscriptor inscrito ve una lección | Estar inscrito, ir a VerLeccion | Muestra el contenido de la lección | Sí |
| 12 | Lección de curso no publicado (fix commit 10) | Admin despublica curso, suscriptor intenta VerLeccion | TempData["Error"] + redirección a MisCursos | Sí |

---

## 3. Prevención de inscripción duplicada

| # | Escenario | Pasos | Resultado esperado | OK |
|---|-----------|-------|-------------------|----|
| 13 | Suscriptor intenta inscribirse dos veces | Inscribirse, luego enviar POST de nuevo | TempData["Error"] "Ya estás inscrito" + redirección a VerCurso | Sí |
| 14 | Inscripción directa por índice único | Insertar registro duplicado en la BD | La BD rechaza por índice único (UsuarioId, CursoId) | Sí |
| 15 | Manipulación del formulario | Modificar el hidden input idCurso | Solo se inscribe al curso del id enviado | Sí |

---

## 4. Protección CSRF

| # | Escenario | Pasos | Resultado esperado | OK |
|---|-----------|-------|-------------------|----|
| 16 | POST sin anti-forgery token | Enviar POST a /Inscripciones/Inscribir sin token | Petición rechazada (400 Bad Request) | Sí |
| 17 | POST a MarcarCompletada sin token | Enviar POST sin token | Petición rechazada | Sí |
| 18 | Token de otra sesión | Usar token anti-forgery de otra sesión | Petición rechazada | Sí |

---

## 5. Validación de curso publicado

| # | Escenario | Pasos | Resultado esperado | OK |
|---|-----------|-------|-------------------|----|
| 19 | Catálogo solo muestra publicados | Crear curso no publicado, ir a /Home/Index | El curso no aparece en el catálogo | Sí |
| 20 | Detalle de curso no publicado | Ir a /Home/Detalle/{id} de curso no publicado | NotFound (404) | Sí |
| 21 | Inscripción a curso no publicado | POST /Inscripciones/Inscribir con id no publicado | TempData["Error"] "El curso no está disponible" | Sí |
| 22 | VerLeccion de curso no publicado | Admin despublica curso, suscriptor intenta VerLeccion | TempData["Error"] + redirección a MisCursos | Sí |

---

## 6. Marcar lección como completada

| # | Escenario | Pasos | Resultado esperado | OK |
|---|-----------|-------|-------------------|----|
| 23 | Marcar lección ajena | Modificar idInscripcion en el POST | Forbid (403) | Sí |
| 24 | Marcar dos veces la misma lección | Marcar completada, luego enviar POST de nuevo | No duplica el registro | Sí |
| 25 | Progreso refleja lecciones completadas | Marcar varias lecciones, ir a MiProgreso | El porcentaje y conteo son correctos | Sí |

---

## Credenciales de prueba

| Rol | Correo | Contraseña |
|-----|--------|------------|
| Administrador | admin@microcursosunivalle.edu.co | Admin@123456 |
| Suscriptor | (registrado vía /Cuenta/Registrar) | (contraseña del registro) |

---

## Conclusión

Todas las pruebas de seguridad pasaron correctamente. Las validaciones del
lado del servidor (verificación de inscripción, control de roles, prevención
de duplicados, protección CSRF y validación de curso publicado) funcionan como
se espera. El fix del commit 10 cierra el hueco de seguridad donde un
suscriptor podía ver lecciones de un curso despublicado.
