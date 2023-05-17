# GitHub Action CI/CD microservicios

## Introduccion:

Este proyecto tiene la finalidad de hacer un proceso CI/CD utilizando GitHub Action en un entorno cloud de Azure. Para ello se dispone en este repositorio el codigo fuente de una applicacion y un cliente .NET y una base de datos Mongo. Estos seran microservicios que utilizaremos para lograr el deployment en AKS utilizando un registro de imagenes ACR.  

### Arquitectura

El flujo de trabajo sera el siguiente: 

![arq](img/arq.png)

Siguiendo estrategias de [Gitflow][flow] dispondemos de una rama _develop_ en la que tiene que ser utilizada para trabajar el codigo fuente. Dejando la rama _main_ como rama productiva. 


[flow]: https://www.atlassian.com/es/git/tutorials/comparing-workflows/gitflow-workflow

### API 

Dentro del directorio ***/src*** se encuntra los proyectos que utilizaremos. 

Para nuestra API tendremos una aplicacion en .NET que se vincula con una base de datos Mongo. Esta api seteara informacion de productos a la base de datos. Como tambien dispone de un metodo GET en el controlador para recuperar este listado. 

Dentro de este directorio se encuntra el Dockerfile necesario para contruir nuestra imagen para el contenedor que desplegaremos luego. 



### Client

Dentro del directorio ***/src*** se encuntra los proyectos que utilizaremos. 
Comenzaremos desarrollando Asp.Net MVC para crear un cliente que se conecte a nuestra API y pueda consumir la informacion a travez del endpoint expuesto y obtener asi los datos ya mencionados. 

Dentro de este directorio se encuntra el Dockerfile necesario para contruir nuestra imagen para el contenedor que desplegaremos luego. 


### Running LOCAL

Imaginemos un escenario posible, somos parte del equipo de desarrolladores e hicimos nuestro cambios. Para poder levantar estos servicios de manera local en la raiz de repositorio disponemos de los file _docker-compose.yml_ y _docker-compose.override.yml_.  De esta manera haciendo los siguientes comandos haremos un build de cada servicios utilizando los Dockerfile que disponemos y como resultado tendremos nuestra imagen de la API y el Cliente. En el caso de MongoDB no es necesario disponer de un dockerfile ya que usara el repositorio de Oficial.

Para compilar y crear nuestra imagenes de contenedores necesarias ejecutamos el comando _build_. Y para levantar nuestros servicios hacemos el comando _up_:

```bash
 docker-compose  -f .\docker-compose.yml -f .\docker-compose.override.yml build
 docker-compose  -f .\docker-compose.yml -f .\docker-compose.override.yml up -d
```

Algunos detalles como variables de entorno se encuentran en el file _docker-compose.override.yml_ hay que tener en cuenta por ejemplo la conexion a la base de datos. El punto de conexion sera el nombre de nuestro contenedor ejecutando. Otros detalles son los mapeo de puertos desde el entorno local al port del contenedor, como tambien los volumenes creados. 


Una vez corriendo nuestro contenedores podemos hacer nuestras pruebas y cambios en un entorno local. 
Ejecutando el siguiente comandando veremos nuestros contendores ejecutando:
```bash
 docker ps -a
```
![client1](img/client1.png)
![api1](img/api1.png)

## CI 

A esta altura podemos crear un pipeline de integracion continua con GitHub Actions. Para ello volviendo a arquitectura propuesta necesitamos disponer de un ACR en Azure. Con este repositorio de imagenes disponible y privado para nosotros, el pipeline se encargara de hacer un Build de la API y del Client de su respectivo Dockerfile y con la imagen creada hacer un push al repositorio ACR.
No es necesario enviar la imagen de mongo ya que no usamos un dockerfile. La imagen de MongoDB se encuentra disponible de su DockerHub Oficial. 
![acr](img/ACR.png)

En la seccion de Access Key de nuestro ACR habilataremos el acceso Admin y tendremos informacion como ***login server, username, password*** necesario para crear nuestro pipeline. 

En la seccion Configuracion->Secrets->Action de nuestro repositorio configuraremos _Repository Secrets_ 

Definimermos los siguientes secretos con el valor sacado de nuestro ACR en el portal: 

- ACR_PASSWORD
- ACR_USERNAME  

Dentro de la carpeta ***/.github/workflows***  tenemos disponible nuestro pipelie CI que se encargara de hacer de construir nuestra imagen y pushearla a nuestro repositorio en Azure.
En este caso, siguiendo la logica del pipeline y el flujo de trabajo que buscamos, se ejecutara cuando haya un push o pull request a la rama develop. Generando una image con un tag con el _github.sha_ de nuestro commit. Esto se hace de esta manera para tener una imagen creada en base a la version actual del commit en la rama de desarrollo.  Caso contrario sera cuando se haga un push o pull request a nuestra rama main productiva donde el pipeline sera otro que se vera mas adelante.