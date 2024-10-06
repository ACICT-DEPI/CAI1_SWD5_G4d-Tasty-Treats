﻿using Microsoft.AspNetCore.Mvc;
using TastyTreats.Models;
using TastyTreats.Repositories;

namespace TastyTreats.Controllers
{
    public class ItemController : Controller
    {
        //injection
        IItemRepository _itemRepository;
        public ItemController(IItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
        }
        

        public IActionResult Index()
        {
            var items = _itemRepository.GetAll();
            if (items == null)
            {
                return NotFound();
            }
            return View(items);
        }
        public IActionResult Details(int id)
        {
            var item = _itemRepository.GetById(id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View(new Item());
        }

        [HttpPost]
        public IActionResult Add(Item item, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    Console.WriteLine($"Uploading file: {imageFile.FileName}, Size: {imageFile.Length}");

                    string fileName = Path.GetFileNameWithoutExtension(imageFile.FileName);
                    string extension = Path.GetExtension(imageFile.FileName);
                    fileName = fileName + "_" + Guid.NewGuid().ToString() + extension;

                    string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img");
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                        Console.WriteLine($"Created directory: {folderPath}");
                    }

                    string path = Path.Combine(folderPath, fileName);

                    try
                    {
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            imageFile.CopyTo(stream);
                        }
                        item.ItemPicture = "/img/" + fileName;
                        Console.WriteLine($"Image saved to {path}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error saving image: {ex.Message}");
                        ModelState.AddModelError("ImageFile", "Could not save the image.");
                    }
                }
                else
                {
                    Console.WriteLine("No image file uploaded.");
                }

                try
                {
                    _itemRepository.Add(item);
                    _itemRepository.Save();
                    Console.WriteLine("Item saved successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error saving item: {ex.Message}");
                    ModelState.AddModelError("", "Could not save the item.");
                }

                return RedirectToAction("Index");
            }

            Console.WriteLine("Model state is invalid");
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine(error.ErrorMessage); 
            }
            return View(item);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var item = _itemRepository.GetById(id);
            if (item != null)
            {
                return View(item);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult Edit(int id, Item newItem, IFormFile imageFile)
        {
            if (newItem.Name != null)
            {
                var oldItem = _itemRepository.GetById(id);

                if (oldItem == null)
                {
                    return NotFound(); 
                }

                if (imageFile != null && imageFile.Length > 0)
                {
                    string fileName = Path.GetFileNameWithoutExtension(imageFile.FileName);
                    string extension = Path.GetExtension(imageFile.FileName);
                    fileName = fileName + "_" + Guid.NewGuid().ToString() + extension;

                    string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img");
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }
                    string path = Path.Combine(folderPath, fileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        imageFile.CopyTo(stream);
                    }

                    newItem.ItemPicture = "/img/" + fileName;
                }
                else
                {
                    newItem.ItemPicture = oldItem.ItemPicture;
                }

                oldItem.ItemPicture = newItem.ItemPicture;
                oldItem.Name = newItem.Name;
                oldItem.Price = newItem.Price;
                oldItem.Discount = newItem.Discount;
                oldItem.Description = newItem.Description;
                oldItem.Category = newItem.Category;
                oldItem.Availability = newItem.Availability;

                _itemRepository.Update(oldItem); 
                _itemRepository.Save();

                return RedirectToAction("Index");
            }

            return View(newItem);
        }


        [HttpGet]
        public IActionResult Delete(int id)
        {
            var item = _itemRepository.GetById(id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult Deleted(int id)
        {
            _itemRepository.Delete(id);
            _itemRepository.Save();
            return RedirectToAction("Index");
        }
    }
}
