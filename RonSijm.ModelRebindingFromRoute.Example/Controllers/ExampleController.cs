using Microsoft.AspNetCore.Mvc;
using RonSijm.ModelRebindingFromRoute.Example.Models;

namespace RonSijm.ModelRebindingFromRoute.Example.Controllers
{
    [ApiController]
    public class ExampleController : ControllerBase
    {
        [HttpPost("[controller]/book/{bookId}/page/{pageId}")]
        public string Echo(BookRequestModel bookRequestModel)
        {
            return bookRequestModel.BookId + " - " + bookRequestModel.PageId;
        }
    }
}
