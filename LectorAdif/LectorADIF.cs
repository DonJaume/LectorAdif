using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace LectorAdif
{
    [Serializable]
    public class Adif
    {
        [Serializable]
        public struct Str_Cabecera
        {
            public string Generated, ADIF_VER, CREATED_TIMESTAMP, PROGRAMID, PROGRAMVERSION;
        }

        //Variables accesibles
        private string[] ListaCampos;
        private List<Dictionary<string, string>> lista_QSO = new List<System.Collections.Generic.Dictionary<string, string>>();
        private bool Inc_Camp_vacios = false;
        private int N_Campos_No_cabecera = 0;
        private string patch;

        private Str_Cabecera cabecera = new Str_Cabecera();

        //Variables no accesibles
        private string I_cabecera = "Exported Fields :";
        private Dictionary<string, string> dic_vacio = new Dictionary<string, string>();
        List<string> n_campos = new List<string>();



        //******************************************* Constructores **************************************************

        /// <summary>
        /// Genera una lista de diccionarios que contiene los QSO's, lee la cabecera de campos si existe. Permite cambiar el inicio de cabecera.
        /// </summary>
        /// <param name="Patch">Ruta del archivo</param>
        /// <param name="Cab">En caso de haber cabecera y el valor es 'true', adjunta al diccionario todos los campos ahunque no existan en el QSO. Si el valor es 'false' solo adjunta los campos encontrados en el QSO</param>
        /// <param name="Ini_cabecera">Cambia la cadena de inicio de cabecera. Por defecto es 'Exported Fields :'</param>
        public Adif(string Patch, bool Cab, string Ini_cabecera)
        {
            patch = Patch;
            Inc_Camp_vacios = Cab;
            I_cabecera = Ini_cabecera;
            GenerarLista(Patch);
        }

        /// <summary>
        /// Genera una lista de diccionarios que contiene los QSO's, lee la cabecera de campos si existe.
        /// </summary>
        /// <param name="Patch">Ruta del archivo</param>  
        /// <param name="Cab">En caso de haber cabecera y el valor es 'true', adjunta al diccionario todos los campos ahunque no existan en el QSO. Si el valor es 'false' solo adjunta los campos encontrados en el QSO</param>
        public Adif(string Patch, bool Cab)
        {
            patch = Patch;
            Inc_Camp_vacios = Cab;
            GenerarLista(Patch);
        }

        /// <summary>
        /// Genera una lista de diccionarios que contiene los QSO's
        /// </summary>
        /// <param name="Patch">Ruta del archivo</param>
        public Adif(string Patch)
        {
            patch = Patch;
            Inc_Camp_vacios = false;
            GenerarLista(Patch);
        }


        //****************************************** METODOS PÚBLICOS ******************************


        /// <summary>
        /// Inserta un nuevo campo en un QSO o lo modifica si éste ya existe.
        /// </summary>
        /// <param name="n_QSO">Número de QSO</param>
        /// <param name="campo">Nombre del campo</param>
        /// <param name="valor">Valor del campo</param>
        public void Insertar_Modificar_Campo(int n_QSO, string campo, string valor)
        {
            if (!n_campos.Contains(campo))                                    //si el campo que se desea adjuntar no está en la lista de campos, lo adjuntamos
            {
                n_campos.Add(campo);                                          //incluimos el campo nuevo en la lista
                N_Campos_No_cabecera++;
                GeneraListaCampos();                                          //Se regenera la matriz del listado de campos incluyendo el campo nuevo
                lista_QSO[n_QSO].Add(campo, valor);                           //incluimos el rato en el campo del qso seleccionado
            }
            else
                lista_QSO[n_QSO][campo] = valor;

        }

        /// <summary>
        /// Indica si se encontró una cabecera válida.
        /// <para>Si el es True, en la variable 'campos' estarán incluidos todos los campos de la cabecera.</para>
        /// <para>Si es False, en la variable 'campos' se aparecerán los campos encontrados en los QSO's.</para>
        /// </summary>
        public bool CabeceraEncontrada
        {
            get { return Inc_Camp_vacios; }
        }

        /// <summary>
        /// Retorna el listado de Campos obtenidos.
        /// </summary>
        public string[] Campos
        {
            get { return ListaCampos; }
        }

        /// <summary>
        /// Indica el número de nuevos campos.
        /// <para>Si el archivo contíene cabecera (CabeceraEncontrada = true), éste resultado indica los campos encontrados que NO estaban incluidos en la cabecera.</para>
        /// <para>Si el archivo contíene cabecera (CabeceraEncontrada = false), éste resultado indica el nº total de los campos encontrados.</para>
        /// </summary>
        public int CamposNoCabecera
        {
            get { return N_Campos_No_cabecera; }
        }


        /// <summary>
        /// Retorna una lista de diccionarios con los QSO's obtenidos.
        /// </summary>
        public List<Dictionary<string, string>> QSO
        {
            get { return lista_QSO; }
            //set { lista_QSO = value; }
        }

        /// <summary>
        /// Retorna la dirección actual del archivo
        /// </summary>
        public string CurrentPatch
        {
            get { return patch; }
        }

        /// <summary>
        /// Obtiene o establece la información de cabecera.
        /// </summary>
        public Str_Cabecera Cabecera
        {
            get { return cabecera; }
            set { cabecera = value; }
        }

        /// <summary>
        /// Obtiene la hora actual en formato ADIF
        /// </summary>
        public string FechaHora
        {
            get
            {
                DateTime FH = DateTime.Now;
                return FH.ToString("yyyyMMdd HHmmss");
            }
        }

        //----metodos de guardado (Sobrecargados)----

            
        /// <summary>
        /// Guarda el archivo actual.
        /// </summary>
        /// <param name="CabeceraCampos">False: No guarda la información de campos en la cabecera.  True: Guarda la información de campos en la cabecera.</param>
        public void Guardar(bool CabeceraCampos)
        {
            SaveFile(CabeceraCampos, null);
        }

        /// <summary>
        /// Guarda el archivo actual.
        /// </summary>
        /// <param name="CabeceraCampos">False: No guarda la información de campos en la cabecera.  True: Guarda la información de campos en la cabecera.</param>
        /// <param name="SelCampos">Lista de los elementos que desea guardar</param>
        public void Guardar(bool CabeceraCampos, string[] SelCampos)
        {
            SaveFile(CabeceraCampos, SelCampos);
        }

        /// <summary>
        /// Guarda el archivo actual en una nueva ruta
        /// </summary>
        /// <param name="NewPatch">Ruta completa de guardado de archivo</param>
        /// <param name="CabeceraCampos">False: No guarda la información de campos en la cabecera.  True: Guarda la información de campos en la cabecera.</param>
        public void GuardarComo(string NewPatch, bool CabeceraCampos)
        {
            patch = NewPatch;
            SaveFile(CabeceraCampos, null);
        }

        /// <summary>
        /// Guarda el archivo actual en una nueva ruta
        /// </summary>
        /// <param name="NewPatch">Ruta completa de guardado de archivo</param>
        /// <param name="CabeceraCampos">False: No guarda la información de campos en la cabecera.  True: Guarda la información de campos en la cabecera.</param>
        /// <param name="SelCampos">Lista de los elementos que desea guardar</param>
        public void GuardarComo(string NewPatch, bool CabeceraCampos, string[] SelCampos)
        {
            patch = NewPatch;
            SaveFile(CabeceraCampos, SelCampos);
        }


        //****************** MÉTODOS PRIVADOS *********************


        //Metodo encargado de guardar el archivo segun los parametros enviados
        private void SaveFile(bool CabeceraCampos, string[] SelCampos)
        {
            StreamWriter esctibir = new StreamWriter(patch, false);     //creamos el objeto de guardado 

            string[] writeCamos;                    //
            bool SC = false;                        // Bool (bandera) que nos indicará si escribiremos todos los campos o solo los seleccionados en la matriz "SelCampos" recibida
            if (SelCampos != null) SC = true;       //si la cadena Selcampos NO es nula, solo escribiremos los campos que traiga la cadena

            //listado de campos a pasar
            //si SelCampos es nula, se escriben todos los campos que contiene el QSO,
            if (SC)
                writeCamos = SelCampos;
            else
                writeCamos = ListaCampos;

            //escribiendo cabecera
            if (cabecera.Generated != null) esctibir.WriteLine(cabecera.Generated + "\r\n");
            if (cabecera.ADIF_VER != null) esctibir.WriteLine("<ADIF_VER:" + cabecera.ADIF_VER.Length.ToString() + ">" + cabecera.ADIF_VER);
            if (cabecera.PROGRAMID != null) esctibir.WriteLine("<PROGRAMID:" + cabecera.PROGRAMID.Length.ToString() + ">" + cabecera.PROGRAMID);
            if (cabecera.PROGRAMVERSION != null) esctibir.WriteLine("<PROGRAMVERSION:" + cabecera.PROGRAMVERSION.Length.ToString() + ">" + cabecera.PROGRAMVERSION);
            if (cabecera.CREATED_TIMESTAMP != null) esctibir.WriteLine("<CREATED_TIMESTAMP:" + cabecera.CREATED_TIMESTAMP.Length.ToString() + ">" + cabecera.CREATED_TIMESTAMP);



            //escribiendo informacion de campos en cabecera (solo si ha sido indicado en el parametro recibido)
            if (CabeceraCampos)
            {
                esctibir.WriteLine("");
                esctibir.WriteLine("Exported Fields :");
                for (int i = 0; i < writeCamos.Length; i++)
                    esctibir.Write(writeCamos[i] + ",");
            }
            esctibir.WriteLine();
            esctibir.WriteLine("<EOH>");

            //escribiendo QSO's

            if (SC)     //escribimos solo campos seleccionados
            {
                foreach (Dictionary<string, string> s in lista_QSO)
                {
                    foreach (KeyValuePair<string, string> x in s)
                        if (writeCamos.Contains<string>(x.Key))
                            esctibir.Write("<{0}:{1}>{2}  ", x.Key, x.Value.Length, x.Value);

                    esctibir.Write("<EOR>");
                    esctibir.WriteLine();
                }
            }

            else        //escribimos todo
            {
                foreach (Dictionary<string, string> s in lista_QSO)
                {
                    foreach (KeyValuePair<string, string> x in s)
                        esctibir.Write("<{0}:{1}>{2}  ", x.Key, x.Value.Length, x.Value);

                    esctibir.Write("<EOR>");
                    esctibir.WriteLine();
                }
            }

            esctibir.Close();

        }


        //Metodo que se encarga de leer el archivo ADI y organizar los datos
        private void GenerarLista(string Patch)
        {
            //variables locales
            int p_temp = 0;
            string cabecera = "";


            StreamReader leer = new StreamReader(Patch);
            string lectura = leer.ReadToEnd();
            leer.Close();                                                     //leemos el archivo

            //-------------------------- ZONA DE BUSQUEDA DE CABECERA ------------------------------

            p_temp = lectura.IndexOf(I_cabecera, 0);                              //buscamos si hay cabecera
            if (p_temp > 0)                                                        //si el resultado es mayor que 0, tenemos cabecera
            {
                int puntero = p_temp + I_cabecera.Length;                         //guardamos la posición de inicio de cabecera

                p_temp = lectura.IndexOf("<EOH>", puntero);                       //Buscamos donde acaba la cabecera
                cabecera = lectura.Substring(puntero, p_temp - puntero).          //Extraemos la cabecera
                    Replace("\r\n", "").Replace(" ", "");                         //Sin espacios ni saltos de linea

                char[] seps = { ',' };                                            //Espliteamos la cabecera
                n_campos = cabecera.Split(seps).ToList<string>();

                //en le caso que queramos incluir los campos vacios en el QSO
                if (Inc_Camp_vacios) GenerarDiccionario();                         //generamos un diccionario con los campos iniciado con ""
            }
            else
                Inc_Camp_vacios = false;                                          //Si no se ha encontrado cabecera ponemos el flag en false;



            //-------------------------- ZONA DE BUSQUEDA DE QSO'S ------------------------------

            string[] seps2 = { "<EOR>" };                                         //separamos los QSO's en una matriz
            string[] QSOs = lectura.Split(seps2, StringSplitOptions.RemoveEmptyEntries);

            ObtenerCabecera(QSOs[0].Substring(0, QSOs[0].IndexOf("<EOH>")));
            QSOs[0] = QSOs[0].Remove(0, QSOs[0].IndexOf("<EOH>") + 5);              //en el primer registro, borrar todo hasta <EOH>

            for (int i = 0; i < QSOs.Length; i++)
                AnadirQSO(QSOs[i].Replace("\r\n", ""));                             //Enviamos el QSO para procesar, le quitamos saltos de linea

            GeneraListaCampos();                                                    //Una vez terminado de añadir QSO's a la lista, generamos la lista de campos
        }

        //lee y asigna los valores de la cabecera (ADIF_VER, PROGRAMID, PROGRAMVERSION, CREATED_TIMESTAMP)
        private void ObtenerCabecera(string DATOS)
        {
            cabecera.Generated = DATOS.Substring(0, DATOS.IndexOf('\r'));

            int PosAdifVer = DATOS.IndexOf("ADIF_VER", 0, DATOS.Length, StringComparison.CurrentCultureIgnoreCase);
            if (PosAdifVer > 0)
            {
                string AdifVer = DATOS.Substring(PosAdifVer, (DATOS.IndexOf('\r', PosAdifVer) - PosAdifVer)).Replace("\r\n", "");
                cabecera.ADIF_VER = AdifVer.Substring(AdifVer.LastIndexOf('>') + 1);
            }

            int PosID = DATOS.IndexOf("PROGRAMID", 0, DATOS.Length, StringComparison.CurrentCultureIgnoreCase);
            if (PosID > 0)
            {
                string ID = DATOS.Substring(PosID, (DATOS.IndexOf('\r', PosID) - PosID)).Replace("\r\n", "");
                cabecera.PROGRAMID = ID.Substring(ID.LastIndexOf('>') + 1);
            }

            int PosPrVer = DATOS.IndexOf("PROGRAMVERSION", 0, DATOS.Length, StringComparison.CurrentCultureIgnoreCase);
            if (PosPrVer > 0)
            {
                string PrVer = DATOS.Substring(PosPrVer, (DATOS.IndexOf('\r', PosPrVer) - PosPrVer)).Replace("\r\n", "");
                cabecera.PROGRAMVERSION = PrVer.Substring(PrVer.LastIndexOf('>') + 1);
            }

            int PosTime = DATOS.IndexOf("CREATED_TIMESTAMP", 0, DATOS.Length, StringComparison.CurrentCultureIgnoreCase);
            if (PosTime > 0)
            {
                string Time = DATOS.Substring(PosTime, (DATOS.IndexOf('\r', PosTime) - PosTime)).Replace("\r\n", "");
                cabecera.CREATED_TIMESTAMP = Time.Substring(Time.LastIndexOf('>') + 1);
            }
        }

        //Convierte la Lista de campos encontrados a la matriz pública
        private void GeneraListaCampos()
        {
            ListaCampos = n_campos.ToArray();
        }

        //Genera un diccionario vacio con todos los campos encontrados
        //Si la generacion del listado de QSO's seleccionamos que todos los campos sean representados, utiliza éste diccionario para incluir
        // en cada QSO, posteriormente se rellenan con los datos obtenidos
        private void GenerarDiccionario()
        {
            for (int i = 0; i < n_campos.Count; i++)
                dic_vacio.Add(n_campos[i], "");
        }

        //Paso previo para incluir el QSO en el listado
        //Segun la configuración seleccionada, crea el listado de un modo u otro
        private void AnadirQSO(string qso)
        {
            //correccion de vacios, se borrará
            qso = qso.Replace("  <", "<");

            if (Inc_Camp_vacios) A_QSO_con_Cabecera(qso);
            else A_QSO_SIN_Cabecera(qso);
        }

        //Cea el QSO cuando el fichero incluye cabecera y se solicita que se incluyan todos los campos
        private void A_QSO_con_Cabecera(string qso)
        {
            lista_QSO.Add(dic_vacio);                       //añadimos al listado el QSO con todos los campos en blanco

            string[] datos = MiSplit(qso, '<', '>');

            for (int i = 1; i < datos.Length; i += 2)
            {
                string clave = datos[i - 1].Remove(datos[i - 1].IndexOf(':'));         //clave de diccionario
                string dato = datos[i];                                               //dato             

                if (!n_campos.Contains(clave))                                     //Adjuntamos las claves encontradas que no estaban en la cabeceraa la lista nuevoscampos.
                {
                    n_campos.Add(clave);
                    N_Campos_No_cabecera++;
                }

                lista_QSO[lista_QSO.Count - 1][clave] = dato;
            }
        }

        //Cea el QSO cuando el fichero NO incluye cabecera o se solicita que NO se incluyan los campos que no tienen datos
        private void A_QSO_SIN_Cabecera(string qso)
        {
            Dictionary<string, string> Dic_temp = new Dictionary<string, string>();

            string[] datos = MiSplit(qso, '<', '>');                                //Obtenemos claves y datos en una matriz

            for (int i = 1; i < datos.Length; i += 2)
            {
                string clave = datos[i - 1].Remove(datos[i - 1].IndexOf(':'));         //clave de diccionario
                string dato = datos[i];                                               //dato

                Dic_temp.Add(clave, dato);                                             //los añadimos al diccionario temporal 

                if (!n_campos.Contains(clave))                                         //Adjuntamos las claves encontradas que no estaban en la cabeceraa la lista nuevoscampos.
                {
                    n_campos.Add(clave);
                    N_Campos_No_cabecera++;
                }
            }
            lista_QSO.Add(Dic_temp);                                                    //Añadimos el QSO (desglosado en diccionario) en la lista
        }

        //Split de la cadena, separa los datos de la cadena desde el caracter sep1 al sep2
        private string[] MiSplit(string cadena, char sep1, char sep2)
        {
            string subcadena = "";
            List<string> salida = new List<string>();

            bool SC1 = true;
            char sep = sep1;

            for (int i = 0; i < cadena.Length; i++)
            {
                if (cadena[i] == sep)
                {
                    if (subcadena != "") salida.Add(subcadena);
                    subcadena = "";
                    SC1 = !SC1;
                    sep = SC1 ? sep1 : sep2;

                }
                else subcadena += cadena[i];
            }

            return salida.ToArray();
        }
    }

}

