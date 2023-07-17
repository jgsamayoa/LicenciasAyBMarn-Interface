using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using SigeaciInterface.Models;
using System.Dynamic;
using System.Net.Http.Headers;
using System.Text;

namespace InterfaceBiaweb.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ApiConsumeController : ControllerBase
    {

        public static IWebHostEnvironment _env;

        public ApiConsumeController(IWebHostEnvironment env)
        {
            _env = env;
        }

        // Url local
        //string urlBase = "https://localhost:7291/";
        string urlBase = "https://localhost:44396/";

        // Url del servidor
        //string urlBase = "http://205.209.122.7:90/";



        /// <summary>
        /// Obtener listado de registros desde la api
        /// </summary>
        /// <query name="endpoint">contiene la url que se consume en el api</query>
        /// <returns>Json resonse</returns>


        [HttpGet("obtener")]
        [Produces("application/json")]


        public async Task<ExpandoObject> GetByEndPoint()
        {
            try
            {
                //https://makolyte.com/csharp-deserialize-json-to-dynamic-object/


                //!Capturar el token que viene en los headers
                string token = HttpContext.Request.Headers["Authorization"];

                //!Capturar la ruta que se va a consumir en el api, esta viene en la url como endpoint


                string endpoint = HttpContext.Request.Query["endpoint"].ToString();

                string result;

                using (HttpClient client = new HttpClient())
                {

                    //!Adjuntar el token a los headers que van hacia la api
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);


                    //client.DefaultRequestHeaders.Add("Authorization ", $"Bearer {token}");

                    //!Asignar la url principal que consume la api

                    client.BaseAddress = new System.Uri(urlBase);


                    //!Asginar la url final que se consume en base a la variable endpoint
                    string url = string.Format(endpoint);

                    //!Realizar la petición a la api
                    var response = await client.GetAsync(url);

                    result = response.Content.ReadAsStringAsync().Result;

                    //?Validar que se haya llegado al servicio y este de una respuesta
                    if (response != null)
                    {
                        //int statusCode = (int)response.StatusCode;

                        //!Convertir a json la respuesta que se recibe del api
                        dynamic config = JsonConvert.DeserializeObject<ExpandoObject>(result, new ExpandoObjectConverter());

                        return config;

                    }
                    //?Si no existe respuesta del servicio
                    else
                    {
                        string error = "{statusCode:401, message:'Ha ocurrido un error de conexión'}";
                        dynamic config = JsonConvert.DeserializeObject<ExpandoObject>(error, new ExpandoObjectConverter());
                        return config;

                    }
                }

            }
            catch (FormatException fex)
            {
                string message = fex.Message;
                string error = "{status :500, message:'Entro al catch del json format'}";
                dynamic config = JsonConvert.DeserializeObject<ExpandoObject>(error, new ExpandoObjectConverter());
                return config;
            }
            catch (JsonReaderException jex)
            {
                string message = jex.Message;
                string error = "{status:500, message:'Entro al catch del json'}";
                dynamic config = JsonConvert.DeserializeObject<ExpandoObject>(error, new ExpandoObjectConverter());
                return config;
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                string error = "{status:500, message:'Ha ocurrido un error'}";
                dynamic config = JsonConvert.DeserializeObject<ExpandoObject>(error, new ExpandoObjectConverter());
                return config;
            }

        }


        /// <summary>
        /// Obtener un item específico de registros desde la api
        /// </summary>
        /// <query name="endpoint">contiene la url que se consume en el api</query>
        /// <param name="id"></param>
        /// <returns>Json resonse</returns>

        [HttpGet("edit/{id}")]
        [Produces("application/json")]


        public async Task<ExpandoObject> Get(int id)
        {
            //!Obtener la url que se consume en la api
            // Se concatena el id que que viene en la url
            // Ejemplo apiconsume/edit/39?endpoint=api/Empresas/
            string endpoint = HttpContext.Request.Query["endpoint"].ToString() + id;

            try
            {

                //!Capturar el token que viene en los headers
                string token = HttpContext.Request.Headers["Authorization"];


                string result;
                //HttpClient client = new HttpClient();


                using (HttpClient client = new HttpClient())
                {

                    //!Adjuntar el token a los headers que van hacia la api
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    //!Asignar la url principal que consume la api
                    client.BaseAddress = new System.Uri(urlBase);

                    //!Asginar la url final que se consume en base a la variable endpoint
                    string url = string.Format(endpoint);

                    //!Realizar la petición a la api
                    var response = await client.GetAsync(url);

                    //?Validar que haya una respuesta del servicio
                    if (response != null)
                    {
                        int statusCode = (int)response.StatusCode;

                        result = response.Content.ReadAsStringAsync().Result;
                        dynamic config = JsonConvert.DeserializeObject<ExpandoObject>(result, new ExpandoObjectConverter());
                        return config;

                    }
                    //?Si no existe respuesta del servicio
                    else
                    {
                        string error = "{statusCode:401, message:'Ha ocurrido un error de conexión'}";
                        dynamic config = JsonConvert.DeserializeObject<ExpandoObject>(error, new ExpandoObjectConverter());
                        return config;

                    }

                }


            }
            catch (FormatException fex)
            {
                string message = fex.Message;
                string error = "{status :500, message:'Ha ocurrido un error interno, ponte en contacto con un administrador del sistema'}";
                dynamic config = JsonConvert.DeserializeObject<ExpandoObject>(error, new ExpandoObjectConverter());
                return config;
            }
            catch (JsonReaderException jex)
            {
                string message = jex.Message;
                string error = "{status:500, message:'Ha ocurrido un error interno, ponte en contacto con un administrador del sistema'}";
                dynamic config = JsonConvert.DeserializeObject<ExpandoObject>(error, new ExpandoObjectConverter());
                return config;
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                string error = "{status:500, message:'Ha ocurrido un error'}";
                dynamic config = JsonConvert.DeserializeObject<ExpandoObject>(error, new ExpandoObjectConverter());
                return config;
            }


        }

        /// <summary>
        /// Realizar el registro de un registro
        /// </summary>
        /// <query name="endpoint">contiene la url que se consume en el api</query>
        /// <param>Recibe un objeto como string</param>
        /// <returns>Json response</returns>


        [HttpPost("create")]
        [Produces("application/json")]

        //[Consumes("application/json")]
        public async Task<ExpandoObject> Post([FromBody] DynamicData data)
        {


            //!Convertir el DynamicData (Modelo) que viene como parametro a json
            string json2 = JsonConvert.SerializeObject(data, Formatting.Indented);

            //!Crear un objeto dynamic a partir del json creado
            dynamic itemData = JObject.Parse(json2);

            //!Obtener el atributo "Data" que es el que encapsula toda la información del item enviado.
            string jsonString = JsonConvert.SerializeObject(itemData.Data);

            //!Capturar la ruta que se va a consumir en el api, esta viene en la url como endpoint
            string endpoint = HttpContext.Request.Query["endpoint"].ToString();


            try
            {


                //!Capturar el token que viene en los headers
                string token = HttpContext.Request.Headers["Authorization"];


                using (HttpClient client = new HttpClient())
                {


                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //!Adjuntar el token a los headers que van hacia la api
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);


                    //!Asignar la url primaria que consume la api
                    client.BaseAddress = new System.Uri(urlBase);

                    //!Asginar la url final que se consume en base a la variable endpoint
                    string url = string.Format(endpoint);

                    //StringContent content = new StringContent(JsonConvert.SerializeObject(jsonString), Encoding.UTF8, "application/json");
                    var content = new StringContent(jsonString.ToString(), Encoding.UTF8, "application/json");

                    //!Realizar la petición a la api
                    var response = await client.PostAsync(url, content);

                    string apiResponse = await response.Content.ReadAsStringAsync();

                    //?Validar que se haya llegado al servicio y este de una respuesta
                    if (response != null)
                    {
                        //int statusCode = (int)response.StatusCode;

                        //!Convertir a json la respuesta que se recibe del api
                        dynamic config = JsonConvert.DeserializeObject<ExpandoObject>(apiResponse, new ExpandoObjectConverter());

                        return config;

                    }
                    //?Si no existe respuesta del servicio
                    else
                    {
                        string error = "{statusCode:401, message:'Ha ocurrido un error de conexión'}";
                        dynamic config = JsonConvert.DeserializeObject<ExpandoObject>(error, new ExpandoObjectConverter());
                        return config;

                    }

                }

            }
            catch (FormatException fex)
            {
                string message = fex.Message;
                string error = "{status :500, message:'Ha ocurrido un error en el servidor, por favor, ponte en contacto con un administrador del sistema.'}";
                dynamic config = JsonConvert.DeserializeObject<ExpandoObject>(error, new ExpandoObjectConverter());
                return config;
            }
            catch (JsonReaderException jex)
            {
                string message = jex.Message;
                string error = "{status:500, message:'Ha ocurrido un error interno, por favor, ponte en contacto con un administrador del sistema.'}";
                dynamic config = JsonConvert.DeserializeObject<ExpandoObject>(error, new ExpandoObjectConverter());
                return config;
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                string error = "{status:500, message:'Ha ocurrido un error interno, por favor, ponte en contacto con un administrador del sistema.'}";
                dynamic config = JsonConvert.DeserializeObject<ExpandoObject>(error, new ExpandoObjectConverter());
                return config;
            }

        }



        /// <summary>
        /// Realizar actualizacion de un item
        /// </summary>
        /// <query name="endpoint">contiene la url que se consume en el api</query>
        /// <param Data="json">Recibe un objeto como string</param>
        /// <returns>Json response</returns>


        [HttpPost("update/{id}")]
        [Produces("application/json")]


        public async Task<ExpandoObject> Put([FromBody] DynamicData data, int id)
        {
            //!Convertir el DynamicData (Modelo) que viene como parametro a json
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);

            //!Crear un objeto dynamic a partir del json creado
            dynamic itemData = JObject.Parse(json);

            //!Obtener el atributo "Data" que es el que encapsula toda la información del item enviado.
            string jsonString = JsonConvert.SerializeObject(itemData.Data);


            //!Obtener el endpoint que viene en la url, a este se le concatena el id que viene en la url
            string endpoint = HttpContext.Request.Query["endpoint"].ToString() + id;

            try
            {


                //!Capturar el token que viene en los headers
                string token = HttpContext.Request.Headers["Authorization"];


                using (HttpClient client = new HttpClient())
                {

                    //!Adjuntar el token a los headers que van hacia la api
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);


                    //!Asignar la url primaria que consume la api
                    client.BaseAddress = new System.Uri(urlBase);

                    //!Crear la ur a partir del endpoint recibido
                    string url = string.Format(endpoint);

                    //!Crear el content para enviarlo en la solicitud, este crea los headers de la solicitud
                    var content = new StringContent(jsonString.ToString(), Encoding.UTF8, "application/json");

                    //!Realizar la solicitud a la api
                    var response = await client.PutAsync(url, content);

                    //!Obtener la resupuesta de la solicitud
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    //ApiResponse rs = JsonConvert.DeserializeObject<ApiResponse>(apiResponse, new ExpandoObjectConverter());

                    //?Validar que se haya llegado al servicio y este de una respuesta
                    if (response != null)
                    {
                        //int statusCode = (int)response.StatusCode;

                        //!Convertir a json la respuesta que se recibe del api
                        dynamic config = JsonConvert.DeserializeObject<ExpandoObject>(apiResponse, new ExpandoObjectConverter());

                        return config;

                    }
                    //?Si no existe respuesta del servicio
                    else
                    {
                        string error = "{statusCode:401, message:'Ha ocurrido un error de conexión'}";
                        dynamic config = JsonConvert.DeserializeObject<ExpandoObject>(error, new ExpandoObjectConverter());
                        return config;

                    }
                }
            }
            catch (FormatException fex)
            {
                string message = fex.Message;
                string error = "{status :500, message:'Ha ocurrido un error interno, ponte en contacto con un administrador del sistema'}";
                dynamic config = JsonConvert.DeserializeObject<ExpandoObject>(error, new ExpandoObjectConverter());
                return config;
            }
            catch (JsonReaderException jex)
            {
                string message = jex.Message;
                string error = "{status:500, message:'Ha ocurrido un error interno, ponte en contacto con un administrador del sistema'}";
                dynamic config = JsonConvert.DeserializeObject<ExpandoObject>(error, new ExpandoObjectConverter());
                return config;
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                string error = "{status:500, message:'Ha ocurrido un error'}";
                dynamic config = JsonConvert.DeserializeObject<ExpandoObject>(error, new ExpandoObjectConverter());
                return config;
            }




        }

        /// <summary>
        /// Almacenar documentos
        /// </summary>
        /// <param name="s"></param>
        /// <returns>json</returns>
        /// 
        [HttpPost("document")]
        [Produces("application/json")]
        public async Task<ExpandoObject> RegistrarDocumento([FromForm] Documento archivos)
        {
            string message = "";



            if (archivos.Files.Length > 0)
            {


                var random = new Random();
                int randomnumber = random.Next();

                string[] CarEspeciales = new string[] { "!", "#", "$", "%", "&", "'", "(", ")", "*", "+", ",", "-", ".", "/", ":", ";", "<", "=", ">", "?", " ", "ó", "ú", "í", "é", "á", "Ú", "Ó", "Í", "É", "Á" };

                for (int ca = 0; ca < CarEspeciales.Count(); ca++)
                {

                    while (archivos.path.Contains(CarEspeciales[ca]))
                    {
                        archivos.path = archivos.path.Replace(CarEspeciales[ca], "_");
                    }
                }
                //_env.WebRootPath
                string ruta = Path.Combine(_env.ContentRootPath, "DocumentosBiaWeb" + archivos.path);

                string rutaWeb = Path.Combine(_env.ContentRootPath, "documentos" + archivos.path);

                string nombreArchivo = randomnumber.ToString() + Path.GetExtension(archivos.Files.FileName);

                string rutaRegistro = rutaWeb + nombreArchivo;

                //Verificar si existe la carpeta con el nombre de la empresa
                if (!Directory.Exists(ruta))
                {
                    Directory.CreateDirectory(ruta);
                }

                //Crear la ruta con el archivo 
                string rutaArchivo = ruta + "\\" + nombreArchivo;
                using (FileStream fileStream = System.IO.File.Create(rutaArchivo))
                {
                    archivos.Files.CopyTo(fileStream);
                    fileStream.Flush();


                    FileResponse fResponse = new FileResponse();
                    fResponse.status = 200;
                    fResponse.mensaje = "Se ha cargado el archivo con éxito";
                    fResponse.pathArchivo = "\\documentos" + archivos.path + nombreArchivo;
                    fResponse.extensionArchivo = Path.GetExtension(archivos.Files.FileName);
                    message = JsonConvert.SerializeObject(fResponse);
                    //message = String.Format(json);

                }
            }
            else
            {

                message = "{status:404, message:'No se ha seleccionado ningún archivo.'}";
            }

            dynamic config = JsonConvert.DeserializeObject<ExpandoObject>(message, new ExpandoObjectConverter());

            return config;

        }
    }
}
