using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CipherServices.Services;
using CipherServices.Data;
using CipherServices.Models;

namespace CipherServices.Pages
{
  public class IndexModel : PageModel
  {
    public Dictionary<string, string> Secrets { get; set; }
    [BindProperty]
    public Message NewMessage { get; set; }

    private readonly MessageContext _context;
    private readonly IDecrypter _decrypter;
    private readonly IEncrypter _encrypter;

    public IndexModel(IDecrypter decrypter, MessageContext context, IEncrypter encrypter)
    {
      _decrypter = decrypter;
      _context = context;
      _encrypter = encrypter;
    }

    public async Task<IActionResult> OnGetAsync()
    {
      await LoadSecretsAsync(_decrypter, _context);
      return Page();
    }

    private async Task LoadSecretsAsync(IDecrypter decrypter, MessageContext context)
    {
      Secrets = new Dictionary<string, string>();
      var messages = await context.Messages.ToListAsync();

      foreach (Message m in messages)
      {
        Secrets.TryAdd(m.Text, decrypter.Decrypt(m.Text));
      }
    }

    public async Task<IActionResult> OnPostAsync()
    {
      if(!ModelState.IsValid)
      {
        string cleanedText = NewMessage.Text.Trim().ToLower();
       Message message = new Message 
       {
         Text = _encrypter.Encrypt(cleanedText);
       };
       _context.Messages.Add(message);
       await _context.SaveChangesAsync();
       return RedirectToPage("/Index");
      }else{
        await LoadSecretsAsync(_decrypter, _context);
      return Page();
      }  
      
       
      
    }
  }
}
