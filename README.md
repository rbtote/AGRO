# AGRO COMPILER
by Aglahir Jimenez and Roberto Ramírez

## :beginner: Introducción


## :memo: Tipos de datos

El lenguaje AGRO consta de los siguientes tipos de datos primitivos
|Tipo |Descripción|Ejemplo|
|-----|--------|-------|
|int  |Valor númerico sin puntos decimales     |0, 1, 2, 3,...
|float|Valor númerio con puntos decimales      |0.5, 1.1, 4.5, 1.0...
|char|Cualquier letra rodeada por comillas simples| 'c', 'a', ...

## :clipboard: Estructura del Programa

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

## :bookmark_tabs: Contextos en AGRO

### Globales
Las variables globales se escriben fuera del contexto de `main`, con la sintaxis básica en forma:

&nbsp;&nbsp;&nbsp; *tipo = literal;*
```
    int i,j;
    float f;
    char c;
```

### Clases
Cada definición de clase debe tener la estructura: (**class** + `nombre de la clase`) seguido de llaves, y cada declaración dentro de la clase debe tener el indicador de accesibilidad del atributo o método antes de la declaración deseada.

De igual manera, se puede especificar que la clase tiene herencia simple de otra clase, con la estructura: (**class** + `nombre de la clase` **:** `nombre clase parent`). Lo que hará que la clase que estamos definiendo tenga los métodos y atributos de su clase parent.

&nbsp;&nbsp;&nbsp;&nbsp;*class = nombreClase : claseParent {  
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;...  
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;...  
&nbsp;&nbsp;&nbsp;&nbsp;}*

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
Cada definición de funcion debe tener la estructura: (**tipo** + `nombre de la función`) seguido de paréntesis que pueden contener los parámetros necesarios de la función y llaves.
Recordando que las funciones pueden retornar cualquier tipo primitivo de datos o no regresar nada si se declaran como tipo void.

&nbsp;&nbsp;&nbsp;&nbsp;*tipoFunc = nombreFunc() {  
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;...  
&nbsp;&nbsp;&nbsp;&nbsp;}*

```
void setC(int toAssign){
    c = toAssign;
}

int getC() {
    return c;
}

```

## :pencil2: Estatutos en AGRO

### Asignación
En **AGRO** contamos con 3 tipos de asignaciones:

`Asignación normal` : Consta de simplemente una variable objetivo y el valor que se le quiera asignar, como se muestra a continuación:
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

`Input`: Es el estatuto que se usa para poder ingresar valores tecleados por el usuario al código, tiene la sintaxis:
*input(variable);*
```
int a;
input(a);
```

`Output`: Este es el estatuo que usamos para poder imprimir en pantalla los valores de alguna variable interna, podemos imprimir varios valores si los separamos con **,**, de igual manera podemos imprimir letreros, especificados con palabras entre comillas, la sintaxis es:
*print(variable,variable ,...)*
```
int a;
print("El valor de a es: ",a);
```

### Condiciones
El estatuto de condicion se define de la siguiente manera:   
&nbsp;&nbsp;*if(condicion) {  
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;...  
&nbsp;&nbsp;}*
Y si es necesario, se puede incluir una parte de else:
&nbsp;&nbsp;*if(condicion) {  
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;...  
&nbsp;&nbsp;}*
&nbsp;&nbsp;*else {  
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;...  
&nbsp;&nbsp;}*
```
int out;
if (n < 2) {
        out = n;
    }
    else {
        out = (fibonacciRecursive(n - 1) + fibonacciRecursive(n - 2));
    }
```

### Ciclos (While / For)

### Llamadas (Funciones / Métodos)