using api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using webapiii.Models;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace webapiii.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandController : ControllerBase
    {
        private readonly BrandContext _dbContext;

        public BrandController(BrandContext dbContext)
        {
            _dbContext = dbContext;
        }
        //[ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet]
        [Route("login")]
        public IActionResult Login(string username, string password)
        {
            if (username == "gana" && password == "12345")
            {
                var token = CreateJwtToken();
                return Ok(new { status = true, Message = "success", Data = new { Token = token } });


            }
            return BadRequest();
        }


        private string CreateJwtToken()
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,"1"),
                
               new Claim(JwtRegisteredClaimNames.Email,"gana"),
                 new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
            };
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("this is my custom Secret key for authentication"));
            var token = new JwtSecurityToken(
                issuer: "https://example.com",
                audience: "https://example.com",
                expires: DateTime.Now.AddDays(1),
                claims: claims,
                signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)


                );
            return new JwtSecurityTokenHandler().WriteToken(token); 
        
        }








        [HttpGet]
        public async Task<ActionResult<IEnumerable<Brand>>> GetBrands()
        {
            if (_dbContext.brands == null)
            {
                return NotFound();
            }


            return await _dbContext.brands.ToListAsync();

        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Brand>> GetBrands(int id)
        {
            if (_dbContext.brands == null)
            {
                return NotFound();
            }
            var brand = await _dbContext.brands.FindAsync(id);
            if (brand == null)
            {
                return NotFound();
            }

            return brand;

        }

        [HttpPost]
        public async Task<ActionResult<Brand>> PostBrand(Brand brand)
        {
            _dbContext.brands.Add(brand);
            await _dbContext.SaveChangesAsync();
            return brand;
            //return CreatedAtAction(nameof(GetBrands), new { id = brand.Id }, brand);


        }



        [HttpDelete("{id}")]
        public async Task<ActionResult<Brand>> DeleteBrand(int id)
        {
            if (_dbContext.brands == null)
            {
                return NotFound();
            }
            var brand = await _dbContext.brands.FindAsync(id);
            if (brand == null)
            {
                return NotFound();
            }
            _dbContext.brands.Remove(brand);
            await _dbContext.SaveChangesAsync();

            return NoContent();

        }



        [HttpPut("{id}")]
        public async Task<IActionResult> PutBrand(int id, Brand brand)
        {
            if (id != brand.Id)
            {
                return BadRequest();
            }
            //_dbContext.brands.Update(brand);
            _dbContext.Entry(brand).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!BrandExists(id))
            {
                return NotFound();
            }

            return NoContent();
        }

        private bool BrandExists(long id)
        {
            throw new NotImplementedException();
        }

        [HttpPost("upload")]
        public async Task<ActionResult> UploadFile([FromForm] Fileupload filesmodel)
        {
            ModelState.Remove("Id");
            if (filesmodel.File == null && filesmodel.File.Length == 0)
            {
            }

            var folderName = Path.Combine("Resources", "All files");
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            if (!Directory.Exists(pathToSave))
            {
                Directory.CreateDirectory(pathToSave);
            }

            var filename = filesmodel.File.FileName;
            var fullpath = Path.Combine(pathToSave, filename);

            var dbpath = Path.Combine(folderName, filename);
            filesmodel.filepath = fullpath;
            filesmodel.Id = 0;
            if (System.IO.File.Exists(fullpath))
            {
                return BadRequest("file already exist");

            }
            using (var stream = new FileStream(fullpath, FileMode.Create))
            {
                _dbContext.files.Add(filesmodel);
                await _dbContext.SaveChangesAsync();

                filesmodel.File.CopyTo(stream);
            }
            

            return Ok(new { dbpath });


        }
    }
}

