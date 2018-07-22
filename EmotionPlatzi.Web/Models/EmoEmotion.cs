using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmotionPlatzi.Web.Models
{
    public class EmoEmotion
    {
        public int Id { get; set; }
        public float Score { get; set; }

        public EmoEmotionEnum EmotionType { get; set; }

        // Relaciones
        // Se indica la relacion con EmoFace
        public int EmoFaceId { get; set; }
        public virtual EmoFace Face { get; set; }
    }
}