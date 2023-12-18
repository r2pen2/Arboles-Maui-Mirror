using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArbolesMAUI.Model;

/**
 * Code reference: https://github.com/KIGAMESYTB/Upload-Image-From-Phone-Maui-Dotnet-App
**/


namespace ArbolesMAUI.ViewModels
{
    public class UploadImage
    {
        /// <summary>
        /// Open Media Picker
        /// </summary>
        /// <returns></returns>
        public async Task<FileResult> OpenMediaPickerAsync()
        {
            try
            {
                var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Please a pick photo"
                });

                if (result.ContentType == "image/png" ||
                    result.ContentType == "image/jpeg" ||
                    result.ContentType == "image/jpg")
                    return result;
                else
                    await App.Current.MainPage.DisplayAlert("Error: Image Type", "Please choose a new image", "Ok");

                return null;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Handles photo capture operation
        /// </summary>
        /// <returns>FileResult</returns>
        public async Task<FileResult> TakePhoto()
        {
            try
            {
                if (MediaPicker.Default.IsCaptureSupported)
                {
                    FileResult photo = await MediaPicker.Default.CapturePhotoAsync();

                    if (photo != null)
                    {
                        // save the file into local storage
                        string localFilePath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);

                        using Stream sourceStream = await photo.OpenReadAsync();
                        using FileStream localFileStream = File.OpenWrite(localFilePath);

                        await sourceStream.CopyToAsync(localFileStream);

                        return photo;
                    }
                    else
                        await App.Current.MainPage.DisplayAlert("Error: Capture Type", "No Image Was Captured", "Ok");
                }
                else
                    await App.Current.MainPage.DisplayAlert("Error: Capture Type", "Photo Capture Not Supported", "Ok");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }

        /// <summary>
        /// Convert FileResult to Stream
        /// </summary>
        /// <param name="fileResult">FileResult</param>
        /// <returns>Stream</returns>
        public async Task<Stream> FileResultToStream(FileResult fileResult)
        {
            if(fileResult == null)
                return null;

            return await fileResult.OpenReadAsync();
        }

        /// <summary>
        /// Convert byte[] to Stream
        /// </summary>
        /// <param name="bytes">byte[]</param>
        /// <returns>Stream</returns>
        public Stream ByteArrayToStream(byte[] bytes)
        {
            return new MemoryStream(bytes);
        }

        /// <summary>
        /// Convert byte[] to string
        /// </summary>
        /// <param name="bytes">byte[]</param>
        /// <returns>string</returns>
        public string ByteBase64ToString(byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Convert string to byte[]
        /// </summary>
        /// <param name="text">string</param>
        /// <returns>byte[]</returns>
        public byte[] StringToByteBase64(string text)
        {
            return Convert.FromBase64String(text);
        }

        /// <summary>
        /// Upload a image
        /// </summary>
        /// <param name="fileResult">FileResult</param>
        /// <returns>ImageFile class</returns>
        public async Task<ImageFile> Upload(FileResult fileResult)
        {
            byte[] bytes;

            try
            {
                using (var ms = new MemoryStream())
                {
                    var stream = await FileResultToStream(fileResult);
                    stream.CopyTo(ms);
                    bytes = ms.ToArray();
                }

                return new ImageFile
                {
                    byteBase64 = ByteBase64ToString(bytes),
                    ContentType = fileResult.ContentType,
                    FileName = fileResult.FileName
                };
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
