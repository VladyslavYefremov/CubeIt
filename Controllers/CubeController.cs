using System;
using System.Collections.Generic;
using System.Web.Http;

using E_2.Models;
using Newtonsoft.Json;

namespace E_2.Controllers
{
    [RoutePrefix("api/Cube")]
    public class CubeController : ApiController
    {
        [HttpPost]
        [Route("Cross")]
        public String PostCross([FromBody] string cube)
        {
            try
            {
                var cubeData = JsonConvert.DeserializeObject<List<CubeData>>(cube);
                var cubeObject = new Cube(cubeData);
                var cubeSolver = new CubeSolver(cubeObject);
                cubeSolver.Invoke();

                return cubeObject.Moves;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
}
