
# Clase LectorAdif 

Clase escrita en C# (.net) que nos permite leer y tratar archivos ADIF (.adi) así como su posterior guardado. Este formato de archivos es muy utilizado en libros de guardia (LOG) de radioaficionados.


----------


## Constructores 

**1) Adif(String)** Inicializa una nueva instancia de la clase *Adif*.

- String Patch - Cadena "string" de la ruta completa del archivo .adi que se va a abrir.  
	
**2) Adif(String, Boolean)** Inicializa una nueva instancia de la clase *Adif*, con la opción de leer la cabecera (si existe) de los campos del ADIF.

- string Patch - Cadena "string" de la ruta completa del archivo .adi que se va a abrir. 
- bool cab - En caso de haber cabecera y el valor es 'true', adjunta al diccionario todos los campos ahunque no existan en el QSO. Si el valor es 'false' solo adjunta los campos encontrados en el QSO.

**3) Adif(String, Boolean, String)** Inicializa una nueva instancia de la clase *Adif*, con la opción de leer la cabecera (si existe) de los campos del ADIF. Permite especificar la cadena de inicio de los campos de la cabecera.

- string Patch - Cadena "string" de la ruta completa del archivo .adi que se va a abrir. 
- bool cab - En caso de haber cabecera y el valor es 'true', adjunta al diccionario todos los campos ahunque no existan en el QSO. Si el valor es 'false' solo adjunta los campos encontrados en el QSO.
- string Ini_cabecera - Cambia la cadena de inicio de cabecera. Por defecto es 'Exported Fields :'


----------


## Propiedades 

**1) CabeceraEncontrada** - Tipo Bool - Indica si encontró una cabecera válida.

- *True:* En la propiedad 'campos' estarán incluidos todos los campos de la cabecera.
- *False:* En la propiedad 'campos' se aparecerán los campos encontrados en los QSO's.

**2) Campos** - Tipo string[] - Retorna el listado de Campos obtenidos en una matriz de tipo string.

**3) CamposNoCabecera** - Tipo int - Indica el número de nuevos campos.

- Si el archivo contiene cabecera (CabeceraEncontrada = true), éste resultado indica los campos encontrados que NO estaban incluidos en la cabecera.
- Si el archivo contiene cabecera (CabeceraEncontrada = false), éste resultado indica el nº total de los campos encontrados.

**4) QSO** - Tipo List<Dictionary<string, string>> - Retorna una lista de diccionarios con los QSO's obtenidos.

**5) CurrentPatch** - Tipo string - Retorna la ruta actual (Patch) del archivo.

**6) FechaHora** - Tipo string - Retorna una cadena con la fecha y hora actuales en formato ADIF.


----------

## Métodos

**1) Insertar\_Modificar\_Campo(int, string, string)** -  Inserta un nuevo campo en un QSO o lo modifica si éste ya existe.

- int n\_QSO - Número del QSO a modificar
- string campo - Nombre del campo a incluir o modificar.
- string valor - Dato que se va a introducir en el campo nuevo o seleccionado.

**2) Guardar(Boolean)** - Guarda el archivo actual en la ruta actual.

- bool CabeceraCampos - Indicamos si queremos guardar la información de los campos en la cabecera.
	- True - Se guardará la información de campos.
	- False - No será guardada.

**3) Guardar(Boolean, String[])** - Guarda el archivo actual en la ruta actual con la opción de guardar los campos seleccionados.

- bool CabeceraCampos - Indicamos si queremos guardar la información de los campos en la cabecera.
	- True - Se guardará la información de campos.
	- False - No será guardada.
- string[] SelCampos - Matriz de string's conteniendo los nombres de los campos que se desean guardar.

**4) GuardarComo(String, Boolean)** - Guarda el archivo actual en la ruta actual.

- string NewPatch - Ruta completa donde se va a guardar el archivo.

- bool CabeceraCampos - Indicamos si queremos guardar la información de los campos en la cabecera.
	- True - Se guardará la información de campos.
	- False - No será guardada.

**5) GuardarComo(String, Boolean, String[])** - Guarda el archivo actual en la ruta actual con la opción de guardar los campos seleccionados.

- string NewPatch - Ruta completa donde se va a guardar el archivo.


- bool CabeceraCampos - Indicamos si queremos guardar la información de los campos en la cabecera.
	- True - Se guardará la información de campos.
	- False - No será guardada.
	

- string[] SelCampos - Matriz de string's conteniendo los nombres de los campos que se desean guardar.
