# AGRO COMPILER
by Aglahir Jimenez and Roberto Ramírez

## :beginner: Introducción


## Tipos de datos

El lenguaje AGRO consta de los siguientes tipos de datos primitivos
|Tipo |Descripción|Ejemplo|
|-----|--------|-------|
|int  |Valor númerico sin puntos decimales     |0, 1, 2, 3,...
|float|Valor númerio con puntos decimales      |0.5, 1.1, 4.5, 1.0...
|char|Cualquier letra rodeada por comillas simples| 'c', 'a', ...

## Estructura del Programa

El programa consta de una parte de declaraciones, las cuales pueden ser **variables, funciones o clases** y despues, el contexto de `main`.

Ejemplo de programa básico:

```
int a,b,c,d;

class Math{
    + int a;
    - int b;
}

void test(){
    int k;
    k = 444 + 1;
    print("El valor de k es: ", k);
}

main {
    float g;
    b = 15;
    c = 1;
    d = 444;
    g = 3.14;
    print("El valor de g es: ", g);
}
```

## Contextos en AGRO

### Globales
Las variables globales se escriben fuera del contexto de `main`, con la sintaxis básica en fomra:
&nbsp;&nbsp;&nbsp; *tipo = literal;*
```
    int i,j;
    float f;
    char c;
```

### Clases
Cada definición de clase debe tener la estructura: **class** + `nombre de la clase` seguido de llaves, y cada declaración dentro de la clase debe tener el indicador de accesibilidad del atributo o método antes de la declaración deseada.
&nbsp;&nbsp;&nbsp; *class = nombreClase {*
&nbsp;&nbsp;&nbsp; *...*
&nbsp;&nbsp;&nbsp; *...*
&nbsp;&nbsp;&nbsp; *}*

```
class Math{
    + int a;
    - int b;
}

class MathSon : Math{
    + int c;
    
    + void printC(){
        print(c, "\n");
    }
    
    + int getc(int ok){
        print("a: ", a, "\n");
        print("ok: ", ok, "\n");
        return c;
    }

    + void printA(){
        int x;
        print(a, "\n");
    }
}
```

### Funciones
Cada definición de funcion debe tener la estructura: **tipo** + `nombre de la función` seguido de paréntesis que pueden contener los parámetros necesarios de la función y llaves.
Recordando que las funciones pueden retornar cualquier tipo primitivo de datos o no regresar nada si se declaran como tipo void.
&nbsp;&nbsp;&nbsp; *tipoFunc = nombreFunc() {*
&nbsp;&nbsp;&nbsp; *...*
&nbsp;&nbsp;&nbsp; *}*

```
void setC(int toAssign){
    c = toAssign;
}

int getC() {
    return c;
}

```

## Estatutos en AGRO

### Asignación
En **AGRO** contamos con 3 tipos de asignaciones:
`Asignación normal` : Consta de simplemente una variable objetivo y el valor que se le quiera asignar, con la siguiente sintaxis:
* variable objetivo = valor a asignar;*
```
a = (b+4)/c-1;
```

`Asignación corta`: Consta de la variable objetivo y un símbolo de asignación corta, los cuales pueden ser: ** +=, -=, *=. /= ** y el valor que se ejecutará con la operación especificada y se asignará a la variable objetivo, ejemplo:
```
a *= b;
%% equivalente a: a = a*b;
```

`Paso (step)`: Consta solamente de la variable objetivo y un símbolo de step, que son: ** ++, -- **. Lo que simplemente sumará o restará 1 de la variable dada y la asignará a la misma variable.
```
a++;
%% equivalente a: a = a+1;
```

### Input / Output


### Condiciones

### Ciclos (While / For)

### Llamadas (Funciones / Métodos)