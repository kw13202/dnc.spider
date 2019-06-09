using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dnc.efcontext;
using dnc.model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dnc.spider.webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoodsController : ControllerBase
    {
        private EfContext _context;
        public GoodsController(EfContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 获取商品列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<List<Goods>> GetList()
        {
            var list = await _context.Goods.AsNoTracking().ToListAsync();
            return list;
        }


    }
}