Feature: Sesiones colaborativas
    La principal particularidad del fogon es la de mantener sesiones compartidas
    El estado compartido es la cancion, la lista de canciones y los roles de cada integrante 
    en la sesion (o fogon) 

    Scenario: Dos usuarios se conectan
        Given "Usuario1" accede a la aplicacion
        And "Usuario2" accede a la aplicacion
        And "Usuario2" inicia un fogon
        And "Usuario2" carga la cancion "adios nonino"
	    When "Usuario1" se une al fogon de "Usuario2"
	Then "Usuario1" ve la cancion "adios nonino" en reproduccion