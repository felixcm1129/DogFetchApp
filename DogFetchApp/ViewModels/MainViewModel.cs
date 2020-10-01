using ApiHelper;
using ApiHelper.Models;
using DogFetchApp.Commands;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Image = System.Drawing.Image;

namespace DogFetchApp.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private string selectedBreed;
        private string selectedPhoto;
        private string urlPhoto;
        private string selectedNumber;
        public int n = 0;
        public int maxNumber;
        public int currentNumber;
        private string next = "1";
        private string previous = "0";
        private string url_selected_Picture;

        private List<string> breeds;
        private List<string> photos;
        private List<string> number;
        private List<string> photosUrl;

        public DelegateCommand<string> SetImageCommand { get; private set; }
        public DelegateCommand<string> PreviousCommand { get; private set; }
        public DelegateCommand<string> NextCommand { get; private set; }
        public DelegateCommand<string> ChangeLanguageCommand { get; private set; }

        public string Next
        {
            get => next;
            set
            {
                next = value;
                OnPropertyChanged();
                NextCommand.RaiseCanExecuteChanged();
                PreviousCommand.RaiseCanExecuteChanged();
            }
        }
        public string Previous
        {
            get => previous;
            set
            {
                previous = value;
                OnPropertyChanged();
                PreviousCommand.RaiseCanExecuteChanged();
            }
        }

        public string Url_Selected_Picture
        {
            get => url_selected_Picture;
            set
            {
                url_selected_Picture = value;
                OnPropertyChanged();
            }
        }

        public List<string> Number
        {
            get => number;
            set
            {
                number = value;
                OnPropertyChanged();
            }
        }

        public string UrlPhoto
        {
            get => urlPhoto;
            set
            {
                urlPhoto = value;
                OnPropertyChanged();
            }
        }

        public List<string> Photos
        {
            get => photos;
            set
            {
                photos = value;
                OnPropertyChanged();
            }
        }

        public List<string> PhotosUrl
        {
            get => photosUrl;
            set 
            {
                photosUrl = value;
                OnPropertyChanged();
            }
        }

        public string SelectedPhoto
        {
            get => selectedPhoto;
            set
            {
                selectedPhoto = value;
                OnPropertyChanged();
            }
        }

        public string SelectedNumber
        {
            get => selectedNumber;
            set
            {
                selectedNumber = value;
                OnPropertyChanged();
            }
        }

        public List<string> Breeds
        {
            get => breeds;
            set
            {
                breeds = value;
                OnPropertyChanged();
            }
        }

        public string SelectedBreed
        {
            get => selectedBreed;
            set
            {
                selectedBreed = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            initValues();
            SetImageCommand = new DelegateCommand<string>(SetImageUrl);
            PreviousCommand = new DelegateCommand<string>(PreviousImage, CanExecutePreviousImage);
            NextCommand = new DelegateCommand<string>(NextImage, CanExcuteNextImage);
            ChangeLanguageCommand = new DelegateCommand<string>(ChangeLanguage);
        }


        private void ChangeLanguage(string param)
        {
            Properties.Settings.Default.Language = param;
            Properties.Settings.Default.Save();

            if (MessageBox.Show(
                    "Please restart app for the settings to take effect.\nWould you like to restart?",
                    "Warning!",
                    MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                Restart();
        }

        void Restart()
        {
            var filename = Application.ResourceAssembly.Location;
            var newFile = Path.ChangeExtension(filename, ".exe");
            Process.Start(newFile);
            Application.Current.Shutdown();
        }

        private bool CanExcuteNextImage(string obj)
        {
            if (Next == "1")
            {
                return true;
            }
            else return false;
        }

        private void NextImage(string obj)
        {
            if(currentNumber < maxNumber)
            {
                currentNumber += 1;
                Debug.WriteLine(currentNumber);
                Previous = "1";
                LoadImage(currentNumber);
                if(currentNumber == 0)
                {
                    Previous = "0";
                }
                if(currentNumber == maxNumber)
                {
                    Next = "0";
                }
            }
        }

        private bool CanExecutePreviousImage(string obj)
        {
            if (Previous == "1")
            {
                return true;
            }
            else return false;
        }

        private void PreviousImage(string obj)
        {
            if(currentNumber > 0)
            {
                currentNumber -= 1;
                Next = "1";
                LoadImage(currentNumber);
                if(currentNumber == 0)
                {
                    Previous = "0";
                }
            }
        }

        private async void initValues()
        {
            Photos = new List<string>();
            PhotosUrl = new List<string>();
            SetNumberList();
            await LoadBreedsList();
        }

        private void SetNumberList()
        {
            Number = new List<string>();
            Number.Add("1");
            Number.Add("3");
            Number.Add("5");
            Number.Add("7");
            Number.Add("10");
            SelectedNumber = Number[0];
        }

        private async Task LoadBreedsList()
        {
            //setup pour aller chercher la liste de toute les races et la mettre dans une List<string>
            Breeds = new List<string>();

            var breedInfo = await DogApiProcessor.LoadBreedList();
            int lenght = breedInfo.message.Count;

            for (int i = 0; i < lenght; i++)
            {
                Breeds.Add(breedInfo.message[i]);
            }
            SelectedBreed = breedInfo.message[0];
        }

        private async void SetImageUrl(string obj)
        {
            currentNumber = 0;
            if(Next == "0")
            {
                Next = "1";
            }
            

            Photos.Clear();
            PhotosUrl.Clear();

            Debug.WriteLine("Beginning....");

            //création dossier images

            string path = Directory.GetCurrentDirectory();
            string folderName = path + "/images";
            System.IO.Directory.CreateDirectory(folderName);

            //creation des photos dans le dossier images

            PhotoMessage tempPhoto = new PhotoMessage();
            
            int number = Int16.Parse(SelectedNumber);

            maxNumber = number - 1;

            var temp = SelectedBreed;

            for (int i = 1; i <= number; i++)
            {
                n++;
                string tempPath = path + "/images/img" + n.ToString() + ".jpg";
                //setup pour l'url des photos selon le nombre choisi et la race
                tempPhoto.message = await DogApiProcessor.GetImageUrl(temp);

                UrlPhoto = tempPhoto.message;

                PhotosUrl.Add(UrlPhoto);

                Debug.WriteLine(UrlPhoto);

                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(new Uri(UrlPhoto), tempPath);
                    var tmpImg = Image.FromFile(tempPath);
                    Photos.Add(tempPath);
                    //var img = new Bitmap(tmpImg);
                    //img.Save(tempPath, ImageFormat.Jpeg);
                }

            }

            Debug.WriteLine(next);
            Debug.WriteLine(currentNumber);
            Debug.WriteLine(maxNumber);

            SelectedPhoto = Photos[0];
            Url_Selected_Picture = PhotosUrl[0];
        }

        private void LoadImage(int imageNumber)
        {
            SelectedPhoto = Photos[imageNumber];
            Url_Selected_Picture = PhotosUrl[imageNumber];
        }

    }
}
