using EmotionPlatzi.Web.Models;
using EmotionPlatzi.Web.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace EmotionPlatzi.Web.Controllers
{
    public class EmoUploaderController : Controller
    {
        string serverFolderPath;
        string key;
        string emotionRoot;
        EmotionHelper emoHelper;
        EmotionPlatziWebContext db = new EmotionPlatziWebContext();

        public EmoUploaderController()
        {
            serverFolderPath = ConfigurationManager.AppSettings["UPLOAD_DIR"];
            key = ConfigurationManager.AppSettings["EMOTION_KEY"];
            emotionRoot = ConfigurationManager.AppSettings["EMOTION_ROOT"];
            emoHelper = new EmotionHelper(key,emotionRoot);
        }
        
        // GET: EmoUploader
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Index(HttpPostedFileBase file)
        {
            // file?: pregunta si es nulo antes de realizar la validacion
            if (file?.ContentLength > 0)
            {
                // Generamos un nombre aleatorio para el archivo
                var pictureName = Guid.NewGuid().ToString();
                pictureName += Path.GetExtension(file.FileName);

                // concatenamos la ruta con el nombre
                var route = Server.MapPath(serverFolderPath);
                route = route + "/" + pictureName;

                // lo guardamos en disco
                file.SaveAs(route);

                var emoPicture = await emoHelper.DetectAndExtractFacesAsync(file.InputStream);
                emoPicture.Name = file.FileName;
                emoPicture.Path = $"{serverFolderPath}/{pictureName}";


                db.EmoPictures.Add(emoPicture);
                await db.SaveChangesAsync();

                return RedirectToAction("Details", "EmoPictures", new { Id = emoPicture.Id });
            }
            return View();
        }
    }
}