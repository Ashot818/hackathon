using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityLens.Services.Concrete;

public class ImageService
{
    private readonly string _imagesFolder;

    public ImageService(IWebHostEnvironment env)
    {
        _imagesFolder = Path.Combine(env.WebRootPath, "images");
        if (!Directory.Exists(_imagesFolder))
            Directory.CreateDirectory(_imagesFolder);
    }
    

   

    //public string SaveBase64Image(string base64)
    //{
    //    var bytes = Convert.FromBase64String(base64.Split(',').Last());
    //    var fileName = $"{Guid.NewGuid()}.png";
    //    var filePath = Path.Combine(_imagesFolder, fileName);
    //    File.WriteAllBytes(filePath, bytes);
    //    return $"/images/{fileName}"; 
    //}
}
