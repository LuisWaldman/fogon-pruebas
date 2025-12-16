Feature: Buscar canciones
    El fogon permite buscar canciones por autor y por nombre
    pero tambien por caracteristicas tipicas como la escala, el tempo o la cantidad de acordes.

    Scenario: Busca paloma de calamaro
        Given Usuario va al fogon
        When busca "flaca calamaro"
        Then aparecen resultados relacionados con "flaca calamaro"