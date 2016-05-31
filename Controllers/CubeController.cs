using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Web.Helpers;
using System.Web.Http;

using E_2.Models;
using Newtonsoft.Json;

namespace E_2.Controllers
{
    [RoutePrefix("api/Cube")]
    public class CubeController : ApiController
    {
        public CubeController()
        {
                
        }

        [HttpGet]
        public IEnumerable<String> GetInformation()
        {
            return new[]
            {
                "lol",
                "efr",
                "vladyslav"
            };
        }

        // [    {"direction":"u","cells":["y","w","y","w","y","w","y","w","y"]
        //}         ,{"direction":"b","cells":["o","r","o","r","o","r","o","r","o"]
        //            },{"direction":"f","cells":["r","o","r","o","r","o","r","o","r"]},
        //          {"direction":"l","cells":["b","g","b","g","b","g","b","g","b"]},{"direction":"r","cells":["g","b","g","b","g","b","g","b","g"]},{"direction":"d","cells":["w","y","w","y","w","y","w","y","w"]}]

        [HttpPost]
        [Route("Cross")]
        public String PostCross([FromBody] string cube)
        {
            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject(cube);
            dynamic obj2 = System.Web.Helpers.Json.Decode(cube);

            try
            {
                var cubeData = JsonConvert.DeserializeObject<List<CubeData>>(cube);
                var cubeObj = new Cube(cubeData);
                var cubeCross = new CubeCross(cubeObj);
                //var a = cubeCross.FindCornerAndGetState();

                cubeCross.InvokeCorner();

                return cubeObj.Moves;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
}
