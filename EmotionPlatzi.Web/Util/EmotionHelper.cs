using EmotionPlatzi.Web.Models;
using Microsoft.ProjectOxford.Common.Contract;
using Microsoft.ProjectOxford.Emotion;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Web;
using System.Reflection;
using System.Threading.Tasks;

namespace EmotionPlatzi.Web.Util
{
    public class EmotionHelper
    {
        public EmotionServiceClient emoClient;

        public EmotionHelper(string subscriptionKey, string baseUri)
        {
            emoClient = new EmotionServiceClient(subscriptionKey, baseUri);
        }

        public async Task<EmoPicture> DetectAndExtractFacesAsync(Stream imageStream)
        {
            Emotion[] emotions = await emoClient.RecognizeAsync(imageStream);

            var emoPicture = new EmoPicture();

            emoPicture.Faces = ExtractFaces(emotions, emoPicture);

            return emoPicture;
        }

        private ObservableCollection<EmoFace> ExtractFaces(Emotion[] emotions, EmoPicture emoPicture)
        {
            var listFaces = new ObservableCollection<EmoFace>();
            foreach (var emotion in emotions)
            {
                var emoFace = new EmoFace()
                {
                    X = emotion.FaceRectangle.Left,
                    Y = emotion.FaceRectangle.Top,
                    Width = emotion.FaceRectangle.Width,
                    Height = emotion.FaceRectangle.Height,
                    Picture = emoPicture
                };

                emoFace.Emotions = ProcessEmotions(emotion.Scores, emoFace);

                listFaces.Add(emoFace);

            }

            return listFaces;
        }
        /*
         * La funcion extraera cada emocion de la variable scores para transformarla en un 
         * array de EmoEmotion
         * scores: es la respuesta con cada valor de emociones en una sola variable
         * 
         */
        private ObservableCollection<EmoEmotion> ProcessEmotions(EmotionScores scores, EmoFace emoFace)
        {
            // se usara reflection
            var emotionList = new ObservableCollection<EmoEmotion>();

            // Obtengo los campos del objeto
            // BindingFlags.Public: para obtener solo las propiedades publicas
            // BindingFlags.Instance: Para que no me devuelva los campos estaticos
            var properties = scores.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // como properties es collection, puedo usar linq
            //var filterProperties = properties.Where(p => p.PropertyType == typeof(float));
            var filterProperties = from p in properties
                                   where p.PropertyType == typeof(float)
                                   select p;

            // variable que se usara para tener el tipo de emotion
            var emotype = EmoEmotionEnum.Undetermined;
            foreach (var prop in filterProperties)
            {
                // trato de determinar el valor de la enumeracion
                if (!Enum.TryParse<EmoEmotionEnum>(prop.Name, out emotype))
                    emotype = EmoEmotionEnum.Undetermined;

                var emoEmotion = new EmoEmotion();
                // obtengo el valor de la emotion
                emoEmotion.Score = (float) prop.GetValue(scores);
                emoEmotion.EmotionType = emotype;
                emoEmotion.Face = emoFace;

                emotionList.Add(emoEmotion);
            }

            return emotionList;

        }
    }
}