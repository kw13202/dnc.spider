using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dnc.efcontext;
using dnc.viewmodel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace dnc.spider.webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuartzController : ControllerBase
    {
        private readonly IScheduler _scheduler;
        private readonly EfContext _context;

        public QuartzController(EfContext context,IScheduler scheduler)
        {
            _context = context;
            _scheduler = scheduler;
        }

        [HttpPost]
        public async Task<IActionResult> JobStartNow([FromBody]QuartzVM quartzVM)
        {
            if (!quartzVM.verify(out string msg))
            {
                return BadRequest(msg);
            }

            if (!_scheduler.IsStarted)
            {
                await _scheduler.Start();
            }

            if (quartzVM.TypeId == 0)
            {
                var model = await _context.QuartzInfos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == quartzVM.Id);
                if (model == null)
                {
                    return BadRequest("找不到Id对应的数据");
                }

                // 创建触发器
                var trigger = TriggerBuilder.Create()
                                    .StartNow()
                                    .Build();
                // 创建任务
                Type type = Type.GetType(model.FullClassName);
                var jobDetail = JobBuilder.Create(type)
                                    .Build();

                await _scheduler.ScheduleJob(jobDetail, trigger);

            }


            return Ok();
        }
    }
}