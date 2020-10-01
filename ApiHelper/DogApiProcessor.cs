using ApiHelper.Models;
using Microsoft.VisualBasic.CompilerServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

namespace ApiHelper
{
    public class DogApiProcessor
    {
        public static async Task<BreedsMessage> LoadBreedList()
        {
            ///TODO : À compléter LoadBreedList
            /// Attention le type de retour n'est pas nécessairement bon
            /// J'ai mis quelque chose pour avoir une base
            /// TODO : Compléter le modèle manquant
            /// 

            string url = $"https://dog.ceo/api/breeds/list";

            using (HttpResponseMessage response = await ApiHelper.ApiClient.GetAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    var message = await response.Content.ReadAsStringAsync();
                    BreedsMessage breeds = JsonConvert.DeserializeObject<BreedsMessage>(message);
                    return breeds;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
                
            }


        }

        public static async Task<string> GetImageUrl(string breed)
        {
            /// TODO : GetImageUrl()
            /// TODO : Compléter le modèle manquant
            string photoBreed = breed;
            string url = $"https://dog.ceo/api/breed" + "/" + photoBreed + "/images/random";

            using (HttpResponseMessage response = await ApiHelper.ApiClient.GetAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    PhotoMessage urlPhoto = new PhotoMessage();
                    var photoUrl = await response.Content.ReadAsStringAsync();
                    urlPhoto = JsonConvert.DeserializeObject<PhotoMessage>(photoUrl);
                    return urlPhoto.message;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }

            }
        }

    }
}
