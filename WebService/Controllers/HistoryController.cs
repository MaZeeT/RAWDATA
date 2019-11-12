using AutoMapper;
using DatabaseService.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebService.Controllers
{
    [ApiController]
    [Route("api/history")]
    public class HistoryController
    {
        private IHistoryService _historyService;
        private IMapper _mapper;

        public HistoryController(IHistoryService historyService, IMapper mapper)
        {
            _historyService = historyService;
            _mapper = mapper;
        }
        
        
        
        
    }
}