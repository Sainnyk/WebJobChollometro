using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using WebJobChollometro.Data;
using WebJobChollometro.Models;

namespace WebJobChollometro.Repositories
{
    public class RepositoryChollos
    {
        private ChollometroContext context;
        public RepositoryChollos(ChollometroContext context)
        {
            this.context = context;
        }


        private int GetMaxIdChollo()
        {
            if (this.context.Chollos.Count() == 0)
            {
                return 1;
            }
            else
            {
                //Recuperamos el maximo ID con lambda
                int maximo = this.context.Chollos.Max( x => x.IdChollo) + 1;
                return maximo;
            }
        }

        //Método para leer datos de Chollometro y convertirlos en una List<Chollo> -> Service
        private List<Chollo> GetChollosWeb()
        {
            string url = "https://www.chollometro.com/rss";
            //Lo tenemos que recuperar de otra forma porque tiene restricciones, debemos hacerpensar a la pagina que accedemos de un explorador web
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            request.Accept = @"text/html application/xhtml+xml, *,*";

            request.Referer = @"chollometro.com";

            request.Headers.Add("Accept-Language", "es-ES");

            request.UserAgent = @"Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; Trident/6.0)";

            request.Host = @"www.chollometro.com";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            //Lo que nos da es un Stream, Debemos convertir dicho flujo en un String de XML
            string xmlData = "";
            using (var stream = new StreamReader(response.GetResponseStream()))
            {
                xmlData = stream.ReadToEnd();
            }
            //Mediante LINQ TO XML recorremos los datos Xml recibidos
            XDocument document = XDocument.Parse(xmlData);

            var consulta = from datos in document.Descendants("item")
                           select datos;
            List<Chollo> chollos = new List<Chollo>();
            int idChollo = this.GetMaxIdChollo();
            //Recorremos los chollos
            foreach(var item in consulta)
            {
                Chollo chollo = new Chollo();
                chollo.IdChollo = idChollo;
                chollo.Titulo = item.Element("title").Value;
                chollo.Descripcion = item.Element("description").Value;
                chollo.Link = item.Element("link").Value;
                chollo.Fecha = DateTime.Now;
                //Incrementamos el ID de cada chollo
                idChollo++;
                chollos.Add(chollo);
            }
            return chollos;
        }

        //Metodo para insertar chollos en nuestra BBDD -> Repository
        public void PopulateChollos()
        {
            //Recuperamos los chollos de la web
            List<Chollo> chollos = this.GetChollosWeb();
            //Recorremos todos los chollos e insertamos en el context
            foreach(Chollo chollo in chollos)
            {
                this.context.Chollos.Add(chollo);
            }
            this.context.SaveChanges();
        }

    }
}
